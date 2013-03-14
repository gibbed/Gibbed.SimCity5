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
using System.Globalization;
using System.IO;
using System.Xml.XPath;
using Gibbed.IO;
using Gibbed.RefPack;
using Gibbed.SimCity5.FileFormats;
using NDesk.Options;

namespace Gibbed.SimCity5.Pack
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            bool showHelp = false;
            bool verbose = false;
            bool shouldCompress = false;

            var options = new OptionSet()
            {
                { "c|compress", "enable compression", v => shouldCompress = v != null },
                { "v|verbose", "be verbose", v => verbose = v != null },
                { "h|help", "show this message and exit", v => showHelp = v != null },
            };

            List<string> extras;

            try
            {
                extras = options.Parse(args);
            }
            catch (OptionException e)
            {
                Console.Write("{0}: ", GetExecutableName());
                Console.WriteLine(e.Message);
                Console.WriteLine("Try `{0} --help' for more information.", GetExecutableName());
                return;
            }

            if (extras.Count < 1 || extras.Count > 2 || showHelp == true)
            {
                Console.WriteLine("Usage: {0} [OPTIONS]+ input_dir [output_package]", GetExecutableName());
                Console.WriteLine();
                Console.WriteLine("Options:");
                options.WriteOptionDescriptions(Console.Out);
                return;
            }

            string filesPath = extras[0];
            string filesBasePath;

            if (Directory.Exists(filesPath) == true)
            {
                filesBasePath = filesPath;
                filesPath = Path.Combine(filesBasePath, "files.xml");
            }
            else
            {
                filesBasePath = Path.GetDirectoryName(filesPath);
                filesBasePath = filesBasePath ?? "";
            }

            string outputPath = extras.Count > 1
                                    ? extras[1]
                                    : Path.ChangeExtension(filesBasePath, ".package");

            var document = new XPathDocument(filesPath);
            var navigator = document.CreateNavigator();
            var nodes = navigator.Select("/files/file");

            var filePaths = new Dictionary<ResourceKey, string>();

            if (verbose == true)
            {
                Console.WriteLine("Discovering files...");
            }

            while (nodes.MoveNext())
            {
                var groupText = nodes.Current.GetAttribute("group", "");
                var instanceText = nodes.Current.GetAttribute("instance", "");
                var typeText = nodes.Current.GetAttribute("type", "");

                if (groupText == null ||
                    instanceText == null ||
                    typeText == null)
                {
                    throw new InvalidDataException("file missing attributes");
                }

                uint groupId;
                ulong instanceId;
                uint typeId;
                if (uint.TryParse(groupText, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out groupId) == false ||
                    ulong.TryParse(instanceText, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out instanceId) ==
                    false ||
                    uint.TryParse(typeText, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out typeId) == false)
                {
                    Console.WriteLine("Failed to parse resource key [{0}, {1}, {2}]!",
                                      groupText,
                                      instanceText,
                                      typeText);
                    continue;
                }

                var key = new ResourceKey(instanceId, typeId, groupId);

                string inputPath;
                if (Path.IsPathRooted(nodes.Current.Value) == false)
                {
                    // relative path, it should be relative to the XML file
                    inputPath = Path.Combine(filesBasePath, nodes.Current.Value);
                }
                else
                {
                    inputPath = nodes.Current.Value;
                }

                if (File.Exists(inputPath) == false)
                {
                    Console.WriteLine(inputPath + " does not exist!");
                    continue;
                }

                filePaths.Add(key, inputPath);
            }

            if (verbose == true)
            {
                Console.WriteLine("Writing files...");
            }

            using (var output = File.Create(outputPath))
            {
                var dbpf = new DatabasePackedFile
                {
                    IsBig = false,
                    Version = new Version(3, 0),
                };

                dbpf.WriteHeader(output, 0, 0);
                foreach (var kv in filePaths)
                {
                    var key = kv.Key;
                    var filePath = kv.Value;

                    if (verbose == true)
                    {
                        Console.WriteLine("{0}", filePath);
                    }

                    using (var input = File.OpenRead(filePath))
                    {
                        if (shouldCompress == false)
                        {
                            long offset = output.Position;
                            output.WriteFromStream(input, (uint)input.Length);

                            dbpf.Entries.Add(new DatabasePackedFile.Entry
                            {
                                Key = key,
                                CompressedSize = (uint)input.Length | 0x80000000,
                                UncompressedSize = (uint)input.Length,
                                CompressionFlags = 0,
                                Flags = 1,
                                Offset = offset,
                            });
                        }
                        else
                        {
                            byte[] compressed;
                            var success = input.RefPackCompress((int)input.Length, out compressed);

                            if (success == true)
                            {
                                long offset = output.Position;
                                output.WriteBytes(compressed);

                                dbpf.Entries.Add(new DatabasePackedFile.Entry
                                {
                                    Key = key,
                                    CompressedSize = (uint)(compressed.Length) | 0x80000000,
                                    UncompressedSize = (uint)input.Length,
                                    CompressionFlags = -1,
                                    Flags = 1,
                                    Offset = offset,
                                });
                            }
                            else
                            {
                                input.Position = 0;
                                long offset = output.Position;
                                output.WriteFromStream(input, (uint)input.Length);

                                dbpf.Entries.Add(new DatabasePackedFile.Entry
                                {
                                    Key = key,
                                    CompressedSize = (uint)input.Length | 0x80000000,
                                    UncompressedSize = (uint)input.Length,
                                    CompressionFlags = 0,
                                    Flags = 1,
                                    Offset = offset,
                                });
                            }
                        }
                    }
                }

                var endOfData = output.Position;

                dbpf.WriteIndex(output);

                long indexSize = output.Position - endOfData;

                output.Position = 0;
                dbpf.WriteHeader(output, endOfData, indexSize);
            }
        }

        private static string GetExecutableName()
        {
            return Path.GetFileName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase);
        }
    }
}
