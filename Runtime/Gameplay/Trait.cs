/*
 * This file is part of Taichi project.
 *
 * (c) MuGuangyi <muguangyi@hotmail.com>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 */

using System;
using Taichi.Binding;

namespace Taichi.Gameplay
{
    public abstract class Trait : ITrait
    {
        public event ValueChangeHandler ValueChanged;

        public virtual bool HasValue(string name)
        {
            return GetType().GetProperty(name) != null;
        }

        public virtual object Get(string name)
        {
            throw new NotImplementedException("Get");
        }

        public virtual void Set(string name, object value)
        {
            throw new NotImplementedException("Set");
        }

        public virtual bool HasMethod(string method)
        {
            return GetType().GetMethod(method) != null;
        }

        public virtual void Call(string method, params object[] args)
        {
            throw new NotImplementedException(method);
        }

        public virtual object CallWithReturn(string method, params object[] args)
        {
            throw new NotImplementedException(method);
        }

        internal void Init()
        {
            OnInit();
        }

        internal void Destroy()
        {
            OnDestroy();
        }

        protected void NotifyValueChanged(object sender, ValueChangedArgs args)
        {
            this.ValueChanged?.Invoke(sender, args);
        }

        protected virtual void OnInit()
        { }

        protected virtual void OnDestroy()
        { }
    }
}
