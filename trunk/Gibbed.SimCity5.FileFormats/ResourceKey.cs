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

namespace Gibbed.SimCity5.FileFormats
{
    public struct ResourceKey
    {
        public readonly uint TypeId;
        public readonly uint GroupId;
        public readonly ulong InstanceId;

        public ResourceKey(ulong instanceId, uint typeId, uint groupId)
        {
            this.InstanceId = instanceId;
            this.TypeId = typeId;
            this.GroupId = groupId;
        }

        public ResourceKey(ulong instanceId, uint typeId)
            : this(instanceId, typeId, 0)
        {
        }

        public ResourceKey(string instance, uint typeId, uint groupId)
            : this(instance.HashFNV64(), typeId, groupId)
        {
        }

        public ResourceKey(string instance, uint typeId)
            : this(instance, typeId, 0)
        {
        }

        public string ToPath()
        {
            return string.Format("{0:X8}-{1:X8}-{2:X16}",
                                 this.TypeId,
                                 this.GroupId,
                                 this.InstanceId);
        }

        public override string ToString()
        {
            return string.Format("{0:X8}:{1:X8}:{2:X16}",
                                 this.TypeId,
                                 this.GroupId,
                                 this.InstanceId);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || obj.GetType() != this.GetType())
            {
                return false;
            }

            return (ResourceKey)obj == this;
        }

        public static bool operator !=(ResourceKey a, ResourceKey b)
        {
            return
                a.TypeId != b.TypeId ||
                a.GroupId != b.GroupId ||
                a.InstanceId != b.InstanceId;
        }

        public static bool operator ==(ResourceKey a, ResourceKey b)
        {
            return
                a.TypeId == b.TypeId &&
                a.GroupId == b.GroupId &&
                a.InstanceId == b.InstanceId;
        }

        public override int GetHashCode()
        {
            return this.InstanceId.GetHashCode() ^ ((int)(this.TypeId ^ (this.GroupId << 16)));
        }
    }
}
