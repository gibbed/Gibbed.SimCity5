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
using System.Text;
using Gibbed.IO;

namespace Gibbed.SimCity5.FileFormats.Variants.Arrays
{
    public sealed class String8ArrayVariant : ArrayVariant<string>
    {
        public String8ArrayVariant()
            : this(default(IEnumerable<string>))
        {
        }

        public String8ArrayVariant(IEnumerable<string> value)
            : base(value)
        {
            this.Flags = VariantFlags.RequiresDeallocation |
                         VariantFlags.RequiresAllocation |
                         VariantFlags.Array |
                         VariantFlags.Unknown7;
        }

        public override VariantType Type
        {
            get { return VariantType.String8; }
        }

        internal override int MemorySize
        {
            get { return 16; }
        }

        internal override int FileSize
        {
            get { return -1; }
        }

        public bool IsObfuscated
        {
            get { return (this.Flags & VariantFlags.Obfuscated) != VariantFlags.None; }

            set
            {
                if (value == true)
                {
                    this.Flags |= VariantFlags.Obfuscated;
                }
                else
                {
                    this.Flags &= ~VariantFlags.Obfuscated;
                }
            }
        }

        public static explicit operator String8ArrayVariant(List<string> value)
        {
            return new String8ArrayVariant(value);
        }

        public static explicit operator List<string>(String8ArrayVariant variant)
        {
            return variant.Value.ToList();
        }

        protected override void SerializeItem(string value, Stream output, Endian endian)
        {
            if (this.IsObfuscated == false)
            {
                var bytes = Encoding.ASCII.GetBytes(value ?? "");
                output.WriteValueS32(bytes.Length, endian);
                output.WriteBytes(bytes);
            }
            else
            {
                var bytes = Encoding.ASCII.GetBytes(value ?? "");
                Values.String8ValueVariant.Bogocrypt(bytes);

                output.WriteValueU8(0);
                output.WriteValueS32(bytes.Length, endian);
                output.WriteBytes(bytes);
            }
        }

        protected override void DeserializeItem(out string value, Stream input, Endian endian)
        {
            if (this.IsObfuscated == false)
            {
                var length = input.ReadValueU32(endian);
                var bytes = input.ReadBytes(length);
                value = Encoding.ASCII.GetString(bytes);
            }
            else
            {
                var dummy = input.ReadValueU8();
                if (dummy != 0)
                {
                    throw new FormatException();
                }

                var length = input.ReadValueU32(endian);
                var bytes = input.ReadBytes(length);
                Values.String8ValueVariant.Bogocrypt(bytes);
                value = Encoding.ASCII.GetString(bytes);
            }
        }
    }
}
