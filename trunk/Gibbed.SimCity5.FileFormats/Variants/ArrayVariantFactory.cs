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
using Gibbed.SimCity5.FileFormats.Variants.Arrays;

namespace Gibbed.SimCity5.FileFormats.Variants
{
    internal static class ArrayVariantFactory
    {
        public static BaseVariant Create(VariantType type)
        {
            switch (type)
            {
                case VariantType.Bool:
                {
                    return new BoolArrayVariant();
                }

                case VariantType.Char8:
                {
                    throw new NotImplementedException();
                }

                case VariantType.Char16:
                {
                    throw new NotImplementedException();
                }

                case VariantType.Int8:
                {
                    throw new NotImplementedException();
                }

                case VariantType.UInt8:
                {
                    throw new NotImplementedException();
                }

                case VariantType.Int16:
                {
                    throw new NotImplementedException();
                }

                case VariantType.UInt16:
                {
                    throw new NotImplementedException();
                }

                case VariantType.Int32:
                {
                    return new Int32ArrayVariant();
                }

                case VariantType.UInt32:
                {
                    return new UInt32ArrayVariant();
                }

                case VariantType.Int64:
                {
                    throw new NotImplementedException();
                }

                case VariantType.UInt64:
                {
                    throw new NotImplementedException();
                }

                case VariantType.Float32:
                {
                    return new Float32ArrayVariant();
                }

                case VariantType.Float64:
                {
                    throw new NotImplementedException();
                }

                case VariantType.String8:
                {
                    return new String8ArrayVariant();
                }

                case VariantType.String16:
                {
                    return new String16ArrayVariant();
                }

                case VariantType.ResourceKey:
                {
                    return new ResourceKeyArrayVariant();
                }

                case VariantType.Flags:
                {
                    throw new NotImplementedException();
                }

                case VariantType.Text:
                {
                    return new TextArrayVariant();
                }

                case VariantType.GroupFilter:
                {
                    throw new NotImplementedException();
                }

                case VariantType.Vector2:
                {
                    return new Vector2ArrayVariant();
                }

                case VariantType.Vector3:
                {
                    return new Vector3ArrayVariant();
                }

                case VariantType.ColorRgb:
                {
                    return new ColorRgbArrayVariant();
                }

                case VariantType.Vector4:
                {
                    return new Vector4ArrayVariant();
                }

                case VariantType.ColorRgba:
                {
                    return new ColorRgbaArrayVariant();
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
                    return new TransformArrayVariant();
                }

                case VariantType.BoundingBox:
                {
                    return new BoundingBoxArrayVariant();
                }
            }

            throw new NotSupportedException();
        }
    }
}
