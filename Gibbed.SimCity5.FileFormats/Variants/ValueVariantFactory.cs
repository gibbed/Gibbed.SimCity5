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
using Gibbed.SimCity5.FileFormats.Variants.Values;

namespace Gibbed.SimCity5.FileFormats.Variants
{
    internal static class ValueVariantFactory
    {
        public static BaseVariant Create(VariantType type)
        {
            switch (type)
            {
                case VariantType.Bool:
                {
                    return new BoolValueVariant();
                }

                case VariantType.Char8:
                {
                    return new Char8ValueVariant();
                }

                case VariantType.Char16:
                {
                    return new Char16ValueVariant();
                }

                case VariantType.Int8:
                {
                    return new Int8ValueVariant();
                }

                case VariantType.UInt8:
                {
                    return new UInt8ValueVariant();
                }

                case VariantType.Int16:
                {
                    return new Int16ValueVariant();
                }

                case VariantType.UInt16:
                {
                    return new UInt16ValueVariant();
                }

                case VariantType.Int32:
                {
                    return new Int32ValueVariant();
                }

                case VariantType.UInt32:
                {
                    return new UInt32ValueVariant();
                }

                case VariantType.Int64:
                {
                    return new Int64ValueVariant();
                }

                case VariantType.UInt64:
                {
                    return new UInt64ValueVariant();
                }

                case VariantType.Float32:
                {
                    return new Float32ValueVariant();
                }

                case VariantType.Float64:
                {
                    return new Float64ValueVariant();
                }

                case VariantType.String8:
                {
                    return new String8ValueVariant();
                }

                case VariantType.String16:
                {
                    return new String16ValueVariant();
                }

                case VariantType.ResourceKey:
                {
                    return new ResourceKeyValueVariant();
                }

                case VariantType.Flags:
                {
                    throw new NotImplementedException();
                }

                case VariantType.Text:
                {
                    throw new NotImplementedException();
                }

                case VariantType.GroupFilter:
                {
                    throw new NotImplementedException();
                }

                case VariantType.Vector2:
                {
                    return new Vector2ValueVariant();
                }
                case VariantType.Vector3:
                {
                    return new Vector3ValueVariant();
                }

                case VariantType.ColorRgb:
                {
                    return new ColorRgbValueVariant();
                }

                case VariantType.Vector4:
                {
                    return new Vector4ValueVariant();
                }

                case VariantType.ColorRgba:
                {
                    return new ColorRgbaValueVariant();
                }

                case VariantType.Matrix2:
                {
                    throw new NotImplementedException();
                }

                case VariantType.Matrix3:
                {
                    throw new NotImplementedException();
                }

                case VariantType.Matrix4:
                {
                    throw new NotImplementedException();
                }

                case VariantType.Transform:
                {
                    throw new NotImplementedException();
                }

                case VariantType.BoundingBox:
                {
                    return new BoundingBoxValueVariant();
                }
            }

            throw new NotSupportedException();
        }
    }
}
