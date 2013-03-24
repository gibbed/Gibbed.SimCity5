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

using System.IO;
using Gibbed.IO;

namespace Gibbed.SimCity5.FileFormats.Variants.Values
{
    public class Vector3ValueVariant : ValueVariant<Vector3>
    {
        public Vector3ValueVariant()
            : this(default(Vector3))
        {
        }

        public Vector3ValueVariant(Vector3 value)
            : base(value)
        {
        }

        public override VariantType Type
        {
            get { return VariantType.Vector3; }
        }

        internal override int MemorySize
        {
            get { return 12; }
        }

        public static explicit operator Vector3ValueVariant(Vector3 value)
        {
            return new Vector3ValueVariant(value);
        }

        public static explicit operator Vector3(Vector3ValueVariant variant)
        {
            return variant.Value;
        }

        internal override void Serialize(Stream output, Endian endian)
        {
            this.Value.Serialize(output, endian);
        }

        internal override void Deserialize(Stream input, Endian endian)
        {
            this.Value = Vector3.Read(input, endian);
        }
    }
}
