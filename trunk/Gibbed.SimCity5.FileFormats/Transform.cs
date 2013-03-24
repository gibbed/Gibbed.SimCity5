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
using Gibbed.IO;

namespace Gibbed.SimCity5.FileFormats
{
    public struct Transform
    {
        [Flags]
        public enum TransformFlags : ushort
        {
            None = 0,
            HasScale = 1 << 0,
            HasRotation = 1 << 1,
            HasTranslation = 1 << 2,
            IsCompressed = 1 << 3,
            ValidFlags = HasScale | HasRotation | HasTranslation | IsCompressed,
        }

        public TransformFlags Flags { get; set; }
        public Vector3 Translation { get; set; }
        public float Scale { get; set; }
        public Matrix3 Rotation { get; set; }

        public void Serialize(Stream output, Endian endian)
        {
            output.WriteValueEnum<TransformFlags>(this.Flags, endian);

            if ((this.Flags & TransformFlags.HasScale) != TransformFlags.None)
            {
                output.WriteValueF32(this.Scale, endian);
            }

            if ((this.Flags & TransformFlags.HasRotation) != TransformFlags.None)
            {
                this.Rotation.Serialize(output, endian);
            }

            if ((this.Flags & TransformFlags.HasTranslation) != TransformFlags.None)
            {
                this.Translation.Serialize(output, endian);
            }
        }

        public void Deserialize(Stream input, Endian endian)
        {
            this.Flags = input.ReadValueEnum<TransformFlags>(endian);
            if ((this.Flags & ~(TransformFlags.ValidFlags)) != 0)
            {
                throw new FormatException();
            }

            this.Scale = ((this.Flags & TransformFlags.HasScale) != 0)
                             ? input.ReadValueS32(endian)
                             : 1.0f;
            this.Rotation = ((this.Flags & TransformFlags.HasRotation) != 0)
                                ? Matrix3.Read(input, endian)
                                : Matrix3.Identity;
            this.Translation = ((this.Flags & TransformFlags.HasTranslation) != 0)
                                   ? Vector3.Read(input, endian)
                                   : Vector3.Zero;
        }

        public static Transform Read(Stream input, Endian endian)
        {
            var value = new Transform();
            value.Deserialize(input, endian);
            return value;
        }
    }
}
