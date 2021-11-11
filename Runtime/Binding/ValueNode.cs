/*
 * This file is part of Taichi project.
 *
 * (c) MuGuangyi <muguangyi@hotmail.com>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 */

using System;
using System.Reflection;

namespace Taichi.Binding
{
    public class ValueNode : INotifyCaller, INotifyCallee
    {
        private readonly MethodInfo setter = null;

        public ValueNode(object target, string name)
        {
            this.Target = target;
            this.Name = name;

            var t = this.Target.GetType();
            var p = t.GetProperty(this.Name);
            this.setter = p.GetSetMethod();
        }

        public event EventHandler NotifyChanged;

        public virtual void Notify(object sender, EventArgs args)
        {
            // Break the circle.
            if (sender == this.Target)
            {
                return;
            }

            if (args is ValueChangedArgs vargs && this.setter != null)
            {
                this.setter.Invoke(this.Target, new object[] { vargs.Value });
                RaiseNotifyChanged(sender, args);
            }
        }

        protected object Target { get; } = null;
        protected string Name { get; } = string.Empty;

        protected void RaiseNotifyChanged(object sender, EventArgs args)
        {
            this.NotifyChanged?.Invoke(sender, args);
        }
    }
}
