/*
 * This file is part of Taichi project.
 *
 * (c) MuGuangyi <muguangyi@hotmail.com>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 */

using System;

namespace Taichi.Binding
{
    public abstract class Modifiable : IModifiable
    {
        public event EventHandler NotifyChanged;

        public void Set(ref object target, object value, string propertyName)
        {
            if (!target.Equals(value))
            {
                target = value;
                this.NotifyChanged?.Invoke(this, new ValueChangedArgs(propertyName, value));
            }
        }

        public void Set<T>(ref T target, T value, string propertyName)
        {
            if (!target.Equals(value))
            {
                target = value;
                this.NotifyChanged?.Invoke(this, new ValueChangedArgs(propertyName, value));
            }
        }
    }
}
