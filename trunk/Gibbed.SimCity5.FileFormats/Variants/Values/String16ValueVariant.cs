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
using System.IO;
using System.Text;
using Gibbed.IO;

namespace Gibbed.SimCity5.FileFormats.Variants.Values
{
    public class String16ValueVariant : ValueVariant<string>
    {
        public String16ValueVariant()
            : this(default(string))
        {
        }

        public String16ValueVariant(string value)
            : base(value)
        {
        }

        public override VariantType Type
        {
            get { return VariantType.String16; }
        }

        internal override VariantFlags ValidFlags
        {
            get { return VariantFlags.Unknown1 | VariantFlags.RequiresDeallocation | VariantFlags.RequiresAllocation; }
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

        public static explicit operator String16ValueVariant(string value)
        {
            return new String16ValueVariant(value);
        }

        public static explicit operator string(String16ValueVariant variant)
        {
            return variant.Value;
        }

        internal override void Serialize(Stream output, Endian endian)
        {
            var encoding = endian == Endian.Big ? Encoding.BigEndianUnicode : Encoding.Unicode;
            var bytes = encoding.GetBytes(this.Value ?? "");
            output.WriteValueS32(bytes.Length / 2, endian);
            output.WriteBytes(bytes);
        }

        internal override void Deserialize(Stream input, Endian endian)
        {
            var encoding = endian == Endian.Big ? Encoding.BigEndianUnicode : Encoding.Unicode;
            var length = input.ReadValueU32(endian);
            var bytes = input.ReadBytes(length * 2);
            this.Value = encoding.GetString(bytes);
        }
    }
}
