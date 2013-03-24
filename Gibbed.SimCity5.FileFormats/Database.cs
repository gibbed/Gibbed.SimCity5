/* Copyright (c) 2013 Rick (rick 'at' gibbed 'dot' us)
 * 
 * This software is provided 'as-is', without any express or implied
 * warranty. In no event will the authors be held liable for any damages
 * arising from the use of this software.
 * 
 * Permission is granted to anyone to use this software for any purpose,
 * including commercial applications, and to alter it and redistribute it
 * freely, subject to the following restrictions:
 * 
 * 1. The origin of this software must not be misrepresented; you must not
 *    claim that you wrote the original software. If you use this software
 *    in a product, an acknowledgment in the product documentation would
 *    be appreciated but is not required.
 * 
 * 2. Altered source versions must be plainly marked as such, and must not
 *    be misrepresented as being the original software.
 * 
 * 3. This notice may not be removed or altered from any source
 *    distribution.
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Gibbed.RefPack;

namespace Gibbed.SimCity5.FileFormats
{
    public class Database
    {
        public class Entry
        {
            public bool IsCompressed { get; set; }
            public uint CompressedSize { get; set; }
            public uint UncompressedSize { get; set; }
        }

        private class StreamEntry : Entry
        {
            public long Offset { get; set; }
            public DatabasePackedFile.CompressionScheme CompressionScheme { get; set; }
            public ushort Flags { get; set; }
        }

        private class MemoryEntry : Entry
        {
            public byte[] Data;
        }

        private readonly Stream _Stream;
        private readonly long _BaseOffset;
        private long _EndOfDataOffset;

        public IEnumerable<ResourceKey> Keys
        {
            get { return this._Entries.Keys; }
        }

        private readonly Dictionary<ResourceKey, Entry> _Entries;
        private readonly Dictionary<ResourceKey, StreamEntry> _OriginalEntries;

        public Database(Stream stream)
            : this(stream, true)
        {
        }

        public Database(Stream stream, bool readExisting)
        {
            if (stream.CanSeek == false ||
                stream.CanRead == false)
            {
                throw new ArgumentException("stream must have seek and read access", "stream");
            }

            this._Stream = stream;
            this._BaseOffset = this._Stream.Position;

            this._Entries = new Dictionary<ResourceKey, Entry>();

            this._OriginalEntries = new Dictionary<ResourceKey, StreamEntry>();

            if (readExisting == true)
            {
                var dbpf = new DatabasePackedFile();
                dbpf.Read(stream);

                this._EndOfDataOffset = 0;

                foreach (var entry in dbpf.Entries)
                {
                    this._Entries.Add(entry.Key,
                                      new StreamEntry()
                                      {
                                          IsCompressed = entry.IsCompressed,
                                          Offset = entry.Offset,
                                          CompressedSize = entry.CompressedSize,
                                          UncompressedSize = entry.UncompressedSize,
                                          CompressionScheme = entry.CompressionScheme,
                                          Flags = entry.Flags,
                                      });

                    if (entry.Offset + entry.CompressedSize > this._EndOfDataOffset)
                    {
                        this._EndOfDataOffset = entry.Offset + entry.CompressedSize;
                    }
                }
            }
            else
            {
                this._EndOfDataOffset = 0;
            }
        }

        public void DeleteResource(ResourceKey key)
        {
            if (this._Stream.CanWrite == false)
            {
                throw new NotSupportedException();
            }

            if (this._Entries.ContainsKey(key) == false)
            {
                throw new KeyNotFoundException();
            }

            var entry = this._Entries[key] as StreamEntry;
            if (entry != null)
            {
                this._OriginalEntries[key] = entry;
            }

            this._Entries.Remove(key);
        }

        public void MoveResource(ResourceKey oldKey, ResourceKey newKey)
        {
            if (this._Stream.CanWrite == false)
            {
                throw new NotSupportedException();
            }

            if (this._Entries.ContainsKey(oldKey) == false)
            {
                throw new KeyNotFoundException();
            }

            if (this._Entries.ContainsKey(newKey) == true)
            {
                throw new ArgumentException("database already contains the new resource key", "newKey");
            }

            var entry = this._Entries[oldKey];

            var streamEntry = entry as StreamEntry;
            if (streamEntry != null)
            {
                this._OriginalEntries[oldKey] = streamEntry;
            }

            this._Entries.Remove(oldKey);
            this._Entries.Add(newKey, entry);
        }

        public byte[] GetResource(ResourceKey key)
        {
            if (this._Entries.ContainsKey(key) == false)
            {
                throw new KeyNotFoundException();
            }

            var streamEntry = this._Entries[key] as StreamEntry;
            if (streamEntry != null)
            {
                this._Stream.Seek(this._BaseOffset + streamEntry.Offset, SeekOrigin.Begin);

                byte[] data;
                if (streamEntry.IsCompressed == true)
                {
                    data = this._Stream.RefPackDecompress();

                    if (data.Length != streamEntry.UncompressedSize)
                    {
                        throw new InvalidOperationException("decompressed data not same length as specified in index");
                    }
                }
                else
                {
                    data = new byte[streamEntry.UncompressedSize];
                    this._Stream.Read(data, 0, data.Length);
                }

                return data;
            }

            var memoryEntry = this._Entries[key] as MemoryEntry;
            if (memoryEntry != null)
            {
                return (byte[])(memoryEntry.Data.Clone());
            }

            throw new InvalidOperationException();
        }

        public Stream GetResourceStream(ResourceKey key)
        {
            return new MemoryStream(this.GetResource(key));
        }

        public byte[] GetRawResource(ResourceKey key)
        {
            if (this._Entries.ContainsKey(key) == false ||
                (this._Entries[key] is StreamEntry) == false)
            {
                throw new KeyNotFoundException();
            }

            var entry = (StreamEntry)this._Entries[key];
            this._Stream.Seek(this._BaseOffset + entry.Offset, SeekOrigin.Begin);

            var data = new byte[entry.CompressedSize];
            this._Stream.Read(data, 0, data.Length);
            return data;
        }

        public void SetResource(ResourceKey key, byte[] data)
        {
            if (this._Stream.CanWrite == false)
            {
                throw new NotSupportedException();
            }

            if (this._Entries.ContainsKey(key) && this._Entries[key] is StreamEntry)
            {
                this._OriginalEntries[key] = (StreamEntry)this._Entries[key];
            }

            this._Entries[key] = new MemoryEntry
            {
                IsCompressed = false,
                CompressedSize = (uint)data.Length,
                UncompressedSize = (uint)data.Length,
                Data = (byte[])(data.Clone()),
            };
        }

        public void SetResourceStream(ResourceKey key, Stream stream)
        {
            var data = new byte[stream.Length];

            long oldPosition = stream.Position;
            stream.Seek(0, SeekOrigin.Begin);
            stream.Read(data, 0, data.Length);
            stream.Seek(oldPosition, SeekOrigin.Begin);

            this.SetResource(key, data);
        }

        public void Commit()
        {
            this.Commit(false);
        }

        public void Commit(bool cleanCommit)
        {
            if (this._Stream.CanWrite == false)
            {
                throw new NotSupportedException();
            }

            var dbpf = new DatabasePackedFile
            {
                Version = new Version(2, 0),
            };

            if (cleanCommit == false)
            {
                if (this._EndOfDataOffset == 0)
                {
                    // new archive
                    this._Stream.Seek(this._BaseOffset, SeekOrigin.Begin);
                    dbpf.WriteHeader(this._Stream, 0, 0);
                    this._EndOfDataOffset = this._Stream.Position - this._BaseOffset;
                }

                foreach (var kvp in this._Entries)
                {
                    var entry = new DatabasePackedFile.Entry
                    {
                        Key = kvp.Key,
                    };

                    var memoryEntry = kvp.Value as MemoryEntry;
                    if (memoryEntry != null)
                    {
                        byte[] compressed;
                        bool success = memoryEntry.Data.RefPackCompress(out compressed);
                        if (success == true)
                        {
                            entry.UncompressedSize = (uint)(memoryEntry.Data.Length);
                            entry.CompressedSize = (uint)(compressed.Length) | 0x80000000;
                            entry.CompressionScheme = DatabasePackedFile.CompressionScheme.RefPack;
                            entry.Flags = 1;
                            memoryEntry.Data = compressed;
                        }
                        else
                        {
                            entry.UncompressedSize = memoryEntry.UncompressedSize;
                            entry.CompressedSize = memoryEntry.CompressedSize | 0x80000000;
                            entry.CompressionScheme = DatabasePackedFile.CompressionScheme.None;
                            entry.Flags = 1;
                        }

                        // Is this replacing old data?
                        if (this._OriginalEntries.ContainsKey(kvp.Key) == true)
                        {
                            var stream = this._OriginalEntries[kvp.Key];

                            // Let's see if the new data can fit where the old data was
                            if (memoryEntry.Data.Length <= stream.CompressedSize)
                            {
                                entry.Offset = stream.Offset;
                                this._Stream.Seek(this._BaseOffset + stream.Offset, SeekOrigin.Begin);
                                this._Stream.Write(memoryEntry.Data, 0, memoryEntry.Data.Length);
                            }
                            else
                            {
                                entry.Offset = this._EndOfDataOffset;
                                this._Stream.Seek(this._BaseOffset + this._EndOfDataOffset, SeekOrigin.Begin);
                                this._Stream.Write(memoryEntry.Data, 0, memoryEntry.Data.Length);
                                this._EndOfDataOffset += memoryEntry.Data.Length;
                            }
                        }
                            // New data
                        else
                        {
                            entry.Offset = this._EndOfDataOffset;
                            this._Stream.Seek(this._BaseOffset + this._EndOfDataOffset, SeekOrigin.Begin);
                            this._Stream.Write(memoryEntry.Data, 0, memoryEntry.Data.Length);
                            this._EndOfDataOffset += memoryEntry.Data.Length;
                        }
                    }
                    else
                    {
                        var streamEntry = kvp.Value as StreamEntry;
                        if (streamEntry != null)
                        {
                            entry.CompressedSize = streamEntry.CompressedSize | 0x80000000;
                            entry.UncompressedSize = streamEntry.UncompressedSize;
                            entry.Offset = streamEntry.Offset;
                            entry.CompressionScheme = streamEntry.CompressionScheme;
                            entry.Flags = streamEntry.Flags;
                        }
                        else
                        {
                            throw new InvalidOperationException();
                        }
                    }

                    dbpf.Entries.Add(entry);
                }

                this._Stream.Seek(this._BaseOffset + this._EndOfDataOffset, SeekOrigin.Begin);
                dbpf.WriteIndex(this._Stream);

                var indexSize = (uint)(this._Stream.Position - (this._BaseOffset + this._EndOfDataOffset));

                this._Stream.Seek(this._BaseOffset, SeekOrigin.Begin);
                dbpf.WriteHeader(this._Stream, this._EndOfDataOffset, indexSize);
            }
            else
            {
                Stream clean;
                string tempFileName = null;

                /* packages greater than 50mb will be cleaned with
                 * a file supported stream
                 */
                if (this._Stream.Length >= 1024 * 1024 * 50)
                {
                    tempFileName = Path.GetTempFileName();
                    clean = File.Create(tempFileName);
                }
                else
                {
                    clean = new MemoryStream();
                }

                dbpf.WriteHeader(clean, 0, 0);
                this._EndOfDataOffset = clean.Position;

                foreach (var kvp in this._Entries)
                {
                    var entry = new DatabasePackedFile.Entry
                    {
                        Key = kvp.Key,
                    };

                    var memoryEntry = kvp.Value as MemoryEntry;
                    if (memoryEntry != null)
                    {
                        byte[] compressed;
                        bool success = memoryEntry.Data.RefPackCompress(out compressed);
                        if (success == true)
                        {
                            entry.UncompressedSize = (uint)(memoryEntry.Data.Length);
                            entry.CompressedSize = (uint)(compressed.Length) | 0x80000000;
                            entry.CompressionScheme = DatabasePackedFile.CompressionScheme.None;
                            entry.Flags = 1;
                            entry.Offset = this._EndOfDataOffset;
                            memoryEntry.Data = compressed;
                        }
                        else
                        {
                            entry.UncompressedSize = memoryEntry.UncompressedSize;
                            entry.CompressedSize = memoryEntry.CompressedSize | 0x80000000;
                            entry.CompressionScheme = 0;
                            entry.Flags = 1;
                            entry.Offset = this._EndOfDataOffset;
                        }

                        clean.Write(memoryEntry.Data, 0, memoryEntry.Data.Length);
                        this._EndOfDataOffset += memoryEntry.Data.Length;
                    }
                    else
                    {
                        var streamEntry = kvp.Value as StreamEntry;
                        if (streamEntry != null)
                        {
                            entry.CompressedSize = streamEntry.CompressedSize | 0x80000000;
                            entry.UncompressedSize = streamEntry.UncompressedSize;
                            entry.CompressionScheme = streamEntry.CompressionScheme;
                            entry.Flags = streamEntry.Flags;
                            entry.Offset = this._EndOfDataOffset;

                            // Copy data
                            this._Stream.Seek(this._BaseOffset + streamEntry.Offset, SeekOrigin.Begin);
                            var data = new byte[4096];
                            var left = (int)streamEntry.CompressedSize;
                            while (left > 0)
                            {
                                int block = Math.Min(left, data.Length);
                                this._Stream.Read(data, 0, block);
                                clean.Write(data, 0, block);
                                left -= block;
                            }

                            this._EndOfDataOffset += streamEntry.CompressedSize;
                        }
                        else
                        {
                            throw new InvalidOperationException();
                        }
                    }

                    dbpf.Entries.Add(entry);
                }

                dbpf.WriteIndex(clean);

                var indexSize = (uint)(clean.Position - this._EndOfDataOffset);

                clean.Seek(0, SeekOrigin.Begin);
                dbpf.WriteHeader(clean, this._EndOfDataOffset, indexSize);

                // copy clean to real stream
                {
                    this._Stream.Seek(this._BaseOffset, SeekOrigin.Begin);
                    clean.Seek(0, SeekOrigin.Begin);

                    var data = new byte[4096];
                    long left = clean.Length;
                    while (left > 0)
                    {
                        var block = (int)Math.Min(left, data.Length);
                        clean.Read(data, 0, block);
                        this._Stream.Write(data, 0, block);
                        left -= block;
                    }
                }

                this._Stream.SetLength(this._BaseOffset + this._EndOfDataOffset + indexSize);
                clean.Close();

                if (tempFileName != null)
                {
                    File.Delete(tempFileName);
                }
            }

            this._Entries.Clear();
            this._OriginalEntries.Clear();

            foreach (var entry in dbpf.Entries)
            {
                this._Entries.Add(entry.Key,
                                  new StreamEntry()
                                  {
                                      IsCompressed = entry.IsCompressed,
                                      Offset = entry.Offset,
                                      CompressedSize = entry.CompressedSize,
                                      UncompressedSize = entry.UncompressedSize,
                                      CompressionScheme = entry.CompressionScheme,
                                      Flags = entry.Flags,
                                  });
            }
        }

        public void Rollback()
        {
            if (this._Stream.CanWrite == false)
            {
                throw new NotSupportedException();
            }

            foreach (var entry in this._OriginalEntries)
            {
                this._Entries[entry.Key] = entry.Value;
            }

            foreach (var entry in this._Entries.Where(entry => entry.Value is MemoryEntry).ToList())
            {
                this._Entries.Remove(entry.Key);
            }
        }
    }
}
