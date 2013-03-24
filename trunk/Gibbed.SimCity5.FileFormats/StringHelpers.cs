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

namespace Gibbed.SimCity5.FileFormats
{
    public static class StringHelpers
    {
        public static uint HashFNV32(this string input)
        {
            return input.HashFNV32(0x811C9DC5u);
        }

        public static uint HashFNV32(this string input, uint seed)
        {
            if (input == null)
            {
                throw new ArgumentNullException("input");
            }

            if (input.Length == 0)
            {
                return 0;
            }

            var lower = input.ToLowerInvariant();

            var hash = seed;
            foreach (char t in lower)
            {
                hash *= 0x1000193u;
                hash ^= t;
            }
            return hash;
        }

        public static ulong HashFNV64(this string input)
        {
            return input.HashFNV64(0xCBF29CE484222325ul);
        }

        public static ulong HashFNV64(this string input, ulong seed)
        {
            if (input == null)
            {
                throw new ArgumentNullException("input");
            }

            if (input.Length == 0)
            {
                return 0;
            }

            string lower = input.ToLowerInvariant();

            var hash = seed;
            foreach (char t in lower)
            {
                hash *= 0x00000100000001B3ul;
                hash ^= t;
            }
            return hash;
        }
    }
}
