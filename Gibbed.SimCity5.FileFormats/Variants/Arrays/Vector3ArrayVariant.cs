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
    public class Vector3ArrayVariant : ArrayVariant<Vector3>
    {
        public Vector3ArrayVariant()
            : this(default(IEnumerable<Vector3>))
        {
        }

        public Vector3ArrayVariant(IEnumerable<Vector3> value)
            : base(value)
        {
            this.Flags = VariantFlags.RequiresDeallocation |
                         VariantFlags.RequiresAllocation |
                         VariantFlags.Array |
                         VariantFlags.Unknown7;
        }

        public override VariantType Type
        {
            get { return VariantType.Vector3; }
        }

        internal override int MemorySize
        {
            get { return 12; }
        }

        public static explicit operator Vector3ArrayVariant(List<Vector3> value)
        {
            return new Vector3ArrayVariant(value);
        }

        public static explicit operator List<Vector3>(Vector3ArrayVariant variant)
        {
            return variant.Value.ToList();
        }

        protected override void SerializeItem(Vector3 value, Stream output, Endian endian)
        {
            value.Serialize(output, endian);
        }

        protected override void DeserializeItem(out Vector3 value, Stream input, Endian endian)
        {
            value = Vector3.Read(input, endian);
        }
    }
}
