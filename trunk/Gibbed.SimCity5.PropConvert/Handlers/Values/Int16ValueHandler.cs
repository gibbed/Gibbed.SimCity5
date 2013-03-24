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
using Gibbed.SimCity5.FileFormats.Variants.Values;

namespace Gibbed.SimCity5.PropConvert.Handlers.Values
{
    internal class Int16ValueHandler : SimpleValueHandler<Int16ValueVariant, short>
    {
        public override string Name
        {
            get { return "int16"; }
        }

        protected override void ExportValue(short value, XmlWriter writer)
        {
            writer.WriteValue(value.ToString(CultureInfo.InvariantCulture));
        }

        protected override void ImportValue(XPathNavigator nav, out short value)
        {
            if (short.TryParse(nav.Value,
                               NumberStyles.Integer,
                               CultureInfo.InvariantCulture,
                               out value) == false)
            {
                throw new FormatException();
            }
        }
    }
}
