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
    public class String16ArrayVariant : ArrayVariant<string>
    {
        public String16ArrayVariant()
            : this(default(IEnumerable<string>))
        {
        }

        public String16ArrayVariant(IEnumerable<string> value)
            : base(value)
        {
            this.Flags = VariantFlags.RequiresDeallocation |
                         VariantFlags.RequiresAllocation |
                         VariantFlags.Array |
                         VariantFlags.Unknown7;
        }

        public override VariantType Type
        {
            get { return VariantType.String16; }
        }

        internal override int MemorySize
        {
            get { return 16; }
        }

        internal override int FileSize
        {
            get { return -1; }
        }

        public static explicit operator String16ArrayVariant(List<string> value)
        {
            return new String16ArrayVariant(value);
        }

        public static explicit operator List<string>(String16ArrayVariant variant)
        {
            return variant.Value.ToList();
        }

        protected override void SerializeItem(string value, Stream output, Endian endian)
        {
            var encoding = endian == Endian.Big ? Encoding.BigEndianUnicode : Encoding.Unicode;
            var bytes = encoding.GetBytes(value ?? "");
            output.WriteValueS32(bytes.Length / 2, endian);
            output.WriteBytes(bytes);
        }

        protected override void DeserializeItem(out string value, Stream input, Endian endian)
        {
            var encoding = endian == Endian.Big ? Encoding.BigEndianUnicode : Encoding.Unicode;
            var length = input.ReadValueU32(endian);
            var bytes = input.ReadBytes(length * 2);
            value = encoding.GetString(bytes);
        }
    }
}
