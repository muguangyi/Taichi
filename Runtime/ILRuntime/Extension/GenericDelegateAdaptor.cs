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

namespace ILRuntime.Runtime.Enviorment
{
    public abstract class GenericDelegateAdaptor : DelegateAdapter
    {
        protected readonly Type delegateType = null;
        private Delegate del = null;

        public GenericDelegateAdaptor(Type delegateType, AppDomain appdomain, ILTypeInstance instance, ILMethod method)
            : base(appdomain, instance, method)
        {
            this.delegateType = delegateType ?? throw new ArgumentNullException("delegateType");
        }

        public sealed override Delegate Delegate => del;

        protected void SetDelegate(Delegate del)
        {
            if (this.delegateType == typeof(ILTypeInstance))
            {
                this.del = del;
            }
            else
            {
                this.del = Delegate.CreateDelegate(this.delegateType, del.Target, del.Method);
            }
        }
    }

    public sealed class FuncAdaptor<TResult> : GenericDelegateAdaptor
    {
        private Func<TResult> func = null;

        private static InvocationTypes[] pTypes = null;

        static FuncAdaptor()
        {
            pTypes = new InvocationTypes[]
            {
                InvocationContext.GetInvocationType<TResult>(),
            };
        }

        public FuncAdaptor(Type delegateType, AppDomain appdomain, ILTypeInstance instance, ILMethod method)
            : base(delegateType, appdomain, instance, method)
        {
            SetDelegate(this.func = InvokeILMethod);
        }

        private unsafe TResult InvokeILMethod()
        {
            using (var ctx = BeginInvoke())
            {
                var esp = ILInvoke(ctx.Intepreter, ctx.ESP, ctx.ManagedStack);
                ctx.SetInvoked(esp);
                return ctx.ReadResult<TResult>(pTypes[0]);
            }
        }

        public override IDelegateAdapter Instantiate(AppDomain appdomain, ILTypeInstance instance, ILMethod method)
        {
            return new FuncAdaptor<TResult>(this.delegateType, appdomain, instance, method);
        }

        public override IDelegateAdapter Clone()
        {
            var res = new FuncAdaptor<TResult>(this.delegateType, appdomain, instance, method);
            res.isClone = true;
            return res;
        }

        public override void Combine(Delegate del)
        {
            this.func += (Func<TResult>)del;
        }

        public override void Remove(Delegate del)
        {
            this.func -= (Func<TResult>)del;
        }
    }

    public sealed class FuncAdaptor<T1, TResult> : GenericDelegateAdaptor
    {
        private Func<T1, TResult> func = null;

        private static InvocationTypes[] pTypes = null;

        static FuncAdaptor()
        {
            pTypes = new InvocationTypes[]
            {
                InvocationContext.GetInvocationType<T1>(),
                InvocationContext.GetInvocationType<TResult>(),
            };
        }

        public FuncAdaptor(Type delegateType, AppDomain appdomain, ILTypeInstance instance, ILMethod method)
            : base(delegateType, appdomain, instance, method)
        {
            SetDelegate(this.func = InvokeILMethod);
        }

        private unsafe TResult InvokeILMethod(T1 p1)
        {
            using (var ctx = BeginInvoke())
            {
                ctx.PushParameter(pTypes[0], p1);

                var esp = ILInvoke(ctx.Intepreter, ctx.ESP, ctx.ManagedStack);
                ctx.SetInvoked(esp);
                return ctx.ReadResult<TResult>(pTypes[1]);
            }
        }

        public override IDelegateAdapter Instantiate(AppDomain appdomain, ILTypeInstance instance, ILMethod method)
        {
            return new FuncAdaptor<T1, TResult>(this.delegateType, appdomain, instance, method);
        }

        public override IDelegateAdapter Clone()
        {
            var res = new FuncAdaptor<T1, TResult>(this.delegateType, appdomain, instance, method);
            res.isClone = true;
            return res;
        }

        public override void Combine(Delegate del)
        {
            this.func += (Func<T1, TResult>)del;
        }

        public override void Remove(Delegate del)
        {
            this.func -= (Func<T1, TResult>)del;
        }
    }

    public sealed class FuncAdaptor<T1, T2, TResult> : GenericDelegateAdaptor
    {
        private Func<T1, T2, TResult> func = null;

        private static InvocationTypes[] pTypes = null;

        static FuncAdaptor()
        {
            pTypes = new InvocationTypes[]
            {
                InvocationContext.GetInvocationType<T1>(),
                InvocationContext.GetInvocationType<T2>(),
                InvocationContext.GetInvocationType<TResult>(),
            };
        }

        public FuncAdaptor(Type delegateType, AppDomain appdomain, ILTypeInstance instance, ILMethod method)
            : base(delegateType, appdomain, instance, method)
        {
            SetDelegate(this.func = InvokeILMethod);
        }

        private unsafe TResult InvokeILMethod(T1 p1, T2 p2)
        {
            using (var ctx = BeginInvoke())
            {
                ctx.PushParameter(pTypes[0], p1);
                ctx.PushParameter(pTypes[1], p2);

                var esp = ILInvoke(ctx.Intepreter, ctx.ESP, ctx.ManagedStack);
                ctx.SetInvoked(esp);
                return ctx.ReadResult<TResult>(pTypes[2]);
            }
        }

        public override IDelegateAdapter Instantiate(AppDomain appdomain, ILTypeInstance instance, ILMethod method)
        {
            return new FuncAdaptor<T1, T2, TResult>(this.delegateType, appdomain, instance, method);
        }

        public override IDelegateAdapter Clone()
        {
            var res = new FuncAdaptor<T1, T2, TResult>(this.delegateType, appdomain, instance, method);
            res.isClone = true;
            return res;
        }

        public override void Combine(Delegate del)
        {
            this.func += (Func<T1, T2, TResult>)del;
        }

        public override void Remove(Delegate del)
        {
            this.func -= (Func<T1, T2, TResult>)del;
        }
    }

    public sealed class FuncAdaptor<T1, T2, T3, TResult> : GenericDelegateAdaptor
    {
        private Func<T1, T2, T3, TResult> func = null;

        private static InvocationTypes[] pTypes = null;

        static FuncAdaptor()
        {
            pTypes = new InvocationTypes[]
            {
                InvocationContext.GetInvocationType<T1>(),
                InvocationContext.GetInvocationType<T2>(),
                InvocationContext.GetInvocationType<T3>(),
                InvocationContext.GetInvocationType<TResult>(),
            };
        }

        public FuncAdaptor(Type delegateType, AppDomain appdomain, ILTypeInstance instance, ILMethod method)
            : base(delegateType, appdomain, instance, method)
        {
            SetDelegate(this.func = InvokeILMethod);
        }

        private unsafe TResult InvokeILMethod(T1 p1, T2 p2, T3 p3)
        {
            using (var ctx = BeginInvoke())
            {
                ctx.PushParameter(pTypes[0], p1);
                ctx.PushParameter(pTypes[1], p2);
                ctx.PushParameter(pTypes[2], p3);

                var esp = ILInvoke(ctx.Intepreter, ctx.ESP, ctx.ManagedStack);
                ctx.SetInvoked(esp);
                return ctx.ReadResult<TResult>(pTypes[3]);
            }
        }

        public override IDelegateAdapter Instantiate(AppDomain appdomain, ILTypeInstance instance, ILMethod method)
        {
            return new FuncAdaptor<T1, T2, T3, TResult>(this.delegateType, appdomain, instance, method);
        }

        public override IDelegateAdapter Clone()
        {
            var res = new FuncAdaptor<T1, T2, T3, TResult>(this.delegateType, appdomain, instance, method);
            res.isClone = true;
            return res;
        }

        public override void Combine(Delegate del)
        {
            this.func += (Func<T1, T2, T3, TResult>)del;
        }

        public override void Remove(Delegate del)
        {
            this.func -= (Func<T1, T2, T3, TResult>)del;
        }
    }

    public sealed class FuncAdaptor<T1, T2, T3, T4, TResult> : GenericDelegateAdaptor
    {
        private Func<T1, T2, T3, T4, TResult> func = null;

        private static InvocationTypes[] pTypes = null;

        static FuncAdaptor()
        {
            pTypes = new InvocationTypes[]
            {
                InvocationContext.GetInvocationType<T1>(),
                InvocationContext.GetInvocationType<T2>(),
                InvocationContext.GetInvocationType<T3>(),
                InvocationContext.GetInvocationType<T4>(),
                InvocationContext.GetInvocationType<TResult>(),
            };
        }

        public FuncAdaptor(Type delegateType, AppDomain appdomain, ILTypeInstance instance, ILMethod method)
            : base(delegateType, appdomain, instance, method)
        {
            SetDelegate(this.func = InvokeILMethod);
        }

        private unsafe TResult InvokeILMethod(T1 p1, T2 p2, T3 p3, T4 p4)
        {
            using (var ctx = BeginInvoke())
            {
                ctx.PushParameter(pTypes[0], p1);
                ctx.PushParameter(pTypes[1], p2);
                ctx.PushParameter(pTypes[2], p3);
                ctx.PushParameter(pTypes[3], p4);

                var esp = ILInvoke(ctx.Intepreter, ctx.ESP, ctx.ManagedStack);
                ctx.SetInvoked(esp);
                return ctx.ReadResult<TResult>(pTypes[4]);
            }
        }

        public override IDelegateAdapter Instantiate(AppDomain appdomain, ILTypeInstance instance, ILMethod method)
        {
            return new FuncAdaptor<T1, T2, T3, T4, TResult>(this.delegateType, appdomain, instance, method);
        }

        public override IDelegateAdapter Clone()
        {
            var res = new FuncAdaptor<T1, T2, T3, T4, TResult>(this.delegateType, appdomain, instance, method);
            res.isClone = true;
            return res;
        }

        public override void Combine(Delegate del)
        {
            this.func += (Func<T1, T2, T3, T4, TResult>)del;
        }

        public override void Remove(Delegate del)
        {
            this.func -= (Func<T1, T2, T3, T4, TResult>)del;
        }
    }

    public sealed class FuncAdaptor<T1, T2, T3, T4, T5, TResult> : GenericDelegateAdaptor
    {
        private Func<T1, T2, T3, T4, T5, TResult> func = null;

        private static InvocationTypes[] pTypes = null;

        static FuncAdaptor()
        {
            pTypes = new InvocationTypes[]
            {
                InvocationContext.GetInvocationType<T1>(),
                InvocationContext.GetInvocationType<T2>(),
                InvocationContext.GetInvocationType<T3>(),
                InvocationContext.GetInvocationType<T4>(),
                InvocationContext.GetInvocationType<T5>(),
                InvocationContext.GetInvocationType<TResult>(),
            };
        }

        public FuncAdaptor(Type delegateType, AppDomain appdomain, ILTypeInstance instance, ILMethod method)
            : base(delegateType, appdomain, instance, method)
        {
            SetDelegate(this.func = InvokeILMethod);
        }

        private unsafe TResult InvokeILMethod(T1 p1, T2 p2, T3 p3, T4 p4, T5 p5)
        {
            using (var ctx = BeginInvoke())
            {
                ctx.PushParameter(pTypes[0], p1);
                ctx.PushParameter(pTypes[1], p2);
                ctx.PushParameter(pTypes[2], p3);
                ctx.PushParameter(pTypes[3], p4);
                ctx.PushParameter(pTypes[4], p5);

                var esp = ILInvoke(ctx.Intepreter, ctx.ESP, ctx.ManagedStack);
                ctx.SetInvoked(esp);
                return ctx.ReadResult<TResult>(pTypes[5]);
            }
        }

        public override IDelegateAdapter Instantiate(AppDomain appdomain, ILTypeInstance instance, ILMethod method)
        {
            return new FuncAdaptor<T1, T2, T3, T4, T5, TResult>(this.delegateType, appdomain, instance, method);
        }

        public override IDelegateAdapter Clone()
        {
            var res = new FuncAdaptor<T1, T2, T3, T4, T5, TResult>(this.delegateType, appdomain, instance, method);
            res.isClone = true;
            return res;
        }

        public override void Combine(Delegate del)
        {
            this.func += (Func<T1, T2, T3, T4, T5, TResult>)del;
        }

        public override void Remove(Delegate del)
        {
            this.func -= (Func<T1, T2, T3, T4, T5, TResult>)del;
        }
    }

    public sealed class FuncAdaptor<T1, T2, T3, T4, T5, T6, TResult> : GenericDelegateAdaptor
    {
        private Func<T1, T2, T3, T4, T5, T6, TResult> func = null;

        private static InvocationTypes[] pTypes = null;

        static FuncAdaptor()
        {
            pTypes = new InvocationTypes[]
            {
                InvocationContext.GetInvocationType<T1>(),
                InvocationContext.GetInvocationType<T2>(),
                InvocationContext.GetInvocationType<T3>(),
                InvocationContext.GetInvocationType<T4>(),
                InvocationContext.GetInvocationType<T5>(),
                InvocationContext.GetInvocationType<T6>(),
                InvocationContext.GetInvocationType<TResult>(),
            };
        }

        public FuncAdaptor(Type delegateType, AppDomain appdomain, ILTypeInstance instance, ILMethod method)
            : base(delegateType, appdomain, instance, method)
        {
            SetDelegate(this.func = InvokeILMethod);
        }

        private unsafe TResult InvokeILMethod(T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6)
        {
            using (var ctx = BeginInvoke())
            {
                ctx.PushParameter(pTypes[0], p1);
                ctx.PushParameter(pTypes[1], p2);
                ctx.PushParameter(pTypes[2], p3);
                ctx.PushParameter(pTypes[3], p4);
                ctx.PushParameter(pTypes[4], p5);
                ctx.PushParameter(pTypes[5], p6);

                var esp = ILInvoke(ctx.Intepreter, ctx.ESP, ctx.ManagedStack);
                ctx.SetInvoked(esp);
                return ctx.ReadResult<TResult>(pTypes[6]);
            }
        }

        public override IDelegateAdapter Instantiate(AppDomain appdomain, ILTypeInstance instance, ILMethod method)
        {
            return new FuncAdaptor<T1, T2, T3, T4, T5, T6, TResult>(this.delegateType, appdomain, instance, method);
        }

        public override IDelegateAdapter Clone()
        {
            var res = new FuncAdaptor<T1, T2, T3, T4, T5, T6, TResult>(this.delegateType, appdomain, instance, method);
            res.isClone = true;
            return res;
        }

        public override void Combine(Delegate del)
        {
            this.func += (Func<T1, T2, T3, T4, T5, T6, TResult>)del;
        }

        public override void Remove(Delegate del)
        {
            this.func -= (Func<T1, T2, T3, T4, T5, T6, TResult>)del;
        }
    }

    public sealed class FuncAdaptor<T1, T2, T3, T4, T5, T6, T7, TResult> : GenericDelegateAdaptor
    {
        private Func<T1, T2, T3, T4, T5, T6, T7, TResult> func = null;

        private static InvocationTypes[] pTypes = null;

        static FuncAdaptor()
        {
            pTypes = new InvocationTypes[]
            {
                InvocationContext.GetInvocationType<T1>(),
                InvocationContext.GetInvocationType<T2>(),
                InvocationContext.GetInvocationType<T3>(),
                InvocationContext.GetInvocationType<T4>(),
                InvocationContext.GetInvocationType<T5>(),
                InvocationContext.GetInvocationType<T6>(),
                InvocationContext.GetInvocationType<T7>(),
                InvocationContext.GetInvocationType<TResult>(),
            };
        }

        public FuncAdaptor(Type delegateType, AppDomain appdomain, ILTypeInstance instance, ILMethod method)
            : base(delegateType, appdomain, instance, method)
        {
            SetDelegate(this.func = InvokeILMethod);
        }

        private unsafe TResult InvokeILMethod(T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, T7 p7)
        {
            using (var ctx = BeginInvoke())
            {
                ctx.PushParameter(pTypes[0], p1);
                ctx.PushParameter(pTypes[1], p2);
                ctx.PushParameter(pTypes[2], p3);
                ctx.PushParameter(pTypes[3], p4);
                ctx.PushParameter(pTypes[4], p5);
                ctx.PushParameter(pTypes[5], p6);
                ctx.PushParameter(pTypes[6], p7);

                var esp = ILInvoke(ctx.Intepreter, ctx.ESP, ctx.ManagedStack);
                ctx.SetInvoked(esp);
                return ctx.ReadResult<TResult>(pTypes[7]);
            }
        }

        public override IDelegateAdapter Instantiate(AppDomain appdomain, ILTypeInstance instance, ILMethod method)
        {
            return new FuncAdaptor<T1, T2, T3, T4, T5, T6, T7, TResult>(this.delegateType, appdomain, instance, method);
        }

        public override IDelegateAdapter Clone()
        {
            var res = new FuncAdaptor<T1, T2, T3, T4, T5, T6, T7, TResult>(this.delegateType, appdomain, instance, method);
            res.isClone = true;
            return res;
        }

        public override void Combine(Delegate del)
        {
            this.func += (Func<T1, T2, T3, T4, T5, T6, T7, TResult>)del;
        }

        public override void Remove(Delegate del)
        {
            this.func -= (Func<T1, T2, T3, T4, T5, T6, T7, TResult>)del;
        }
    }

    public sealed class FuncAdaptor<T1, T2, T3, T4, T5, T6, T7, T8, TResult> : GenericDelegateAdaptor
    {
        private Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> func = null;

        private static InvocationTypes[] pTypes = null;

        static FuncAdaptor()
        {
            pTypes = new InvocationTypes[]
            {
                InvocationContext.GetInvocationType<T1>(),
                InvocationContext.GetInvocationType<T2>(),
                InvocationContext.GetInvocationType<T3>(),
                InvocationContext.GetInvocationType<T4>(),
                InvocationContext.GetInvocationType<T5>(),
                InvocationContext.GetInvocationType<T6>(),
                InvocationContext.GetInvocationType<T7>(),
                InvocationContext.GetInvocationType<T8>(),
                InvocationContext.GetInvocationType<TResult>(),
            };
        }

        public FuncAdaptor(Type delegateType, AppDomain appdomain, ILTypeInstance instance, ILMethod method)
            : base(delegateType, appdomain, instance, method)
        {
            SetDelegate(this.func = InvokeILMethod);
        }

        private unsafe TResult InvokeILMethod(T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, T7 p7, T8 p8)
        {
            using (var ctx = BeginInvoke())
            {
                ctx.PushParameter(pTypes[0], p1);
                ctx.PushParameter(pTypes[1], p2);
                ctx.PushParameter(pTypes[2], p3);
                ctx.PushParameter(pTypes[3], p4);
                ctx.PushParameter(pTypes[4], p5);
                ctx.PushParameter(pTypes[5], p6);
                ctx.PushParameter(pTypes[6], p7);
                ctx.PushParameter(pTypes[7], p8);

                var esp = ILInvoke(ctx.Intepreter, ctx.ESP, ctx.ManagedStack);
                ctx.SetInvoked(esp);
                return ctx.ReadResult<TResult>(pTypes[8]);
            }
        }

        public override IDelegateAdapter Instantiate(AppDomain appdomain, ILTypeInstance instance, ILMethod method)
        {
            return new FuncAdaptor<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(this.delegateType, appdomain, instance, method);
        }

        public override IDelegateAdapter Clone()
        {
            var res = new FuncAdaptor<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(this.delegateType, appdomain, instance, method);
            res.isClone = true;
            return res;
        }

        public override void Combine(Delegate del)
        {
            this.func += (Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult>)del;
        }

        public override void Remove(Delegate del)
        {
            this.func -= (Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult>)del;
        }
    }

    public sealed class FuncAdaptor<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> : GenericDelegateAdaptor
    {
        private Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> func = null;

        private static InvocationTypes[] pTypes = null;

        static FuncAdaptor()
        {
            pTypes = new InvocationTypes[]
            {
                InvocationContext.GetInvocationType<T1>(),
                InvocationContext.GetInvocationType<T2>(),
                InvocationContext.GetInvocationType<T3>(),
                InvocationContext.GetInvocationType<T4>(),
                InvocationContext.GetInvocationType<T5>(),
                InvocationContext.GetInvocationType<T6>(),
                InvocationContext.GetInvocationType<T7>(),
                InvocationContext.GetInvocationType<T8>(),
                InvocationContext.GetInvocationType<T9>(),
                InvocationContext.GetInvocationType<TResult>(),
            };
        }

        public FuncAdaptor(Type delegateType, AppDomain appdomain, ILTypeInstance instance, ILMethod method)
            : base(delegateType, appdomain, instance, method)
        {
            SetDelegate(this.func = InvokeILMethod);
        }

        private unsafe TResult InvokeILMethod(T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, T7 p7, T8 p8, T9 p9)
        {
            using (var ctx = BeginInvoke())
            {
                ctx.PushParameter(pTypes[0], p1);
                ctx.PushParameter(pTypes[1], p2);
                ctx.PushParameter(pTypes[2], p3);
                ctx.PushParameter(pTypes[3], p4);
                ctx.PushParameter(pTypes[4], p5);
                ctx.PushParameter(pTypes[5], p6);
                ctx.PushParameter(pTypes[6], p7);
                ctx.PushParameter(pTypes[7], p8);
                ctx.PushParameter(pTypes[8], p9);

                var esp = ILInvoke(ctx.Intepreter, ctx.ESP, ctx.ManagedStack);
                ctx.SetInvoked(esp);
                return ctx.ReadResult<TResult>(pTypes[9]);
            }
        }

        public override IDelegateAdapter Instantiate(AppDomain appdomain, ILTypeInstance instance, ILMethod method)
        {
            return new FuncAdaptor<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(this.delegateType, appdomain, instance, method);
        }

        public override IDelegateAdapter Clone()
        {
            var res = new FuncAdaptor<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(this.delegateType, appdomain, instance, method);
            res.isClone = true;
            return res;
        }

        public override void Combine(Delegate del)
        {
            this.func += (Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>)del;
        }

        public override void Remove(Delegate del)
        {
            this.func -= (Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>)del;
        }
    }

    public sealed class ActionAdaptor : GenericDelegateAdaptor
    {
        private Action action = null;

        public ActionAdaptor(Type delegateType, AppDomain appdomain, ILTypeInstance instance, ILMethod method)
            : base(delegateType, appdomain, instance, method)
        {
            SetDelegate(this.action = InvokeILMethod);
        }

        private unsafe void InvokeILMethod()
        {
            using (var ctx = BeginInvoke())
            {
                ILInvoke(ctx.Intepreter, ctx.ESP, ctx.ManagedStack);
            }
        }

        public override IDelegateAdapter Instantiate(AppDomain appdomain, ILTypeInstance instance, ILMethod method)
        {
            return new ActionAdaptor(this.delegateType, appdomain, instance, method);
        }

        public override IDelegateAdapter Clone()
        {
            var res = new ActionAdaptor(this.delegateType, appdomain, instance, method);
            res.isClone = true;
            return res;
        }

        public override void Combine(Delegate del)
        {
            this.action += (Action)del;
        }

        public override void Remove(Delegate del)
        {
            this.action -= (Action)del;
        }
    }

    public sealed class ActionAdaptor<T1> : GenericDelegateAdaptor
    {
        private Action<T1> action = null;
        private static InvocationTypes pType;

        static ActionAdaptor()
        {
            pType = InvocationContext.GetInvocationType<T1>();
        }

        public ActionAdaptor(Type delegateType, AppDomain appdomain, ILTypeInstance instance, ILMethod method)
            : base(delegateType, appdomain, instance, method)
        {
            SetDelegate(action = InvokeILMethod);
        }

        private unsafe void InvokeILMethod(T1 p1)
        {
            using (var ctx = BeginInvoke())
            {
                ctx.PushParameter(pType, p1);
                ILInvoke(ctx.Intepreter, ctx.ESP, ctx.ManagedStack);
            }
        }

        public override IDelegateAdapter Instantiate(AppDomain appdomain, ILTypeInstance instance, ILMethod method)
        {
            return new ActionAdaptor<T1>(this.delegateType, appdomain, instance, method);
        }

        public override IDelegateAdapter Clone()
        {
            var res = new ActionAdaptor<T1>(this.delegateType, appdomain, instance, method);
            res.isClone = true;
            return res;
        }

        public override void Combine(Delegate del)
        {
            this.action += (Action<T1>)del;
        }

        public override void Remove(Delegate del)
        {
            this.action -= (Action<T1>)del;
        }
    }

    public sealed class ActionAdaptor<T1, T2> : GenericDelegateAdaptor
    {
        private Action<T1, T2> action = null;

        private static InvocationTypes[] pTypes = null;

        static ActionAdaptor()
        {
            pTypes = new InvocationTypes[]
            {
                InvocationContext.GetInvocationType<T1>(),
                InvocationContext.GetInvocationType<T2>(),
            };
        }

        public ActionAdaptor(Type delegateType, AppDomain appdomain, ILTypeInstance instance, ILMethod method)
            : base(delegateType, appdomain, instance, method)
        {
            SetDelegate(this.action = InvokeILMethod);
        }

        private unsafe void InvokeILMethod(T1 p1, T2 p2)
        {
            using (var ctx = BeginInvoke())
            {
                ctx.PushParameter(pTypes[0], p1);
                ctx.PushParameter(pTypes[1], p2);
                ILInvoke(ctx.Intepreter, ctx.ESP, ctx.ManagedStack);
            }
        }

        public override IDelegateAdapter Instantiate(AppDomain appdomain, ILTypeInstance instance, ILMethod method)
        {
            return new ActionAdaptor<T1, T2>(this.delegateType, appdomain, instance, method);
        }

        public override IDelegateAdapter Clone()
        {
            var res = new ActionAdaptor<T1, T2>(this.delegateType, appdomain, instance, method);
            res.isClone = true;
            return res;
        }

        public override void Combine(Delegate del)
        {
            this.action += (Action<T1, T2>)del;
        }

        public override void Remove(Delegate del)
        {
            this.action -= (Action<T1, T2>)del;
        }
    }

    public sealed class ActionAdaptor<T1, T2, T3> : GenericDelegateAdaptor
    {
        private Action<T1, T2, T3> action = null;

        private static InvocationTypes[] pTypes = null;

        static ActionAdaptor()
        {
            pTypes = new InvocationTypes[]
            {
                InvocationContext.GetInvocationType<T1>(),
                InvocationContext.GetInvocationType<T2>(),
                InvocationContext.GetInvocationType<T3>(),
            };
        }

        public ActionAdaptor(Type delegateType, AppDomain appdomain, ILTypeInstance instance, ILMethod method)
            : base(delegateType, appdomain, instance, method)
        {
            SetDelegate(this.action = InvokeILMethod);
        }

        private unsafe void InvokeILMethod(T1 p1, T2 p2, T3 p3)
        {
            using (var ctx = BeginInvoke())
            {
                ctx.PushParameter(pTypes[0], p1);
                ctx.PushParameter(pTypes[1], p2);
                ctx.PushParameter(pTypes[2], p3);
                ILInvoke(ctx.Intepreter, ctx.ESP, ctx.ManagedStack);
            }
        }

        public override IDelegateAdapter Instantiate(AppDomain appdomain, ILTypeInstance instance, ILMethod method)
        {
            return new ActionAdaptor<T1, T2, T3>(this.delegateType, appdomain, instance, method);
        }

        public override IDelegateAdapter Clone()
        {
            var res = new ActionAdaptor<T1, T2, T3>(this.delegateType, appdomain, instance, method);
            res.isClone = true;
            return res;
        }

        public override void Combine(Delegate del)
        {
            this.action += (Action<T1, T2, T3>)del;
        }

        public override void Remove(Delegate del)
        {
            this.action -= (Action<T1, T2, T3>)del;
        }
    }

    public sealed class ActionAdaptor<T1, T2, T3, T4> : GenericDelegateAdaptor
    {
        private Action<T1, T2, T3, T4> action = null;

        private static InvocationTypes[] pTypes = null;

        static ActionAdaptor()
        {
            pTypes = new InvocationTypes[]
            {
                InvocationContext.GetInvocationType<T1>(),
                InvocationContext.GetInvocationType<T2>(),
                InvocationContext.GetInvocationType<T3>(),
                InvocationContext.GetInvocationType<T4>(),
            };
        }

        public ActionAdaptor(Type delegateType, AppDomain appdomain, ILTypeInstance instance, ILMethod method)
            : base(delegateType, appdomain, instance, method)
        {
            SetDelegate(action = InvokeILMethod);
        }

        private unsafe void InvokeILMethod(T1 p1, T2 p2, T3 p3, T4 p4)
        {
            using (var ctx = BeginInvoke())
            {
                ctx.PushParameter(pTypes[0], p1);
                ctx.PushParameter(pTypes[1], p2);
                ctx.PushParameter(pTypes[2], p3);
                ctx.PushParameter(pTypes[3], p4);
                ILInvoke(ctx.Intepreter, ctx.ESP, ctx.ManagedStack);
            }
        }

        public override IDelegateAdapter Instantiate(AppDomain appdomain, ILTypeInstance instance, ILMethod method)
        {
            return new ActionAdaptor<T1, T2, T3, T4>(this.delegateType, appdomain, instance, method);
        }

        public override IDelegateAdapter Clone()
        {
            var res = new ActionAdaptor<T1, T2, T3, T4>(this.delegateType, appdomain, instance, method);
            res.isClone = true;
            return res;
        }

        public override void Combine(Delegate del)
        {
            this.action += (Action<T1, T2, T3, T4>)del;
        }

        public override void Remove(Delegate del)
        {
            this.action -= (Action<T1, T2, T3, T4>)del;
        }
    }

    public sealed class ActionAdaptor<T1, T2, T3, T4, T5> : GenericDelegateAdaptor
    {
        private Action<T1, T2, T3, T4, T5> action = null;

        private static InvocationTypes[] pTypes = null;

        static ActionAdaptor()
        {
            pTypes = new InvocationTypes[]
            {
                InvocationContext.GetInvocationType<T1>(),
                InvocationContext.GetInvocationType<T2>(),
                InvocationContext.GetInvocationType<T3>(),
                InvocationContext.GetInvocationType<T4>(),
                InvocationContext.GetInvocationType<T5>(),
            };
        }

        public ActionAdaptor(Type delegateType, AppDomain appdomain, ILTypeInstance instance, ILMethod method)
            : base(delegateType, appdomain, instance, method)
        {
            SetDelegate(action = InvokeILMethod);
        }

        private unsafe void InvokeILMethod(T1 p1, T2 p2, T3 p3, T4 p4, T5 p5)
        {
            using (var ctx = BeginInvoke())
            {
                ctx.PushParameter(pTypes[0], p1);
                ctx.PushParameter(pTypes[1], p2);
                ctx.PushParameter(pTypes[2], p3);
                ctx.PushParameter(pTypes[3], p4);
                ctx.PushParameter(pTypes[4], p5);
                ILInvoke(ctx.Intepreter, ctx.ESP, ctx.ManagedStack);
            }
        }

        public override IDelegateAdapter Instantiate(AppDomain appdomain, ILTypeInstance instance, ILMethod method)
        {
            return new ActionAdaptor<T1, T2, T3, T4, T5>(this.delegateType, appdomain, instance, method);
        }

        public override IDelegateAdapter Clone()
        {
            var res = new ActionAdaptor<T1, T2, T3, T4, T5>(this.delegateType, appdomain, instance, method);
            res.isClone = true;
            return res;
        }

        public override void Combine(Delegate del)
        {
            this.action += (Action<T1, T2, T3, T4, T5>)del;
        }

        public override void Remove(Delegate del)
        {
            this.action -= (Action<T1, T2, T3, T4, T5>)del;
        }
    }

    public sealed class ActionAdaptor<T1, T2, T3, T4, T5, T6> : GenericDelegateAdaptor
    {
        private Action<T1, T2, T3, T4, T5, T6> action = null;

        private static InvocationTypes[] pTypes = null;

        static ActionAdaptor()
        {
            pTypes = new InvocationTypes[]
            {
                InvocationContext.GetInvocationType<T1>(),
                InvocationContext.GetInvocationType<T2>(),
                InvocationContext.GetInvocationType<T3>(),
                InvocationContext.GetInvocationType<T4>(),
                InvocationContext.GetInvocationType<T5>(),
                InvocationContext.GetInvocationType<T6>(),
            };
        }

        public ActionAdaptor(Type delegateType, AppDomain appdomain, ILTypeInstance instance, ILMethod method)
            : base(delegateType, appdomain, instance, method)
        {
            SetDelegate(this.action = InvokeILMethod);
        }

        private unsafe void InvokeILMethod(T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6)
        {
            using (var ctx = BeginInvoke())
            {
                ctx.PushParameter(pTypes[0], p1);
                ctx.PushParameter(pTypes[1], p2);
                ctx.PushParameter(pTypes[2], p3);
                ctx.PushParameter(pTypes[3], p4);
                ctx.PushParameter(pTypes[4], p5);
                ctx.PushParameter(pTypes[5], p6);
                ILInvoke(ctx.Intepreter, ctx.ESP, ctx.ManagedStack);
            }
        }

        public override IDelegateAdapter Instantiate(AppDomain appdomain, ILTypeInstance instance, ILMethod method)
        {
            return new ActionAdaptor<T1, T2, T3, T4, T5, T6>(this.delegateType, appdomain, instance, method);
        }

        public override IDelegateAdapter Clone()
        {
            var res = new ActionAdaptor<T1, T2, T3, T4, T5, T6>(this.delegateType, appdomain, instance, method);
            res.isClone = true;
            return res;
        }

        public override void Combine(Delegate del)
        {
            this.action += (Action<T1, T2, T3, T4, T5, T6>)del;
        }

        public override void Remove(Delegate del)
        {
            this.action -= (Action<T1, T2, T3, T4, T5, T6>)del;
        }
    }

    public sealed class ActionAdaptor<T1, T2, T3, T4, T5, T6, T7> : GenericDelegateAdaptor
    {
        private Action<T1, T2, T3, T4, T5, T6, T7> action = null;

        private static InvocationTypes[] pTypes = null;

        static ActionAdaptor()
        {
            pTypes = new InvocationTypes[]
            {
                InvocationContext.GetInvocationType<T1>(),
                InvocationContext.GetInvocationType<T2>(),
                InvocationContext.GetInvocationType<T3>(),
                InvocationContext.GetInvocationType<T4>(),
                InvocationContext.GetInvocationType<T5>(),
                InvocationContext.GetInvocationType<T6>(),
                InvocationContext.GetInvocationType<T7>(),
            };
        }

        public ActionAdaptor(Type delegateType, AppDomain appdomain, ILTypeInstance instance, ILMethod method)
            : base(delegateType, appdomain, instance, method)
        {
            SetDelegate(this.action = InvokeILMethod);
        }

        private unsafe void InvokeILMethod(T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, T7 p7)
        {
            using (var ctx = BeginInvoke())
            {
                ctx.PushParameter(pTypes[0], p1);
                ctx.PushParameter(pTypes[1], p2);
                ctx.PushParameter(pTypes[2], p3);
                ctx.PushParameter(pTypes[3], p4);
                ctx.PushParameter(pTypes[4], p5);
                ctx.PushParameter(pTypes[5], p6);
                ctx.PushParameter(pTypes[6], p7);
                ILInvoke(ctx.Intepreter, ctx.ESP, ctx.ManagedStack);
            }
        }

        public override IDelegateAdapter Instantiate(AppDomain appdomain, ILTypeInstance instance, ILMethod method)
        {
            return new ActionAdaptor<T1, T2, T3, T4, T5, T6, T7>(this.delegateType, appdomain, instance, method);
        }

        public override IDelegateAdapter Clone()
        {
            var res = new ActionAdaptor<T1, T2, T3, T4, T5, T6, T7>(this.delegateType, appdomain, instance, method);
            res.isClone = true;
            return res;
        }

        public override void Combine(Delegate del)
        {
            this.action += (Action<T1, T2, T3, T4, T5, T6, T7>)del;
        }

        public override void Remove(Delegate del)
        {
            this.action -= (Action<T1, T2, T3, T4, T5, T6, T7>)del;
        }
    }

    public sealed class ActionAdaptor<T1, T2, T3, T4, T5, T6, T7, T8> : GenericDelegateAdaptor
    {
        private Action<T1, T2, T3, T4, T5, T6, T7, T8> action = null;

        private static InvocationTypes[] pTypes = null;

        static ActionAdaptor()
        {
            pTypes = new InvocationTypes[]
            {
                InvocationContext.GetInvocationType<T1>(),
                InvocationContext.GetInvocationType<T2>(),
                InvocationContext.GetInvocationType<T3>(),
                InvocationContext.GetInvocationType<T4>(),
                InvocationContext.GetInvocationType<T5>(),
                InvocationContext.GetInvocationType<T6>(),
                InvocationContext.GetInvocationType<T7>(),
                InvocationContext.GetInvocationType<T8>(),
            };
        }

        public ActionAdaptor(Type delegateType, AppDomain appdomain, ILTypeInstance instance, ILMethod method)
            : base(delegateType, appdomain, instance, method)
        {
            SetDelegate(this.action = InvokeILMethod);
        }

        private unsafe void InvokeILMethod(T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, T7 p7, T8 p8)
        {
            using (var ctx = BeginInvoke())
            {
                ctx.PushParameter(pTypes[0], p1);
                ctx.PushParameter(pTypes[1], p2);
                ctx.PushParameter(pTypes[2], p3);
                ctx.PushParameter(pTypes[3], p4);
                ctx.PushParameter(pTypes[4], p5);
                ctx.PushParameter(pTypes[5], p6);
                ctx.PushParameter(pTypes[6], p7);
                ctx.PushParameter(pTypes[7], p8);
                ILInvoke(ctx.Intepreter, ctx.ESP, ctx.ManagedStack);
            }
        }

        public override IDelegateAdapter Instantiate(AppDomain appdomain, ILTypeInstance instance, ILMethod method)
        {
            return new ActionAdaptor<T1, T2, T3, T4, T5, T6, T7, T8>(this.delegateType, appdomain, instance, method);
        }

        public override IDelegateAdapter Clone()
        {
            var res = new ActionAdaptor<T1, T2, T3, T4, T5, T6, T7, T8>(this.delegateType, appdomain, instance, method);
            res.isClone = true;
            return res;
        }

        public override void Combine(Delegate del)
        {
            this.action += (Action<T1, T2, T3, T4, T5, T6, T7, T8>)del;
        }

        public override void Remove(Delegate del)
        {
            this.action -= (Action<T1, T2, T3, T4, T5, T6, T7, T8>)del;
        }
    }

    public sealed class ActionAdaptor<T1, T2, T3, T4, T5, T6, T7, T8, T9> : GenericDelegateAdaptor
    {
        private Action<T1, T2, T3, T4, T5, T6, T7, T8, T9> action = null;

        private static InvocationTypes[] pTypes = null;

        static ActionAdaptor()
        {
            pTypes = new InvocationTypes[]
            {
                InvocationContext.GetInvocationType<T1>(),
                InvocationContext.GetInvocationType<T2>(),
                InvocationContext.GetInvocationType<T3>(),
                InvocationContext.GetInvocationType<T4>(),
                InvocationContext.GetInvocationType<T5>(),
                InvocationContext.GetInvocationType<T6>(),
                InvocationContext.GetInvocationType<T7>(),
                InvocationContext.GetInvocationType<T8>(),
                InvocationContext.GetInvocationType<T9>(),
            };
        }

        public ActionAdaptor(Type delegateType, AppDomain appdomain, ILTypeInstance instance, ILMethod method)
            : base(delegateType, appdomain, instance, method)
        {
            SetDelegate(this.action = InvokeILMethod);
        }

        private unsafe void InvokeILMethod(T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, T7 p7, T8 p8, T9 p9)
        {
            using (var ctx = BeginInvoke())
            {
                ctx.PushParameter(pTypes[0], p1);
                ctx.PushParameter(pTypes[1], p2);
                ctx.PushParameter(pTypes[2], p3);
                ctx.PushParameter(pTypes[3], p4);
                ctx.PushParameter(pTypes[4], p5);
                ctx.PushParameter(pTypes[5], p6);
                ctx.PushParameter(pTypes[6], p7);
                ctx.PushParameter(pTypes[7], p8);
                ctx.PushParameter(pTypes[8], p9);
                ILInvoke(ctx.Intepreter, ctx.ESP, ctx.ManagedStack);
            }
        }

        public override IDelegateAdapter Instantiate(AppDomain appdomain, ILTypeInstance instance, ILMethod method)
        {
            return new ActionAdaptor<T1, T2, T3, T4, T5, T6, T7, T8, T9>(this.delegateType, appdomain, instance, method);
        }

        public override IDelegateAdapter Clone()
        {
            var res = new ActionAdaptor<T1, T2, T3, T4, T5, T6, T7, T8, T9>(this.delegateType, appdomain, instance, method);
            res.isClone = true;
            return res;
        }

        public override void Combine(Delegate del)
        {
            this.action += (Action<T1, T2, T3, T4, T5, T6, T7, T8, T9>)del;
        }

        public override void Remove(Delegate del)
        {
            this.action -= (Action<T1, T2, T3, T4, T5, T6, T7, T8, T9>)del;
        }
    }
}
