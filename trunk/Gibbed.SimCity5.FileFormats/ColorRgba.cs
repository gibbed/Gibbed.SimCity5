﻿/* Copyright (c) 2013 Rick (rick 'at' gibbed 'dot' us)
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

namespace Gibbed.SimCity5.FileFormats
{
    public struct ColorRgba
    {
        public float R { get; set; }
        public float G { get; set; }
        public float B { get; set; }
        public float A { get; set; }

        public void Serialize(Stream output, Endian endian)
        {
            output.WriteValueF32(this.R, endian);
            output.WriteValueF32(this.G, endian);
            output.WriteValueF32(this.B, endian);
            output.WriteValueF32(this.A, endian);
        }

        public void Deserialize(Stream input, Endian endian)
        {
            this.R = input.ReadValueF32(endian);
            this.G = input.ReadValueF32(endian);
            this.B = input.ReadValueF32(endian);
            this.A = input.ReadValueF32(endian);
        }

        public static ColorRgba Read(Stream input, Endian endian)
        {
            var value = new ColorRgba();
            value.Deserialize(input, endian);
            return value;
        }
    }
}
