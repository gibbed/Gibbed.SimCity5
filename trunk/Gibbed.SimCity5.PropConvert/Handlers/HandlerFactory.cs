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
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Gibbed.SimCity5.PropConvert.Handlers
{
    internal static class HandlerFactory
    {
        private static readonly object _Lock;

        static HandlerFactory()
        {
            _Lock = new object();
        }

        private static List<BaseHandler> _Handlers;

        public static void GetHandlers(out Dictionary<Type, BaseHandler> typeHandlers,
                                       out Dictionary<string, BaseHandler> nameHandlers)
        {
            lock (_Lock)
            {
                if (_Handlers == null)
                {
                    var assembly = Assembly.GetAssembly(typeof(HandlerFactory));
                    _Handlers =
                        assembly.GetTypes()
                                .Where(
                                    t =>
                                    t.IsClass == true && t.IsAbstract == false &&
                                    t.IsSubclassOf(typeof(BaseHandler)) == true)
                                .Select(type => (BaseHandler)Activator.CreateInstance(type))
                                .ToList();
                }

                typeHandlers = _Handlers.ToDictionary(h => h.Type, h => h);
                nameHandlers = _Handlers.ToDictionary(h => h.Name, h => h);
            }
        }
    }
}
