/*
 * This file is part of Taichi project.
 *
 * (c) MuGuangyi <muguangyi@hotmail.com>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 */

using System;
using Taichi.Async;

namespace Taichi.Gameplay
{
    public interface IAtom : IDisposable
    {
        uint InstanceID { get; }

        bool AddTrait(Type traitType);
        bool RemoveTrait(Type traitType);

        bool AddValueChange(string name, ValueChangeHandler handler, object target = null);
        bool RemoveValueChange(string name, ValueChangeHandler handler, object target = null);
        void ClearTargetHandlers(object target);

        object Get(string name);
        TV Get<TV>(string name);
        void Set(string name, object value);
        void Set<TV>(string name, TV value);

        void Call(string method, params object[] args);
        IAsync<object> CallWithReturn(string method, params object[] args);

        void Call(string method);
        void Call<T1>(string method, T1 t1);
        void Call<T1, T2>(string method, T1 t1, T2 t2);
        void Call<T1, T2, T3>(string method, T1 t1, T2 t2, T3 t3);
        void Call<T1, T2, T3, T4>(string method, T1 t1, T2 t2, T3 t3, T4 t4);
        void Call<T1, T2, T3, T4, T5>(string method, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5);
        void Call<T1, T2, T3, T4, T5, T6>(string method, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6);
        void Call<T1, T2, T3, T4, T5, T6, T7>(string method, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7);
        void Call<T1, T2, T3, T4, T5, T6, T7, T8>(string method, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8);

        IAsync<TR> Call<TR>(string method);
        IAsync<TR> Call<TR, T1>(string method, T1 t1);
        IAsync<TR> Call<TR, T1, T2>(string method, T1 t1, T2 t2);
        IAsync<TR> Call<TR, T1, T2, T3>(string method, T1 t1, T2 t2, T3 t3);
        IAsync<TR> Call<TR, T1, T2, T3, T4>(string method, T1 t1, T2 t2, T3 t3, T4 t4);
        IAsync<TR> Call<TR, T1, T2, T3, T4, T5>(string method, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5);
        IAsync<TR> Call<TR, T1, T2, T3, T4, T5, T6>(string method, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6);
        IAsync<TR> Call<TR, T1, T2, T3, T4, T5, T6, T7>(string method, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7);
        IAsync<TR> Call<TR, T1, T2, T3, T4, T5, T6, T7, T8>(string method, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8);
    }
}
