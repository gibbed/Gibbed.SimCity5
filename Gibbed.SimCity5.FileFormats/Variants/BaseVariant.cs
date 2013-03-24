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

namespace Gibbed.SimCity5.FileFormats.Variants
{
    public abstract class BaseVariant
    {
        private VariantFlags _Flags;

        internal BaseVariant()
        {
        }

        public abstract VariantType Type { get; }

        internal VariantFlags Flags
        {
            get { return this._Flags; }
            set
            {
                if (this.RequiredFlags != VariantFlags.None &&
                    (value & this.RequiredFlags) != this.RequiredFlags)
                {
                    throw new ArgumentException();
                }

                if ((value & ~this.ValidFlags) != VariantFlags.None)
                {
                    throw new ArgumentException();
                }

                this._Flags = value;
            }
        }

        internal virtual VariantFlags RequiredFlags
        {
            get { return VariantFlags.None; }
        }

        internal virtual VariantFlags ValidFlags
        {
            get { return VariantFlags.None; }
        }

        internal abstract int MemorySize { get; }

        internal virtual int FileSize
        {
            get { return this.MemorySize; }
        }

        internal abstract void Serialize(Stream output, Endian endian);
        internal abstract void Deserialize(Stream input, Endian endian);
    }
}
