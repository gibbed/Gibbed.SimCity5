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
    public enum VariantType : ushort
    {
        Bool = 1,
        Char8 = 2,
        Char16 = 3,
        Int8 = 5,
        UInt8 = 6,
        Int16 = 7,
        UInt16 = 8,
        Int32 = 9,
        UInt32 = 10,
        Int64 = 11,
        UInt64 = 12,
        Float32 = 13,
        Float64 = 14,
        String8 = 18,
        String16 = 19,
        ResourceKey = 32,
        Flags = 33,
        Text = 34,
        GroupFilter = 35,
        Vector2 = 48,
        Vector3 = 49,
        ColorRgb = 50,
        Vector4 = 51,
        ColorRgba = 52,
        Matrix2 = 53,
        Matrix3 = 54,
        Matrix4 = 55,
        Transform = 56,
        BoundingBox = 57,
    }
}
