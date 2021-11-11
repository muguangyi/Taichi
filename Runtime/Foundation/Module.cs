/*
 * This file is part of Taichi project.
 *
 * (c) MuGuangyi <muguangyi@hotmail.com>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Taichi.Foundation.Internal;

namespace Taichi.Foundation
{
    using UpdateHandler = Action<float>;

    public static partial class Assembler
    {
        private class Module : IModule
        {
            private const string ContextsProperty = "Contexts";
            private const string OnResolveMethod = "OnResolve";
            private const string OnUpdateMethod = "OnUpdate";

            private object instance = null;
            private UpdateHandler updateHandler = null;

            public Module(Type type)
            {
                this.Type = type;
            }

            public Module(Type type, object instance)
            {
                this.Type = type;
                this.instance = instance;
                Setup();
            }

            public Type Type { get; } = null;

            public object Target
            {
                get
                {
                    if (this.instance == null)
                    {
                        this.instance = CreateInstance(this.Type.FullName);
                        Setup();
                    }

                    return this.instance;
                }
            }

            public void Tick(float deltaTime)
            {
                this.updateHandler?.Invoke(deltaTime);
            }

            private void Setup()
            {
                InjectType(this.Type);

                var ctxsProperty = this.Type.GetProperty(ContextsProperty, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (ctxsProperty != null)
                {
                    var value = ctxsProperty.GetGetMethod(true).Invoke(this.instance, null);
                    if (value != null && value is IEnumerable<Type> ctxs)
                    {
                        foreach (var ctx in ctxs)
                        {
                            InjectType(ctx);
                        }
                    }
                }

                var resolveMethod = this.Type.GetMethod(OnResolveMethod, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                resolveMethod?.Invoke(this.instance, null);

                var updateMethod = this.Type.GetMethod(OnUpdateMethod, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (updateMethod != null)
                {
                    try
                    {
                        this.updateHandler = (UpdateHandler)CreateDelegate(typeof(UpdateHandler), this.instance, updateMethod);
                    }
                    catch
                    { }
                }
            }

            private static void InjectType(Type type)
            {
                var fields = type.GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                                 .Where(f => f.GetCustomAttributes(typeof(ResolveAttribute), false).Length > 0)
                                 .ToArray();
                for (var i = 0; i < fields.Length; ++i)
                {
                    var f = fields[i];
                    if (f.FieldType.IsInterface || f.FieldType.IsClass)
                    {
                        f.SetValue(null, Query(f.FieldType));
                    }
                }
            }
        }
    }
}
