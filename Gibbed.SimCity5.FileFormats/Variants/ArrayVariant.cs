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

namespace Gibbed.SimCity5.FileFormats.Variants
{
    public abstract class ArrayVariant<TType> : BaseVariant
    {
        private List<TType> _Value;

        public IEnumerable<TType> Value
        {
            get { return this._Value == null ? null : this._Value.ToList(); }
            set { this._Value = value == null ? null : value.ToList(); }
        }

        internal override VariantFlags RequiredFlags
        {
            get { return VariantFlags.Array; }
        }

        internal override VariantFlags ValidFlags
        {
            get
            {
                return VariantFlags.RequiresDeallocation | VariantFlags.RequiresAllocation | VariantFlags.Array |
                       VariantFlags.Unknown7;
            }
        }

        internal ArrayVariant(IEnumerable<TType> value)
        {
            this.Value = value;
        }

        internal override sealed void Serialize(Stream output, Endian endian)
        {
            var itemCount = this._Value == null ? 0 : this._Value.Count;
            output.WriteValueS32(itemCount, endian);
            output.WriteValueS32(this.MemorySize, endian);

            if (this._Value != null &&
                this._Value.Count > 0)
            {
                var itemFileSize = this.FileSize;
                if (itemFileSize != -1)
                {
                    var itemBytes = new byte[itemCount * (uint)itemFileSize];
                    using (var data = new MemoryStream(itemBytes, true))
                    {
                        foreach (var item in this._Value)
                        {
                            this.SerializeItem(item, data, endian);
                        }

                        if (data.Position != data.Length)
                        {
                            throw new FormatException();
                        }
                    }
                    output.WriteBytes(itemBytes);
                }
                else
                {
                    foreach (var item in this._Value)
                    {
                        this.SerializeItem(item, output, endian);
                    }
                }
            }
        }

        protected abstract void SerializeItem(TType value, Stream input, Endian endian);

        internal override sealed void Deserialize(Stream input, Endian endian)
        {
            var itemCount = input.ReadValueU32(endian);
            var items = new TType[itemCount];

            var itemMemorySize = input.ReadValueS32(endian);
            if (itemMemorySize != this.MemorySize)
            {
                throw new FormatException();
            }

            var itemFileSize = this.FileSize;
            if (itemFileSize != -1)
            {
                var itemBytes = input.ReadBytes(itemCount * (uint)itemFileSize);
                using (var data = new MemoryStream(itemBytes, false))
                {
                    for (uint i = 0; i < itemCount; i++)
                    {
                        this.DeserializeItem(out items[i], data, endian);
                    }

                    if (data.Position != data.Length)
                    {
                        throw new FormatException();
                    }
                }
            }
            else
            {
                for (uint i = 0; i < itemCount; i++)
                {
                    this.DeserializeItem(out items[i], input, endian);
                }
            }

            this.Value = items;
        }

        protected abstract void DeserializeItem(out TType value, Stream input, Endian endian);
    }
}
