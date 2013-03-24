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
using System.Globalization;
using System.Xml;
using System.Xml.XPath;
using Gibbed.SimCity5.FileFormats;
using Gibbed.SimCity5.FileFormats.Variants.Arrays;

namespace Gibbed.SimCity5.PropConvert.Handlers.Arrays
{
    internal class TransformArrayHandler : SimpleArrayHandler<TransformArrayVariant, Transform>
    {
        public override string Name
        {
            get { return "transforms"; }
        }

        protected override string ItemName
        {
            get { return "transform"; }
        }

        protected override void ExportItem(Transform value, XmlWriter writer)
        {
            writer.WriteElementString("flags", value.Flags.ToString());
            writer.WriteElementString("translation",
                                      string.Format("{0},{1},{2}",
                                                    value.Translation.X.ToString(CultureInfo.InvariantCulture),
                                                    value.Translation.Y.ToString(CultureInfo.InvariantCulture),
                                                    value.Translation.Z.ToString(CultureInfo.InvariantCulture)));
            writer.WriteElementString("scale", value.Scale.ToString(CultureInfo.InvariantCulture));
            writer.WriteElementString("rotation",
                                      string.Format("{0},{1},{2}, {3},{4},{5}, {6},{7},{8}",
                                                    value.Rotation.X.X.ToString(CultureInfo.InvariantCulture),
                                                    value.Rotation.X.Y.ToString(CultureInfo.InvariantCulture),
                                                    value.Rotation.X.Z.ToString(CultureInfo.InvariantCulture),
                                                    value.Rotation.Y.X.ToString(CultureInfo.InvariantCulture),
                                                    value.Rotation.Y.Y.ToString(CultureInfo.InvariantCulture),
                                                    value.Rotation.Y.Z.ToString(CultureInfo.InvariantCulture),
                                                    value.Rotation.Z.X.ToString(CultureInfo.InvariantCulture),
                                                    value.Rotation.Z.Y.ToString(CultureInfo.InvariantCulture),
                                                    value.Rotation.Z.Z.ToString(CultureInfo.InvariantCulture)));
        }

        protected override void ImportItem(XPathNavigator nav, out Transform value)
        {
            throw new NotImplementedException();
        }
    }
}
