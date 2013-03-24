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
using System.Xml;
using System.Xml.XPath;
using Gibbed.SimCity5.FileFormats;
using Gibbed.SimCity5.FileFormats.Variants.Values;

namespace Gibbed.SimCity5.PropConvert.Handlers.Values
{
    internal class ResourceKeyValueHandler : SimpleValueHandler<ResourceKeyValueVariant, ResourceKey>
    {
        public override string Name
        {
            get { return "key"; }
        }

        protected override void ExportValue(ResourceKey value, XmlWriter writer)
        {
            var groupId = value.GroupId;
            var instanceId = value.InstanceId;
            var typeId = value.TypeId;
            
            if (groupId != 0)
            {
                writer.WriteElementString("group", "0x" + groupId.ToString("X8"));
            }

            writer.WriteElementString("instance", "0x" + instanceId.ToString("X8"));

            if (typeId != 0)
            {
                writer.WriteElementString("type", "0x" + typeId.ToString("X8"));
            }
        }

        protected override void ImportValue(XPathNavigator nav, out ResourceKey value)
        {
            var groupNode = nav.SelectSingleNode("group");
            var instanceNode = nav.SelectSingleNode("instance");
            var typeNode = nav.SelectSingleNode("type");

            uint groupId;
            if (groupNode == null ||
                string.IsNullOrEmpty(groupNode.Value) == true)
            {
                groupId = 0;
            }
            else
            {
                if (TryParseHexUInt32(groupNode.Value, out groupId) == false)
                {
                    throw new FormatException();
                }
            }

            uint instanceId;
            if (instanceNode == null ||
                string.IsNullOrEmpty(instanceNode.Value) == true)
            {
                throw new Exception("instanceid cannot be null for key");
            }
            else
            {
                if (TryParseHexUInt32(instanceNode.Value, out instanceId) == false)
                {
                    throw new FormatException();
                }
            }

            uint typeId;
            if (typeNode == null ||
                string.IsNullOrEmpty(typeNode.Value) == true)
            {
                typeId = 0;
            }
            else
            {
                if (TryParseHexUInt32(typeNode.Value, out typeId) == false)
                {
                    throw new FormatException();
                }
            }

            value = new ResourceKey(instanceId, typeId, groupId);
        }
    }
}
