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

using System.Xml;
using System.Xml.XPath;
using Gibbed.SimCity5.FileFormats.Variants.Values;

namespace Gibbed.SimCity5.PropConvert.Handlers.Values
{
    internal class String16ValueHandler : SimpleValueHandler<String16ValueVariant, string>
    {
        public override string Name
        {
            get { return "string16"; }
        }

        protected override void ExportValue(string value, XmlWriter writer)
        {
            writer.WriteValue(value);
        }

        protected override void ImportValue(XPathNavigator nav, out string value)
        {
            value = nav.Value;
        }
    }
}
