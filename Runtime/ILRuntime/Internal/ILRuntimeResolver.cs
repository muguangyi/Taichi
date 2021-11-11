/*
 * This file is part of Taichi project.
 *
 * (c) MuGuangyi <muguangyi@hotmail.com>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 */

using ILRuntime.CLR.TypeSystem;
using ILRuntime.Reflection;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;
using System;
using System.Reflection;
using Taichi.Foundation;

namespace Taichi.ILRuntime.Internal
{
    internal sealed class ILRuntimeResolver : ITypeResolver
    {
        private readonly global::ILRuntime.Runtime.Enviorment.AppDomain domain = null;

        public ILRuntimeResolver(global::ILRuntime.Runtime.Enviorment.AppDomain domain)
        {
            this.domain = domain;
        }

        public object CreateInstance(string module, params object[] payload)
        {
            if (this.domain.LoadedTypes.TryGetValue(module, out IType type))
            {
                var inst = this.domain.Instantiate(module, payload);
                return inst?.CLRInstance;
            }

            return null;
        }

        public Delegate CreateDelegate(Type type, object target, MethodInfo method)
        {
            IDelegateAdapter adaptor = null;
            if (method is ILRuntimeMethodInfo ilMethodInfo && ilMethodInfo.ILMethod != null)
            {
                var ilMethod = ilMethodInfo.ILMethod;

                ILTypeInstance instance = (target != null && target is ILTypeInstance) ? (ILTypeInstance)target : null;
                var m = ilMethod.DeclearingType.GetMethod(method.Name, ilMethod.ParameterCount);
                adaptor = GenericDelegateManager.MakeCrossDelegateAdapter(this.domain, instance, ilMethod, m);
            }

            return adaptor?.GetConvertor(type);
        }
    }
}
