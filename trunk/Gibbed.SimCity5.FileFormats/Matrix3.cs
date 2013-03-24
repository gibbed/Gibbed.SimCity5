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

namespace Gibbed.SimCity5.FileFormats
{
    public struct Matrix3
    {
        public static readonly Matrix3 Identity = new Matrix3(new Vector3(1.0f, 0.0f, 0.0f),
                                                              new Vector3(0.0f, 1.0f, 0.0f),
                                                              new Vector3(0.0f, 0.0f, 1.0f));

        public Vector3 X { get; set; }
        public Vector3 Y { get; set; }
        public Vector3 Z { get; set; }

        public Matrix3(Vector3 x, Vector3 y, Vector3 z)
            : this()
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        public void Serialize(Stream output, Endian endian)
        {
            this.X.Serialize(output, endian);
            this.Y.Serialize(output, endian);
            this.Z.Serialize(output, endian);
        }

        public void Deserialize(Stream input, Endian endian)
        {
            this.X = Vector3.Read(input, endian);
            this.Y = Vector3.Read(input, endian);
            this.Z = Vector3.Read(input, endian);
        }

        public static Matrix3 Read(Stream input, Endian endian)
        {
            var value = new Matrix3();
            value.Deserialize(input, endian);
            return value;
        }
    }
}
