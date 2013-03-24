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
using Gibbed.IO;
using Gibbed.SimCity5.FileFormats.Variants;

namespace Gibbed.SimCity5.FileFormats
{
    public class PropertyListFile
    {
        private readonly Dictionary<uint, BaseVariant> _Properties
            = new Dictionary<uint, BaseVariant>();

        public IEnumerable<uint> Ids
        {
            get { return this._Properties.Keys; }
        }

        public void Serialize(Stream output)
        {
            const Endian endian = Endian.Big;

            output.WriteValueS32(this._Properties.Count, endian);
            foreach (var kv in this._Properties.OrderBy(kv => kv.Key))
            {
                var id = kv.Key;
                var variant = kv.Value;

                output.WriteValueU32(id, endian);
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
                var id = input.ReadValueU32(endian);
                var type = input.ReadValueEnum<VariantType>(endian);
                var flags = input.ReadValueEnum<VariantFlags>(endian);
                var origFlags = flags;

                if ((flags & ~VariantFlags.ValidFlags) != 0)
                {
                    throw new FormatException();
                }

                flags &= ~VariantFlags.Unknown15;

                var variant = (flags & VariantFlags.Array) == VariantFlags.None
                                  ? ValueVariantFactory.Create(type)
                                  : ArrayVariantFactory.Create(type);

                variant.Flags = flags;
                variant.Deserialize(input, endian);

                this._Properties.Add(id, variant);
            }
        }

        public BaseVariant this[uint id]
        {
            get
            {
                if (this._Properties.ContainsKey(id) == false)
                {
                    throw new KeyNotFoundException();
                }

                return this._Properties[id];
            }

            set
            {
                if (value == null)
                {
                    this._Properties.Remove(id);
                }
                else
                {
                    this._Properties[id] = value;
                }
            }
        }
    }
}
