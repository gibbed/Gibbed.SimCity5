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
using System.Runtime.InteropServices;
using Gibbed.IO;

namespace Gibbed.SimCity5.FileFormats
{
    public class DatabasePackedFile
    {
        private Version _Version = new Version();
        private readonly List<Entry> _Entries = new List<Entry>();

        public bool IsBig { get; set; }

        public Version Version
        {
            get { return _Version; }
            set { _Version = value; }
        }

        public List<Entry> Entries
        {
            get { return _Entries; }
        }

        public long IndexOffset { get; set; }
        public long IndexSize { get; set; }

        public void Read(Stream input)
        {
            const Endian endian = Endian.Little;

            long indexCount;
            long indexSize;
            long indexOffset;

            var magic = input.ReadValueU32(endian);
            if (magic != Header.Signature &&
                magic != BigHeader.Signature)
            {
                throw new FormatException("not a database packed file");
            }

            this.IsBig = magic == BigHeader.Signature;

            if (this.IsBig == false)
            {
                var header = input.ReadStructure<Header>();
                if (header.IndexVersion != 3)
                {
                    throw new FormatException("index version was not 3");
                }

                if (header.Zero14 != 0 ||
                    header.Zero18 != 0 ||
                    header.Zero1C != 0)
                {
                    throw new FormatException();
                }

                if (header.Unknown0C != 0 ||
                    header.Unknown10 != 0 ||
                    header.Unknown20 != 0 ||
                    header.OldIndexOffset != 0)
                {
                    throw new FormatException();
                }

                if (header.Unset30 != 0 ||
                    header.Unset34 != 0 ||
                    header.Unset38 != 0 ||
                    header.Unset44 != 0 ||
                    header.Unset48 != 0 ||
                    header.Unset4C != 0 ||
                    header.Unset50 != 0 ||
                    header.Unset54 != 0 ||
                    header.Unset58 != 0 ||
                    header.Unset5C != 0)
                {
                    throw new FormatException();
                }

                if (header.MajorVersion != 3 ||
                    header.MinorVersion != 0)
                {
                    throw new FormatException();
                }

                this._Version = new Version(header.MajorVersion, header.MinorVersion);
                indexCount = header.IndexCount;
                indexOffset = header.IndexOffset != 0 ? header.IndexOffset : header.OldIndexOffset;
                indexSize = header.IndexSize;
            }
            else
            {
                var header = input.ReadStructure<BigHeader>();
                if (header.IndexVersion != 3)
                {
                    throw new FormatException("index version was not 3");
                }

                if (header.Zero14 != 0 ||
                    header.Zero18 != 0 ||
                    header.Zero1C != 0)
                {
                    throw new FormatException();
                }

                if (header.Unknown0C != 0 ||
                    header.Unknown10 != 0 ||
                    header.Unknown20 != 0)
                {
                    throw new FormatException();
                }

                if (header.Unset30 != 0 ||
                    header.Unset40 != 0 ||
                    header.Unset44 != 0 ||
                    header.Unset48 != 0 ||
                    header.Unset4C != 0 ||
                    header.Unset50 != 0 ||
                    header.Unset54 != 0 ||
                    header.Unset58 != 0 ||
                    header.Unset5C != 0 ||
                    header.Unset60 != 0 ||
                    header.Unset64 != 0 ||
                    header.Unset68 != 0 ||
                    header.Unset6C != 0 ||
                    header.Unset70 != 0 ||
                    header.Unset74 != 0)
                {
                    throw new FormatException();
                }

                if (header.MajorVersion != 2 ||
                    header.MinorVersion != 0)
                {
                    throw new FormatException();
                }

                this._Version = new Version(header.MajorVersion, header.MinorVersion);
                indexCount = header.IndexCount;
                indexOffset = header.IndexOffset;
                indexSize = header.IndexSize;
            }

            this.IndexOffset = indexOffset;
            this.IndexSize = indexSize;

            this._Entries.Clear();
            if (indexCount > 0)
            {
                input.Seek(indexOffset, SeekOrigin.Begin);

                using (var data = input.ReadToMemoryStream(indexSize))
                {
                    var globalIndexValues = data.ReadValueEnum<GlobalIndexValue>();
                    if ((globalIndexValues & GlobalIndexValue.Invalid) != 0)
                    {
                        throw new InvalidDataException("don't know how to handle this index data");
                    }

                    var hasGlobalTypeId = (globalIndexValues & GlobalIndexValue.TypeId) != 0;
                    var hasGlobalGroupId = (globalIndexValues & GlobalIndexValue.GroupId) != 0;
                    var hasGlobalInstanceIdHi = (globalIndexValues & GlobalIndexValue.UpperInstanceId) != 0;

                    uint globalTypeId = hasGlobalTypeId ? data.ReadValueU32(endian) : 0xFFFFFFFF;
                    uint globalGroupId = hasGlobalGroupId ? data.ReadValueU32(endian) : 0xFFFFFFFF;
                    uint globalInstanceIdHi = hasGlobalInstanceIdHi ? data.ReadValueU32(endian) : 0xFFFFFFFF;

                    if (hasGlobalInstanceIdHi == true &&
                        globalInstanceIdHi != 0)
                    {
                        // sanity check: SC5 appears to never use upper 32-bits of instance ID
                        throw new NotSupportedException();
                    }

                    for (int i = 0; i < indexCount; i++)
                    {
                        uint typeId = hasGlobalTypeId ? globalTypeId : data.ReadValueU32(endian);
                        uint groupId = hasGlobalGroupId ? globalGroupId : data.ReadValueU32(endian);

                        ulong instanceId = 0;
                        instanceId |= hasGlobalInstanceIdHi ? globalInstanceIdHi : data.ReadValueU32(endian);
                        instanceId <<= 32;
                        instanceId |= data.ReadValueU32();

                        var offset = this.IsBig == true ? data.ReadValueS64() : data.ReadValueS32(endian);
                        var compressedSize = data.ReadValueU32(endian);
                        var uncompressedSize = data.ReadValueU32(endian);

                        short compressionFlags;
                        ushort flags;
                        if (this.IsBig == false)
                        {
                            if ((compressedSize & 0x80000000) != 0)
                            {
                                compressedSize &= ~0x80000000;
                                compressionFlags = data.ReadValueS16(endian);
                                flags = data.ReadValueU16(endian);
                            }
                            else
                            {
                                throw new FormatException("strange index data");
                                compressionFlags = compressedSize == uncompressedSize ? (short)0 : (short)-1;
                                flags = 0;
                            }
                        }
                        else
                        {
                            compressionFlags = data.ReadValueS16(endian);
                            flags = data.ReadValueU16(endian);
                        }

                        if (compressionFlags != 0 &&
                            compressionFlags != -1)
                        {
                            throw new FormatException("bad compression flags");
                        }

                        this._Entries.Add(new Entry
                        {
                            Key = new ResourceKey(instanceId, typeId, groupId),
                            Offset = offset,
                            CompressedSize = compressedSize,
                            UncompressedSize = uncompressedSize,
                            CompressionFlags = compressionFlags,
                            Flags = flags,
                            IsValid = true
                        });
                    }

                    if (data.Position != data.Length)
                    {
                        throw new FormatException();
                    }
                }
            }
        }

        public void WriteHeader(Stream output, long indexOffset, long indexSize)
        {
            if (this.IsBig == false)
            {
                output.WriteValueU32(Header.Signature, Endian.Little);
                var header = new Header
                {
                    MajorVersion = this._Version.Major,
                    MinorVersion = this._Version.Minor,
                    IndexVersion = 3,
                    IndexCount = this._Entries.Count,
                    IndexOffset = (int)indexOffset,
                    IndexSize = (int)indexSize
                };
                output.WriteStructure(header);
            }
            else
            {
                output.WriteValueU32(BigHeader.Signature, Endian.Little);
                var header = new BigHeader
                {
                    MajorVersion = this._Version.Major,
                    MinorVersion = this._Version.Minor,
                    IndexVersion = 3,
                    IndexCount = this._Entries.Count,
                    IndexOffset = indexOffset,
                    IndexSize = indexSize
                };
                output.WriteStructure(header);
            }
        }

        public void WriteIndex(Stream output)
        {
            if (this._Entries.Count == 0)
            {
                output.WriteValueEnum<GlobalIndexValue>(GlobalIndexValue.None);
            }
            else
            {
                bool hasGlobalTypeId = true;
                bool hasGlobalGroupId = true;
                bool hasGlobalUpperInstanceId = true;

                var globalTypeId = this._Entries[0].Key.TypeId;
                var globalGroupId = this._Entries[0].Key.GroupId;
                var globalUpperInstanceId = (uint)(this._Entries[0].Key.InstanceId >> 32);

                for (int i = 1; i < this._Entries.Count; i++)
                {
                    hasGlobalTypeId = hasGlobalTypeId && globalTypeId == this._Entries[i].Key.TypeId;
                    hasGlobalGroupId = hasGlobalGroupId && globalGroupId == this._Entries[i].Key.GroupId;
                    hasGlobalUpperInstanceId = hasGlobalUpperInstanceId &&
                                               globalUpperInstanceId == (uint)(this._Entries[i].Key.InstanceId >> 32);

                    if (hasGlobalTypeId == false &&
                        hasGlobalGroupId == false &&
                        hasGlobalUpperInstanceId == false)
                    {
                        break;
                    }
                }

                var globalIndexValues = GlobalIndexValue.None;

                if (hasGlobalTypeId == true)
                {
                    globalIndexValues |= GlobalIndexValue.TypeId;
                }

                if (hasGlobalGroupId == true)
                {
                    globalIndexValues |= GlobalIndexValue.GroupId;
                }

                if (hasGlobalUpperInstanceId == true)
                {
                    globalIndexValues |= GlobalIndexValue.UpperInstanceId;
                }

                output.WriteValueEnum<GlobalIndexValue>(globalIndexValues);

                if (hasGlobalTypeId == true)
                {
                    output.WriteValueU32(globalTypeId);
                }

                if (hasGlobalGroupId == true)
                {
                    output.WriteValueU32(globalGroupId);
                }

                if (hasGlobalUpperInstanceId == true)
                {
                    output.WriteValueU32(globalUpperInstanceId);
                }

                foreach (var entry in this._Entries)
                {
                    if (hasGlobalTypeId == false)
                    {
                        output.WriteValueU32(entry.Key.TypeId);
                    }

                    if (hasGlobalGroupId == false)
                    {
                        output.WriteValueU32(entry.Key.GroupId);
                    }

                    if (hasGlobalUpperInstanceId == false)
                    {
                        output.WriteValueU32((uint)(entry.Key.InstanceId >> 32));
                    }

                    output.WriteValueU32((uint)(entry.Key.InstanceId & 0xFFFFFFFF));

                    if (this.IsBig == true)
                    {
                        output.WriteValueS64(entry.Offset);
                    }
                    else
                    {
                        output.WriteValueS32((int)entry.Offset);
                    }

                    output.WriteValueU32(entry.CompressedSize);
                    output.WriteValueU32(entry.UncompressedSize);
                    output.WriteValueS16(entry.CompressionFlags);
                    output.WriteValueU16(entry.Flags);
                }
            }
        }

        [Flags]
        public enum GlobalIndexValue : uint
        {
            None = 0,
            TypeId = 1 << 0,
            GroupId = 1 << 1,
            UpperInstanceId = 1 << 2,
            Invalid = ~(TypeId | GroupId | UpperInstanceId),
        }

        public struct Entry
        {
            public ResourceKey Key;

            #region public bool Compressed;
            public bool IsCompressed
            {
                get { return this.CompressionFlags == -1; }
            }
            #endregion

            public long Offset;
            public uint CompressedSize;
            public uint UncompressedSize;
            public short CompressionFlags;
            public ushort Flags;

            public bool IsValid;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct Header
        {
            public const uint Signature = 0x46504244; // 'DBPF'

            //public uint Magic; // 00
            public int MajorVersion; // 04
            public int MinorVersion; // 08
            public readonly uint Unknown0C; // 0C
            public readonly uint Unknown10; // 10
            public readonly uint Zero14; // 14 - Always 0?
            public readonly uint Zero18; // 18 - Always 0?
            public readonly uint Zero1C; // 1C - Always 0?
            public readonly uint Unknown20; // 20
            public int IndexCount; // 24
            public int OldIndexOffset; // 28
            public int IndexSize; // 2C
            public readonly uint Unset30; // 30
            public readonly uint Unset34; // 34
            public readonly uint Unset38; // 38
            public uint IndexVersion; // 3C - Always 3?
            public int IndexOffset; // 40
            public readonly uint Unset44; // 44
            public readonly uint Unset48; // 48
            public readonly uint Unset4C; // 4C
            public readonly uint Unset50; // 50
            public readonly uint Unset54; // 54
            public readonly uint Unset58; // 58
            public readonly uint Unset5C; // 5C
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct BigHeader
        {
            public const uint Signature = 0x46424244; // 'DBBF'

            //public uint Magic; // 00
            public int MajorVersion; // 04
            public int MinorVersion; // 08
            public readonly uint Unknown0C; // 0C
            public readonly uint Unknown10; // 10
            public readonly uint Zero14; // 14 - Always 0?
            public readonly uint Zero18; // 18 - Always 0?
            public readonly uint Zero1C; // 1C - Always 0?
            public readonly uint Unknown20; // 20
            public int IndexCount; // 24
            public long IndexSize; // 28
            public readonly uint Unset30; // 30
            public uint IndexVersion; // 34 - Always 3?
            public long IndexOffset; // 38
            public readonly uint Unset40; // 40
            public readonly uint Unset44; // 44
            public readonly uint Unset48; // 48
            public readonly uint Unset4C; // 4C
            public readonly uint Unset50; // 50
            public readonly uint Unset54; // 54
            public readonly uint Unset58; // 58
            public readonly uint Unset5C; // 5C
            public readonly uint Unset60; // 60
            public readonly uint Unset64; // 64
            public readonly uint Unset68; // 68
            public readonly uint Unset6C; // 6C
            public readonly uint Unset70; // 70
            public readonly uint Unset74; // 74
        }
    }
}
