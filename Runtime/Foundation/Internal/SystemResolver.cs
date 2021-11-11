/*
 * This file is part of Taichi project.
 *
 * (c) MuGuangyi <muguangyi@hotmail.com>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 */

using System;
using System.Linq;
using System.Reflection;

namespace Taichi.Foundation.Internal
{
    internal sealed class SystemResolver : ITypeResolver
    {
        public object CreateInstance(string module, params object[] payload)
        {
            var t = FindType(module);
            return t != null ? Activator.CreateInstance(t, payload) : null;
        }

        public Delegate CreateDelegate(Type type, object target, MethodInfo method)
        {
            return Delegate.CreateDelegate(type, target, method);
        }

        private Type FindType(string module)
        {
            var t = Type.GetType(module);
            if (t == null)
            {
                var assemblies = AppDomain.CurrentDomain.GetAssemblies()
                                                        .Where(a => !FilterAssembly(a));
                t = assemblies.Where(a => a.GetType(module) != null)
                              .First()?
                              .GetType(module);
            }

            return t;
        }

        private bool FilterAssembly(Assembly assembly)
        {
            var assemblyName = assembly.FullName;
            return assemblyName.StartsWith("Unity") ||
                   assemblyName.StartsWith("System") ||
                   assemblyName.StartsWith("Microsoft") ||
                   assemblyName.StartsWith("SyntaxTree") ||
                   assemblyName.StartsWith("Mono") ||
                   assemblyName.StartsWith("ExCSS") ||
                   assemblyName.StartsWith("nunit") ||
                   assemblyName.StartsWith("netstandard") ||
                   assemblyName.StartsWith("mscorlib");
        }
    }
}
