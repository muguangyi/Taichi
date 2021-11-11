/*
 * This file is part of Taichi project.
 *
 * (c) MuGuangyi <muguangyi@hotmail.com>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 */

using Taichi.Foundation.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using System.Reflection;

namespace Taichi.Foundation
{
    public static partial class Assembler
    {
        private static readonly List<ITypeResolver> resolvers = new List<ITypeResolver>();
        private static readonly Dictionary<string, IModule> modules = new Dictionary<string, IModule>();

        static Assembler()
        {
            InstallResolver(new SystemResolver());
        }

        public static void ImportModule(Type interType, Type implType, bool loadImmediate = false)
        {
            if (!interType.IsAssignableFrom(implType))
            {
                throw new InvalidOperationException($"{implType} is not implemented from {interType}");
            }

            var name = interType.FullName;
            var module = new Module(implType);
            if (modules.ContainsKey(name))
            {
                modules[name] = module;
            }
            else
            {
                modules.Add(name, module);
            }

            if (loadImmediate)
            {
                Query(interType);
            }
        }

        public static void ImportModule<Interface, Implement>(bool loadImmediate = false) where Implement : Interface
        {
            ImportModule(typeof(Interface), typeof(Implement), loadImmediate);
        }

        public static void ImportModuleInstance(object instance, Type interType, Type implType)
        {
            if (interType == null || implType == null)
            {
                throw new NullReferenceException("interType or implType");
            }

            if (!interType.IsAssignableFrom(implType))
            {
                throw new InvalidOperationException($"{implType} is not implemented from {interType}");
            }

            var name = interType.FullName;
            var module = new Module(implType, instance);
            if (modules.ContainsKey(name))
            {
                modules[name] = module;
            }
            else
            {
                modules.Add(name, module);
            }
        }

        public static void ImportModuleInstance<Interface, Implement>(object instance) where Implement : Interface
        {
            ImportModuleInstance(instance, typeof(Interface), typeof(Implement));
        }

        public static void Tick(float deltaTime)
        {
            Async.Async.Tick(deltaTime);

            foreach (var i in modules)
            {
                i.Value?.Tick(deltaTime);
            }
        }

        public static void InstallResolver(ITypeResolver resolver)
        {
            resolvers.Add(resolver);
        }

        internal static IList Modules => modules.Select(i => i.Value).ToArray();

        private static object Query(Type interType)
        {
            var name = interType.FullName;
            if (modules.TryGetValue(name, out IModule module))
            {
                return module.Target;
            }

            return default;
        }

        private static object CreateInstance(string module, params object[] payload)
        {
            for (var i = resolvers.Count - 1; i >= 0; --i)
            {
                var obj = resolvers[i].CreateInstance(module, payload);
                if (obj != null)
                {
                    return obj;
                }
            }

            return null;
        }

        private static Delegate CreateDelegate(Type type, object target, MethodInfo method)
        {
            for (var i = resolvers.Count - 1; i >= 0; --i)
            {
                var del = resolvers[i].CreateDelegate(type, target, method);
                if (del != null)
                {
                    return del;
                }
            }

            return null;
        }
    }
}

