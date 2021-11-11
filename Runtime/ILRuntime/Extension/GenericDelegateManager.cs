/*
 * This file is part of Taichi project.
 *
 * (c) MuGuangyi <muguangyi@hotmail.com>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 */

using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Intepreter;
using System;
using System.Collections.Generic;

namespace ILRuntime.Runtime.Enviorment
{
    public class GenericDelegateManager : DelegateManager
    {
        private readonly AppDomain domain = null;
        private readonly IDelegateAdapter dummyAdapter = new DummyDelegateAdapter();

        public GenericDelegateManager(AppDomain domain) : base(domain)
        {
            this.domain = domain;
        }

        internal override Delegate ConvertToDelegate(Type clrDelegateType, IDelegateAdapter adapter)
        {
            return adapter.Delegate;
        }

        internal override IDelegateAdapter FindDelegateAdapter(ILTypeInstance instance, ILMethod ilMethod, IMethod method)
        {
            IDelegateAdapter adapter = null;

            // Use DummyAdapter for IL delegate type.
            if (method is ILMethod)
            {
                adapter = this.dummyAdapter.Instantiate(this.domain, instance, ilMethod);
            }
            else
            {
                adapter = MakeCrossDelegateAdapter(domain, instance, ilMethod, method);
            }

            instance?.SetDelegateAdapter(ilMethod, adapter);

            return adapter;
        }

        public static IDelegateAdapter MakeCrossDelegateAdapter(AppDomain domain, ILTypeInstance instance, ILMethod ilMethod, IMethod method)
        {
            var ps = method.Parameters;
            var list = new List<Type>();
            if (ps != null)
            {
                for (int i = 0; i < ps.Count; i++)
                {
                    list.Add(ps[i].TypeForCLR);
                }
            }

            var returned = method.ReturnType != domain.VoidType;
            if (returned)
            {
                list.Add(method.ReturnType.TypeForCLR);
            }
            var args = list.ToArray();

            Type genericType = null;
            if (method.ReturnType == domain.VoidType)
            {
                switch (method.ParameterCount)
                {
                    case 0:
                        genericType = typeof(ActionAdaptor);
                        break;
                    case 1:
                        genericType = typeof(ActionAdaptor<>).MakeGenericType(args);
                        break;
                    case 2:
                        genericType = typeof(ActionAdaptor<,>).MakeGenericType(args);
                        break;
                    case 3:
                        genericType = typeof(ActionAdaptor<,,>).MakeGenericType(args);
                        break;
                    case 4:
                        genericType = typeof(ActionAdaptor<,,,>).MakeGenericType(args);
                        break;
                    case 5:
                        genericType = typeof(ActionAdaptor<,,,,>).MakeGenericType(args);
                        break;
                    case 6:
                        genericType = typeof(ActionAdaptor<,,,,,>).MakeGenericType(args);
                        break;
                    case 7:
                        genericType = typeof(ActionAdaptor<,,,,,>).MakeGenericType(args);
                        break;
                    case 8:
                        genericType = typeof(ActionAdaptor<,,,,,>).MakeGenericType(args);
                        break;
                    case 9:
                        genericType = typeof(ActionAdaptor<,,,,,>).MakeGenericType(args);
                        break;
                }
            }
            else
            {
                switch (method.ParameterCount)
                {
                    case 0:
                        genericType = typeof(FuncAdaptor<>).MakeGenericType(args);
                        break;
                    case 1:
                        genericType = typeof(FuncAdaptor<,>).MakeGenericType(args);
                        break;
                    case 2:
                        genericType = typeof(FuncAdaptor<,,>).MakeGenericType(args);
                        break;
                    case 3:
                        genericType = typeof(FuncAdaptor<,,,>).MakeGenericType(args);
                        break;
                    case 4:
                        genericType = typeof(FuncAdaptor<,,,,>).MakeGenericType(args);
                        break;
                    case 5:
                        genericType = typeof(FuncAdaptor<,,,,,>).MakeGenericType(args);
                        break;
                    case 6:
                        genericType = typeof(FuncAdaptor<,,,,,,>).MakeGenericType(args);
                        break;
                    case 7:
                        genericType = typeof(FuncAdaptor<,,,,,,,>).MakeGenericType(args);
                        break;
                    case 8:
                        genericType = typeof(FuncAdaptor<,,,,,,,,>).MakeGenericType(args);
                        break;
                    case 9:
                        genericType = typeof(FuncAdaptor<,,,,,,,,,>).MakeGenericType(args);
                        break;
                }
            }

            if (genericType == null)
            {
                throw new InvalidCastException(method.Name);
            }

            var delegateType = method.DeclearingType.TypeForCLR;
            return (IDelegateAdapter)Activator.CreateInstance(genericType, delegateType, domain, instance, ilMethod);
        }
    }
}
