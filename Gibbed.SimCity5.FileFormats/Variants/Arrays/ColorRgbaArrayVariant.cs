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

using System.Collections.Generic;
using System.IO;
using System.Linq;
using Gibbed.IO;

namespace Gibbed.SimCity5.FileFormats.Variants.Arrays
{
    public class ColorRgbaArrayVariant : ArrayVariant<ColorRgba>
    {
        public ColorRgbaArrayVariant()
            : this(default(IEnumerable<ColorRgba>))
        {
        }

        public ColorRgbaArrayVariant(IEnumerable<ColorRgba> value)
            : base(value)
        {
            this.Flags = VariantFlags.RequiresDeallocation |
                         VariantFlags.RequiresAllocation |
                         VariantFlags.Array |
                         VariantFlags.Unknown7;
        }

        public override VariantType Type
        {
            get { return VariantType.ColorRgba; }
        }

        internal override int MemorySize
        {
            get { return 16; }
        }

        public static explicit operator ColorRgbaArrayVariant(List<ColorRgba> value)
        {
            return new ColorRgbaArrayVariant(value);
        }

        public static explicit operator List<ColorRgba>(ColorRgbaArrayVariant variant)
        {
            return variant.Value.ToList();
        }

        protected override void SerializeItem(ColorRgba value, Stream output, Endian endian)
        {
            value.Serialize(output, endian);
        }

        protected override void DeserializeItem(out ColorRgba value, Stream input, Endian endian)
        {
            value = ColorRgba.Read(input, endian);
        }
    }
}
