/*
 * This file is part of Taichi project.
 *
 * (c) MuGuangyi <muguangyi@hotmail.com>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 */

using ILRuntime.CLR.Method;
using ILRuntime.CLR.TypeSystem;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;
using ILRuntime.Runtime.Stack;
using Taichi.Foundation;
using System;
using System.Collections.Generic;

namespace Taichi.ILRuntime.Internal
{
    internal static class ScriptExtension
    {
        public static unsafe void Extend(global::ILRuntime.Runtime.Enviorment.AppDomain domain)
        {
            // Install ILRuntime object creation resolver.
            Assembler.InstallResolver(new ILRuntimeResolver(domain));

            // Setup redirections.
            Redirect(domain, typeof(Assembler), nameof(Assembler.ImportModule), true, RedirectImportModuleGeneric);
            Redirect(domain, typeof(Assembler), nameof(Assembler.ImportModuleInstance), true, RedirectImportInstanceGeneric);
        }

        private static unsafe StackObject* RedirectImportModuleGeneric(ILIntepreter intp, StackObject* esp, IList<object> mStack, CLRMethod method, bool isNewObj)
        {
            var genericArgs = method.GenericArguments;
            if (genericArgs != null && genericArgs.Length == 2)
            {
                var p = ILIntepreter.Minus(esp, 1);
                var loadImmediate = p->Value == 1;
                var interType = genericArgs[0] is ILType ? ((ILType)genericArgs[0]).ReflectionType : genericArgs[0].TypeForCLR;
                var implType = genericArgs[1] is ILType ? ((ILType)genericArgs[1]).ReflectionType : genericArgs[1].TypeForCLR;
                Assembler.ImportModule(interType, implType, loadImmediate);
            }

            return ILIntepreter.Minus(esp, 1);
        }

        private static unsafe StackObject* RedirectImportInstanceGeneric(ILIntepreter intp, StackObject* esp, IList<object> mStack, CLRMethod method, bool isNewObj)
        {
            var genericArgs = method.GenericArguments;
            if (genericArgs != null && genericArgs.Length == 2)
            {
                var domain = intp.AppDomain;
                var param = esp - 1;
                var instance = StackObject.ToObject(param, domain, mStack);
                var interType = genericArgs[0] is ILType ? ((ILType)genericArgs[0]).ReflectionType : genericArgs[0].TypeForCLR;
                var implType = genericArgs[1] is ILType ? ((ILType)genericArgs[1]).ReflectionType : genericArgs[1].TypeForCLR;
                Assembler.ImportModuleInstance(instance, interType, implType);
            }

            return ILIntepreter.Minus(esp, 1);
        }

        private static unsafe void Redirect(global::ILRuntime.Runtime.Enviorment.AppDomain domain, Type type, string method, bool isGeneric, CLRRedirectionDelegate func)
        {
            if (string.IsNullOrEmpty(method))
            {
                return;
            }

            var methods = type.GetMethods();
            foreach (var m in methods)
            {
                if (m.Name == method && isGeneric == m.IsGenericMethod)
                {
                    domain.RegisterCLRMethodRedirection(m, func);
                }
            }
        }
    }
}
