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
using Gibbed.IO;
using Gibbed.SimCity5.FileFormats.Variants;

namespace Gibbed.SimCity5.FileFormats
{
    public class PropertyListFile
    {
        private readonly Dictionary<uint, BaseVariant> _Properties
            = new Dictionary<uint, BaseVariant>();

        public IEnumerable<uint> Keys
        {
            get { return this._Properties.Keys; }
        }

        public void Serialize(Stream output)
        {
            const Endian endian = Endian.Big;

            output.WriteValueS32(this._Properties.Count, endian);
            foreach (var kv in this._Properties)
            {
                var key = kv.Key;
                var variant = kv.Value;

                output.WriteValueU32(key, endian);
                output.WriteValueEnum<VariantType>(variant.Type, endian);

                var flags = variant.Flags;
                output.WriteValueEnum<VariantFlags>(flags | VariantFlags.Unknown15, endian);

                if ((flags & ~VariantFlags.ValidFlags) != 0)
                {
                    throw new FormatException();
                }

                variant.Serialize(output, endian);
            }
        }

        public void Deserialize(Stream input)
        {
            const Endian endian = Endian.Big;

            var count = input.ReadValueU32(endian);
            this._Properties.Clear();
            for (uint i = 0; i < count; i++)
            {
                var key = input.ReadValueU32(endian);
                var type = input.ReadValueEnum<VariantType>(endian);
                var flags = input.ReadValueEnum<VariantFlags>(endian);
                var origFlags = flags;

                if ((flags & ~VariantFlags.ValidFlags) != 0)
                {
                    throw new FormatException();
                }

                flags &= ~VariantFlags.Unknown15;

                BaseVariant variant;

                if ((flags & VariantFlags.Array) == 0)
                {
                    variant = ValueVariantFactory.Create(type);
                }
                else
                {
                    variant = ArrayVariantFactory.Create(type);
                }

                variant.Flags = flags;
                variant.Deserialize(input, endian);

                this._Properties.Add(key, variant);
            }
        }

        public BaseVariant this[uint key]
        {
            get
            {
                if (this._Properties.ContainsKey(key) == false)
                {
                    throw new KeyNotFoundException();
                }

                return this._Properties[key];
            }

            set
            {
                if (value == null)
                {
                    this._Properties.Remove(key);
                }
                else
                {
                    this._Properties[key] = value;
                }
            }
        }
    }
}
