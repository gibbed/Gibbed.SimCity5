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
    public class String8ValueVariant : ValueVariant<string>
    {
        public String8ValueVariant()
            : this(default(string))
        {
        }

        public String8ValueVariant(string value)
            : base(value)
        {
        }

        public override VariantType Type
        {
            get { return VariantType.String8; }
        }

        internal override VariantFlags ValidFlags
        {
            get
            {
                return VariantFlags.Unknown1 | VariantFlags.RequiresDeallocation | VariantFlags.RequiresAllocation |
                       VariantFlags.Obfuscated;
            }
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

        public static explicit operator String8ValueVariant(string value)
        {
            return new String8ValueVariant(value);
        }

        public static explicit operator string(String8ValueVariant variant)
        {
            return variant.Value;
        }

        internal override void Serialize(Stream output, Endian endian)
        {
            if (this.IsObfuscated == false)
            {
                var bytes = Encoding.ASCII.GetBytes(this.Value ?? "");
                output.WriteValueS32(bytes.Length, endian);
                output.WriteBytes(bytes);
            }
            else
            {
                var bytes = Encoding.ASCII.GetBytes(this.Value ?? "");
                Bogocrypt(bytes);

                output.WriteValueU8(0);
                output.WriteValueS32(bytes.Length, endian);
                output.WriteBytes(bytes);
            }
        }

        internal override void Deserialize(Stream input, Endian endian)
        {
            if (this.IsObfuscated == false)
            {
                var length = input.ReadValueU32(endian);
                var bytes = input.ReadBytes(length);
                this.Value = Encoding.ASCII.GetString(bytes);
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
                Bogocrypt(bytes);
                this.Value = Encoding.ASCII.GetString(bytes);
            }
        }

        private static readonly byte[] _BogocryptKey = new byte[]
        {
            0x78, 0x56, 0x34, 0x12, 0xAB, 0x34, 0x74, 0x77,
            0x24, 0x55, 0xA6, 0xB5, 0x37, 0x88, 0x91, 0x67,
        };

        internal static void Bogocrypt(byte[] bytes)
        {
            int keyLength = _BogocryptKey.Length;
            for (int i = 0; i < bytes.Length; i++)
            {
                bytes[i] ^= _BogocryptKey[i % keyLength];
            }
        }
    }
}
