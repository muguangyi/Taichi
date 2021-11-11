/*
 * This file is part of Taichi project.
 *
 * (c) MuGuangyi <muguangyi@hotmail.com>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 */

using Taichi.Async;
using Taichi.Gameplay.Internal;
using Taichi.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using Taichi.Binding;

namespace Taichi.Gameplay
{
    public class Atom : IAtom
    {
        private static uint lastIndex = 0;

        private readonly Dictionary<string, Trait> traits = new Dictionary<string, Trait>();

        private readonly Dictionary<string, (AbstractGetter, WeakReference)> getters = new Dictionary<string, (AbstractGetter, WeakReference)>();
        private readonly Dictionary<string, (AbstractSetter, WeakReference)> setters = new Dictionary<string, (AbstractSetter, WeakReference)>();
        private readonly Dictionary<string, (AbstractMethod, WeakReference)> methods = new Dictionary<string, (AbstractMethod, WeakReference)>();
        private readonly Dictionary<string, LinkedList<(ValueChangeHandler, WeakReference)>> handlers = new Dictionary<string, LinkedList<(ValueChangeHandler, WeakReference)>>();

        public Atom(uint index = 0)
        {
            this.InstanceID = index != 0 ? index : ++lastIndex;
        }

        public uint InstanceID { get; } = 0;

        public virtual void Dispose()
        { }

        public bool AddTrait(Type traitType)
        {
            if (!traitType.IsSubclassOf(typeof(Trait)))
            {
                return false;
            }

            var name = traitType.Name;
            if (this.traits.ContainsKey(name))
            {
                return false;
            }

            var trait = (Trait)Activator.CreateInstance(traitType);
            trait.ValueChanged += OnValueChanged;
            trait.Init();
            this.traits.Add(name, trait);

            return true;
        }

        public bool RemoveTrait(Type traitType)
        {
            var name = traitType.Name;
            if (this.traits.TryGetValue(name, out Trait trait))
            {
                trait.ValueChanged -= OnValueChanged;

                // Remove get/set/method delegates of the property.
                var keys = this.getters.Keys.ToArray();
                foreach (var k in keys)
                {
                    var v = this.getters[k];
                    if (v.Item2.Target == trait)
                    {
                        this.getters.Remove(k);
                    }
                }
                keys = this.setters.Keys.ToArray();
                foreach (var k in keys)
                {
                    var v = this.setters[k];
                    if (v.Item2.Target == trait)
                    {
                        this.setters.Remove(k);
                    }
                }
                keys = this.methods.Keys.ToArray();
                foreach (var k in keys)
                {
                    var v = this.methods[k];
                    if (v.Item2.Target == trait)
                    {
                        this.methods.Remove(k);
                    }
                }

                trait.Destroy();
                return this.traits.Remove(name);
            }

            return false;
        }

        public bool AddValueChange(string name, ValueChangeHandler handler, object target = null)
        {
            if (!this.handlers.TryGetValue(name, out LinkedList<(ValueChangeHandler, WeakReference)> list))
            {
                this.handlers.Add(name, list = new LinkedList<(ValueChangeHandler, WeakReference)>());
            }

            list.AddLast((handler, target != null ? new WeakReference(target) : null));
            return true;
        }

        public bool RemoveValueChange(string name, ValueChangeHandler handler, object target = null)
        {
            if (this.handlers.TryGetValue(name, out LinkedList<(ValueChangeHandler, WeakReference)> list))
            {
                var n = list.First;
                while (n != null)
                {
                    var c = n;
                    n = n.Next;

                    if (c.Value.Item1 == handler)
                    {
                        list.Remove(c);
                    }
                }
            }

            return false;
        }

        public void ClearTargetHandlers(object target)
        {
            if (target == null)
            {
                return;
            }

            foreach (var list in this.handlers.Values)
            {
                var n = list.First;
                while (n != null)
                {
                    var c = n;
                    n = n.Next;

                    if (c.Value.Item2 == target)
                    {
                        list.Remove(c);
                    }
                }
            }
        }

        public virtual object Get(string name)
        {
            if (TryGetGetter(name, out AbstractGetter g))
            {
                return g.Call();
            }

            return null;
        }

        public virtual TV Get<TV>(string name)
        {
            var obj = Get(name);
            return obj != null ? (TV)obj : default;
        }

        public virtual void Set(string name, object value)
        {
            if (TryGetSetter(name, out AbstractSetter s))
            {
                s.Call(value);
            }
        }

        public virtual void Set<TV>(string name, TV value)
        {
            Set(name, (object)value);
        }

        public virtual void Call(string method, params object[] args)
        {
            if (TryGetMethod(method, out AbstractMethod m))
            {
                m.Call(args);
            }
        }

        public virtual IAsync<object> CallWithReturn(string method, params object[] args)
        {
            if (TryGetMethod(method, out AbstractMethod m))
            {
                return new AsyncResult<object>(m.CallWithReturn(method, args));
            }

            return AsyncResult<object>.Null;
        }

        public virtual void Call(string method)
        {
            if (TryGetMethod(method, out AbstractMethod m))
            {
                m.Call();
            }
        }

        public virtual void Call<T1>(string method, T1 t1)
        {
            if (TryGetMethod(method, out AbstractMethod m))
            {
                m.Call(t1);
            }
        }

        public virtual void Call<T1, T2>(string method, T1 t1, T2 t2)
        {
            if (TryGetMethod(method, out AbstractMethod m))
            {
                m.Call(t1, t2);
            }
        }

        public virtual void Call<T1, T2, T3>(string method, T1 t1, T2 t2, T3 t3)
        {
            if (TryGetMethod(method, out AbstractMethod m))
            {
                m.Call(t1, t2, t3);
            }
        }

        public virtual void Call<T1, T2, T3, T4>(string method, T1 t1, T2 t2, T3 t3, T4 t4)
        {
            if (TryGetMethod(method, out AbstractMethod m))
            {
                m.Call(t1, t2, t3, t4);
            }
        }

        public virtual void Call<T1, T2, T3, T4, T5>(string method, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5)
        {
            if (TryGetMethod(method, out AbstractMethod m))
            {
                m.Call(t1, t2, t3, t4, t5);
            }
        }

        public virtual void Call<T1, T2, T3, T4, T5, T6>(string method, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6)
        {
            if (TryGetMethod(method, out AbstractMethod m))
            {
                m.Call(t1, t2, t3, t4, t5, t6);
            }
        }

        public virtual void Call<T1, T2, T3, T4, T5, T6, T7>(string method, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7)
        {
            if (TryGetMethod(method, out AbstractMethod m))
            {
                m.Call(t1, t2, t3, t4, t5, t6, t7);
            }
        }

        public virtual void Call<T1, T2, T3, T4, T5, T6, T7, T8>(string method, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8)
        {
            if (TryGetMethod(method, out AbstractMethod m))
            {
                m.Call(t1, t2, t3, t4, t5, t6, t7, t8);
            }
        }

        public virtual IAsync<TR> Call<TR>(string method)
        {
            if (TryGetMethod(method, out AbstractMethod m))
            {
                return new AsyncResult<TR>((TR)m.CallWithReturn());
            }

            return AsyncResult<TR>.Null;
        }

        public virtual IAsync<TR> Call<TR, T1>(string method, T1 t1)
        {
            if (TryGetMethod(method, out AbstractMethod m))
            {
                return new AsyncResult<TR>((TR)m.CallWithReturn(t1));
            }

            return AsyncResult<TR>.Null;
        }

        public virtual IAsync<TR> Call<TR, T1, T2>(string method, T1 t1, T2 t2)
        {
            if (TryGetMethod(method, out AbstractMethod m))
            {
                return new AsyncResult<TR>((TR)m.CallWithReturn(t1, t2));
            }

            return AsyncResult<TR>.Null;
        }

        public virtual IAsync<TR> Call<TR, T1, T2, T3>(string method, T1 t1, T2 t2, T3 t3)
        {
            if (TryGetMethod(method, out AbstractMethod m))
            {
                return new AsyncResult<TR>((TR)m.CallWithReturn(t1, t2, t3));
            }

            return AsyncResult<TR>.Null;
        }

        public virtual IAsync<TR> Call<TR, T1, T2, T3, T4>(string method, T1 t1, T2 t2, T3 t3, T4 t4)
        {
            if (TryGetMethod(method, out AbstractMethod m))
            {
                return new AsyncResult<TR>((TR)m.CallWithReturn(t1, t2, t3, t4));
            }

            return AsyncResult<TR>.Null;
        }

        public virtual IAsync<TR> Call<TR, T1, T2, T3, T4, T5>(string method, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5)
        {
            if (TryGetMethod(method, out AbstractMethod m))
            {
                return new AsyncResult<TR>((TR)m.CallWithReturn(t1, t2, t3, t4, t5));
            }

            return AsyncResult<TR>.Null;
        }

        public virtual IAsync<TR> Call<TR, T1, T2, T3, T4, T5, T6>(string method, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6)
        {
            if (TryGetMethod(method, out AbstractMethod m))
            {
                return new AsyncResult<TR>((TR)m.CallWithReturn(t1, t2, t3, t4, t5, t6));
            }

            return AsyncResult<TR>.Null;
        }

        public virtual IAsync<TR> Call<TR, T1, T2, T3, T4, T5, T6, T7>(string method, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7)
        {
            if (TryGetMethod(method, out AbstractMethod m))
            {
                return new AsyncResult<TR>((TR)m.CallWithReturn(t1, t2, t3, t4, t5, t6, t7));
            }

            return AsyncResult<TR>.Null;
        }

        public virtual IAsync<TR> Call<TR, T1, T2, T3, T4, T5, T6, T7, T8>(string method, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8)
        {
            if (TryGetMethod(method, out AbstractMethod m))
            {
                return new AsyncResult<TR>((TR)m.CallWithReturn(t1, t2, t3, t4, t5, t6, t7, t8));
            }

            return AsyncResult<TR>.Null;
        }

        internal bool HasValue(string name, AccessMode mode)
        {
            foreach (var i in this.traits)
            {
                if (ContainsValue(name, i.Value, mode) || i.Value.HasValue(name))
                {
                    return true;
                }
            }

            return false;
        }

        private bool IsCompatibleType(Type type, Type targetType)
        {
            return type == targetType || type.IsSubclassOf(targetType) || targetType.IsAssignableFrom(type);
        }

        private bool TryGetGetter(string name, out AbstractGetter g)
        {
            g = null;

            if (this.getters.TryGetValue(name, out (AbstractGetter, WeakReference) target))
            {
                if (target.Item2.IsAlive)
                {
                    g = target.Item1;
                    return true;
                }

                this.getters.Remove(name);
            }

            foreach (var i in this.traits)
            {
                if (TryCollectGetter(name, i.Value, out g))
                {
                    this.getters.Add(name, (g, new WeakReference(i.Value)));
                    return true;
                }
                else if (i.Value.HasValue(name))
                {
                    this.getters.Add(name, (new GenericGetter(i.Value, name), new WeakReference(i.Value)));
                    return true;
                }
            }

            return false;
        }

        private bool TryGetSetter(string name, out AbstractSetter s)
        {
            s = null;

            if (this.setters.TryGetValue(name, out (AbstractSetter, WeakReference) target))
            {
                if (target.Item2.IsAlive)
                {
                    s = target.Item1;
                    return true;
                }

                this.setters.Remove(name);
            }

            foreach (var i in this.traits)
            {
                if (TryCollectSetter(name, i.Value, out s))
                {
                    this.setters.Add(name, (s, new WeakReference(i.Value)));
                    return true;
                }
                else if (i.Value.HasValue(name))
                {
                    this.setters.Add(name, (new GenericSetter(i.Value, name), new WeakReference(i.Value)));
                    return true;
                }
            }

            return false;
        }

        private bool TryGetMethod(string method, out AbstractMethod m)
        {
            m = null;

            if (this.methods.TryGetValue(method, out (AbstractMethod, WeakReference) target))
            {
                if (target.Item2.IsAlive)
                {
                    m = target.Item1;
                    return true;
                }

                this.methods.Remove(method);
            }

            foreach (var i in this.traits)
            {
                if (TryCollectMethod(method, i.Value, out m))
                {
                    this.methods.Add(method, (m, new WeakReference(i.Value)));
                    return true;
                }
                else if (i.Value.HasMethod(method))
                {
                    this.methods.Add(method, (new GenericMethod(i.Value, method), new WeakReference(i.Value)));
                    return true;
                }
            }

            return false;
        }

        private void OnValueChanged(object sender, ValueChangedArgs args)
        {
            if (this.handlers.TryGetValue(args.Target, out LinkedList<(ValueChangeHandler, WeakReference)> list))
            {
                var n = list.First;
                while (n != null)
                {
                    var c = n;
                    n = n.Next;

                    c.Value.Item1?.Invoke(sender, args);
                }
            }
        }

        private static bool ContainsValue(string name, object target, AccessMode mode)
        {
            var t = target.GetType();
            var p = t.GetProperty(name);
            if (p == null)
            {
                return false;
            }

            if ((mode & AccessMode.Get) != 0 && p.GetGetMethod() == null)
            {
                return false;
            }

            if ((mode & AccessMode.Set) != 0 && p.GetSetMethod() == null)
            {
                return false;
            }

            return true;
        }

        private static bool TryCollectGetter(string name, object target, out AbstractGetter getter)
        {
            getter = null;

            if (string.IsNullOrEmpty(name) || target == null)
            {
                return false;
            }

            var t = target.GetType();
            var p = t.GetProperty(name);
            if (p == null || p.GetCustomAttributes(typeof(AccessibleAttribute), true).Length == 0)
            {
                return false;
            }

            var info = p.GetGetMethod();
            if (info == null)
            {
                return false;
            }

            try
            {
                var dtype = typeof(Getter<>.P).MakeGenericType(p.PropertyType);
                var gtype = typeof(Getter<>).MakeGenericType(p.PropertyType);
                var del = Delegate.CreateDelegate(dtype, target, info);
                getter = (AbstractGetter)Activator.CreateInstance(gtype, del);
            }
            catch (Exception ex)
            {
                Log.Error(ex, target);
            }

            return getter != null;
        }

        private static bool TryCollectSetter(string name, object target, out AbstractSetter setter)
        {
            setter = null;

            if (string.IsNullOrEmpty(name) || target == null)
            {
                return false;
            }

            var t = target.GetType();
            var p = t.GetProperty(name);
            if (p == null || p.GetCustomAttributes(typeof(AccessibleAttribute), true).Length == 0)
            {
                return false;
            }

            var info = p.GetSetMethod();
            if (info == null)
            {
                return false;
            }

            try
            {
                var dtype = typeof(Setter<>.P).MakeGenericType(p.PropertyType);
                var stype = typeof(Setter<>).MakeGenericType(p.PropertyType);
                var del = Delegate.CreateDelegate(dtype, target, info);
                setter = (AbstractSetter)Activator.CreateInstance(stype, del);
            }
            catch (Exception ex)
            {
                Log.Error(ex, target);
            }

            return setter != null;
        }

        private static bool TryCollectMethod(string name, object target, out AbstractMethod method)
        {
            method = null;

            if (string.IsNullOrEmpty(name) || target == null)
            {
                return false;
            }

            var t = target.GetType();
            var info = t.GetMethod(name);
            if (info == null || info.GetCustomAttributes(typeof(AccessibleAttribute), true).Length == 0)
            {
                return false;
            }

            var ps = info.GetParameters();
            var list = new List<Type>();
            var returned = info.ReturnType != typeof(void);
            if (returned)
            {
                list.Add(info.ReturnType);
            }
            for (int i = 0; i < ps.Length; i++)
            {
                list.Add(ps[i].ParameterType);
            }
            var args = list.ToArray();

            try
            {
                Type dtype = null;
                Type mtype = null;
                if (returned)
                {
                    switch (args.Length)
                    {
                    case 1:
                        dtype = typeof(MethodRet<>.P).MakeGenericType(args);
                        mtype = typeof(MethodRet<>).MakeGenericType(args);
                        break;
                    case 2:
                        dtype = typeof(MethodRet<,>.P).MakeGenericType(args);
                        mtype = typeof(MethodRet<,>).MakeGenericType(args);
                        break;
                    case 3:
                        dtype = typeof(MethodRet<,,>.P).MakeGenericType(args);
                        mtype = typeof(MethodRet<,,>).MakeGenericType(args);
                        break;
                    case 4:
                        dtype = typeof(MethodRet<,,,>.P).MakeGenericType(args);
                        mtype = typeof(MethodRet<,,,>).MakeGenericType(args);
                        break;
                    case 5:
                        dtype = typeof(MethodRet<,,,,>.P).MakeGenericType(args);
                        mtype = typeof(MethodRet<,,,,>).MakeGenericType(args);
                        break;
                    case 6:
                        dtype = typeof(MethodRet<,,,,,>.P).MakeGenericType(args);
                        mtype = typeof(MethodRet<,,,,,>).MakeGenericType(args);
                        break;
                    case 7:
                        dtype = typeof(MethodRet<,,,,,,>.P).MakeGenericType(args);
                        mtype = typeof(MethodRet<,,,,,,>).MakeGenericType(args);
                        break;
                    case 8:
                        dtype = typeof(MethodRet<,,,,,,,>.P).MakeGenericType(args);
                        mtype = typeof(MethodRet<,,,,,,,>).MakeGenericType(args);
                        break;
                    case 9:
                        dtype = typeof(MethodRet<,,,,,,,,>.P).MakeGenericType(args);
                        mtype = typeof(MethodRet<,,,,,,,,>).MakeGenericType(args);
                        break;
                    }
                }
                else
                {
                    switch (args.Length)
                    {
                    case 0:
                        dtype = typeof(Method.P).MakeGenericType(args);
                        mtype = typeof(Method).MakeGenericType(args);
                        break;
                    case 1:
                        dtype = typeof(Method<>.P).MakeGenericType(args);
                        mtype = typeof(Method<>).MakeGenericType(args);
                        break;
                    case 2:
                        dtype = typeof(Method<,>.P).MakeGenericType(args);
                        mtype = typeof(Method<,>).MakeGenericType(args);
                        break;
                    case 3:
                        dtype = typeof(Method<,,>.P).MakeGenericType(args);
                        mtype = typeof(Method<,,>).MakeGenericType(args);
                        break;
                    case 4:
                        dtype = typeof(Method<,,,>.P).MakeGenericType(args);
                        mtype = typeof(Method<,,,>).MakeGenericType(args);
                        break;
                    case 5:
                        dtype = typeof(Method<,,,,>.P).MakeGenericType(args);
                        mtype = typeof(Method<,,,,>).MakeGenericType(args);
                        break;
                    case 6:
                        dtype = typeof(Method<,,,,,>.P).MakeGenericType(args);
                        mtype = typeof(Method<,,,,,>).MakeGenericType(args);
                        break;
                    case 7:
                        dtype = typeof(Method<,,,,,,>.P).MakeGenericType(args);
                        mtype = typeof(Method<,,,,,,>).MakeGenericType(args);
                        break;
                    case 8:
                        dtype = typeof(Method<,,,,,,,>.P).MakeGenericType(args);
                        mtype = typeof(Method<,,,,,,,>).MakeGenericType(args);
                        break;
                    }
                }


                var del = Delegate.CreateDelegate(dtype, target, info);
                method = (AbstractMethod)Activator.CreateInstance(mtype, del);
            }
            catch (Exception ex)
            {
                Log.Error(ex, target);
            }

            return method != null;
        }
    }
}
