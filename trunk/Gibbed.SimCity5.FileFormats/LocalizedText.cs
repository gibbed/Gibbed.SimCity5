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
    public struct LocalizedText
    {
        public static readonly LocalizedText Zero = new LocalizedText(0, 0);

        public uint TableId { get; set; }
        public uint InstanceId { get; set; }

        public LocalizedText(uint tableId, uint instanceId)
            : this()
        {
            this.TableId = tableId;
            this.InstanceId = instanceId;
        }

        public void Serialize(Stream output, Endian endian)
        {
            output.WriteValueF32(this.TableId, endian);
            output.WriteValueF32(this.InstanceId, endian);
        }

        public void Deserialize(Stream input, Endian endian)
        {
            this.TableId = input.ReadValueU32(endian);
            this.InstanceId = input.ReadValueU32(endian);
        }

        public static LocalizedText Read(Stream input, Endian endian)
        {
            var value = new LocalizedText();
            value.Deserialize(input, endian);
            return value;
        }
    }
}
