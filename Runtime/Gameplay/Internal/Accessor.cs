/*
 * This file is part of Taichi project.
 *
 * (c) MuGuangyi <muguangyi@hotmail.com>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 */

using System;

namespace Taichi.Gameplay.Internal
{
    internal abstract class AbstractGetter
    {
        public virtual object Call()
        {
            return null;
        }
    }

    internal class GenericGetter : AbstractGetter
    {
        private readonly ITrait trait = null;
        private readonly string name = null;

        public GenericGetter(ITrait trait, string name)
        {
            this.trait = trait;
            this.name = name;
        }

        public override object Call()
        {
            return this.trait.Get(name);
        }
    }

    internal class Getter<T> : AbstractGetter
    {
        public delegate T P();

        private readonly P p = null;

        public Getter(Delegate del)
        {
            this.p = (P)del;
        }

        public override object Call()
        {
            return this.p();
        }
    }

    internal abstract class AbstractSetter
    {
        public virtual void Call(object t)
        { }
    }

    internal class GenericSetter : AbstractSetter
    {
        private readonly ITrait trait = null;
        private readonly string name = null;

        public GenericSetter(ITrait trait, string name)
        {
            this.trait = trait;
            this.name = name;
        }

        public override void Call(object t)
        {
            this.trait.Set(this.name, t);
        }
    }

    internal class Setter<T> : AbstractSetter
    {
        public delegate void P(T t);

        private readonly P p = null;

        public Setter(Delegate del)
        {
            this.p = (P)del;
        }

        public override void Call(object t)
        {
            this.p(t.Cast<T>());
        }
    }

    internal abstract class AbstractMethod
    {
        public virtual void Call(params object[] args)
        { }

        public virtual object CallWithReturn(params object[] args)
        {
            return null;
        }
    }

    internal abstract class AbstractMethodRet : AbstractMethod
    {
        public override void Call(params object[] args)
        {
            CallWithReturn(args);
        }
    }

    internal class GenericMethod : AbstractMethod
    {
        private readonly ITrait trait = null;
        private readonly string method = null;

        public GenericMethod(ITrait trait, string method)
        {
            this.trait = trait;
            this.method = method;
        }

        public override void Call(params object[] args)
        {
            this.trait?.Call(this.method, args);
        }

        public override object CallWithReturn(params object[] args)
        {
            return this.trait?.CallWithReturn(this.method, args);
        }
    }

    internal class Method : AbstractMethod
    {
        public delegate void P();

        private readonly P p = null;

        public Method(Delegate del)
        {
            this.p = (P)del;
        }

        public override void Call(params object[] args)
        {
            this.p();
        }
    }

    internal class Method<T1> : AbstractMethod
    {
        public delegate void P(T1 t1);

        private readonly P p = null;

        public Method(Delegate del)
        {
            this.p = (P)del;
        }

        public override void Call(params object[] args)
        {
            this.p(
                args[0].Cast<T1>()
            );
        }
    }

    internal class Method<T1, T2> : AbstractMethod
    {
        public delegate void P(T1 t1, T2 t2);

        private readonly P p = null;

        public Method(Delegate del)
        {
            this.p = (P)del;
        }

        public override void Call(params object[] args)
        {
            this.p(
                args[0].Cast<T1>(),
                args[1].Cast<T2>()
            );
        }
    }

    internal class Method<T1, T2, T3> : AbstractMethod
    {
        public delegate void P(T1 t1, T2 t2, T3 t3);

        private readonly P p = null;

        public Method(Delegate del)
        {
            this.p = (P)del;
        }

        public override void Call(params object[] args)
        {
            this.p(
                args[0].Cast<T1>(),
                args[1].Cast<T2>(),
                args[2].Cast<T3>()
            );
        }
    }

    internal class Method<T1, T2, T3, T4> : AbstractMethod
    {
        public delegate void P(T1 t1, T2 t2, T3 t3, T4 t4);

        private readonly P p = null;

        public Method(Delegate del)
        {
            this.p = (P)del;
        }

        public override void Call(params object[] args)
        {
            this.p(
                args[0].Cast<T1>(),
                args[1].Cast<T2>(),
                args[2].Cast<T3>(),
                args[3].Cast<T4>()
            );
        }
    }

    internal class Method<T1, T2, T3, T4, T5> : AbstractMethod
    {
        public delegate void P(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5);

        private readonly P p = null;

        public Method(Delegate del)
        {
            this.p = (P)del;
        }

        public override void Call(params object[] args)
        {
            this.p(
                args[0].Cast<T1>(),
                args[1].Cast<T2>(),
                args[2].Cast<T3>(),
                args[3].Cast<T4>(),
                args[4].Cast<T5>()
            );
        }
    }

    internal class Method<T1, T2, T3, T4, T5, T6> : AbstractMethod
    {
        public delegate void P(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6);

        private readonly P p = null;

        public Method(Delegate del)
        {
            this.p = (P)del;
        }

        public override void Call(params object[] args)
        {
            this.p(
                args[0].Cast<T1>(),
                args[1].Cast<T2>(),
                args[2].Cast<T3>(),
                args[3].Cast<T4>(),
                args[4].Cast<T5>(),
                args[5].Cast<T6>()
            );
        }
    }

    internal class Method<T1, T2, T3, T4, T5, T6, T7> : AbstractMethod
    {
        public delegate void P(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7);

        private readonly P p = null;

        public Method(Delegate del)
        {
            this.p = (P)del;
        }

        public override void Call(params object[] args)
        {
            this.p(
                args[0].Cast<T1>(),
                args[1].Cast<T2>(),
                args[2].Cast<T3>(),
                args[3].Cast<T4>(),
                args[4].Cast<T5>(),
                args[5].Cast<T6>(),
                args[6].Cast<T7>()
            );
        }
    }

    internal class Method<T1, T2, T3, T4, T5, T6, T7, T8> : AbstractMethod
    {
        public delegate void P(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8);

        private readonly P p = null;

        public Method(Delegate del)
        {
            this.p = (P)del;
        }

        public override void Call(params object[] args)
        {
            this.p(
                args[0].Cast<T1>(),
                args[1].Cast<T2>(),
                args[2].Cast<T3>(),
                args[3].Cast<T4>(),
                args[4].Cast<T5>(),
                args[5].Cast<T6>(),
                args[6].Cast<T7>(),
                args[7].Cast<T8>()
            );
        }
    }

    internal class MethodRet<TR> : AbstractMethodRet
    {
        public delegate TR P();

        private readonly P p = null;

        public MethodRet(Delegate del)
        {
            this.p = (P)del;
        }

        public override object CallWithReturn(params object[] args)
        {
            return this.p();
        }
    }

    internal class MethodRet<TR, T1> : AbstractMethodRet
    {
        public delegate TR P(T1 t1);

        private readonly P p = null;

        public MethodRet(Delegate del)
        {
            this.p = (P)del;
        }

        public override object CallWithReturn(params object[] args)
        {
            return this.p(
                args[0].Cast<T1>()
            );
        }
    }

    internal class MethodRet<TR, T1, T2> : AbstractMethodRet
    {
        public delegate TR P(T1 t1, T2 t2);

        private readonly P p = null;

        public MethodRet(Delegate del)
        {
            this.p = (P)del;
        }

        public override object CallWithReturn(params object[] args)
        {
            return this.p(
                args[0].Cast<T1>(),
                args[1].Cast<T2>()
            );
        }
    }

    internal class MethodRet<TR, T1, T2, T3> : AbstractMethodRet
    {
        public delegate TR P(T1 t1, T2 t2, T3 t3);

        private readonly P p = null;

        public MethodRet(Delegate del)
        {
            this.p = (P)del;
        }

        public override object CallWithReturn(params object[] args)
        {
            return this.p(
                args[0].Cast<T1>(),
                args[1].Cast<T2>(),
                args[2].Cast<T3>()
            );
        }
    }

    internal class MethodRet<TR, T1, T2, T3, T4> : AbstractMethodRet
    {
        public delegate TR P(T1 t1, T2 t2, T3 t3, T4 t4);

        private readonly P p = null;

        public MethodRet(Delegate del)
        {
            this.p = (P)del;
        }

        public override object CallWithReturn(params object[] args)
        {
            return this.p(
                args[0].Cast<T1>(),
                args[1].Cast<T2>(),
                args[2].Cast<T3>(),
                args[3].Cast<T4>()
            );
        }
    }

    internal class MethodRet<TR, T1, T2, T3, T4, T5> : AbstractMethodRet
    {
        public delegate TR P(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5);

        private readonly P p = null;

        public MethodRet(Delegate del)
        {
            this.p = (P)del;
        }

        public override object CallWithReturn(params object[] args)
        {
            return this.p(
                args[0].Cast<T1>(),
                args[1].Cast<T2>(),
                args[2].Cast<T3>(),
                args[3].Cast<T4>(),
                args[4].Cast<T5>()
            );
        }
    }

    internal class MethodRet<TR, T1, T2, T3, T4, T5, T6> : AbstractMethodRet
    {
        public delegate TR P(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6);

        private readonly P p = null;

        public MethodRet(Delegate del)
        {
            this.p = (P)del;
        }

        public override object CallWithReturn(params object[] args)
        {
            return this.p(
                args[0].Cast<T1>(),
                args[1].Cast<T2>(),
                args[2].Cast<T3>(),
                args[3].Cast<T4>(),
                args[4].Cast<T5>(),
                args[5].Cast<T6>()
            );
        }
    }

    internal class MethodRet<TR, T1, T2, T3, T4, T5, T6, T7> : AbstractMethodRet
    {
        public delegate TR P(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7);

        private readonly P p = null;

        public MethodRet(Delegate del)
        {
            this.p = (P)del;
        }

        public override object CallWithReturn(params object[] args)
        {
            return this.p(
                args[0].Cast<T1>(),
                args[1].Cast<T2>(),
                args[2].Cast<T3>(),
                args[3].Cast<T4>(),
                args[4].Cast<T5>(),
                args[5].Cast<T6>(),
                args[6].Cast<T7>()
            );
        }
    }

    internal class MethodRet<TR, T1, T2, T3, T4, T5, T6, T7, T8> : AbstractMethodRet
    {
        public delegate TR P(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8);

        private readonly P p = null;

        public MethodRet(Delegate del)
        {
            this.p = (P)del;
        }

        public override object CallWithReturn(params object[] args)
        {
            return this.p(
                args[0].Cast<T1>(),
                args[1].Cast<T2>(),
                args[2].Cast<T3>(),
                args[3].Cast<T4>(),
                args[4].Cast<T5>(),
                args[5].Cast<T6>(),
                args[6].Cast<T7>(),
                args[7].Cast<T8>()
            );
        }
    }

    internal static class ObjectExtension
    {
        public static T Cast<T>(this object obj)
        {
            // Ignore "Type" as parameter type.
            if (typeof(T) == typeof(Type))
            {
                return (T)obj;
            }

            return (T)Convert.ChangeType(obj, typeof(T));
        }
    }
}
