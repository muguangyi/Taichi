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
    public class ModifiableNode : ValueNode
    {
        private ICommand command = null;

        public ModifiableNode(IModifiable modifier, string name) : base(modifier, name)
        {
            modifier.NotifyChanged += OnNotifyChanged;
        }

        public override void Notify(object sender, EventArgs args)
        {
            if (!TryExecuteCommand(args))
            {
                base.Notify(sender, args);
            }
        }

        private bool TryExecuteCommand(EventArgs args)
        {
            if (args is ValueChangedArgs vargs)
            {
                if (this.command != null)
                {
                    this.command.Execute(vargs.Value);
                    return true;
                }

                var t = this.Target.GetType();
                var p = t.GetProperty(this.Name);
                if (typeof(ICommand).IsAssignableFrom(p.PropertyType))
                {
                    this.command = (ICommand)p.GetGetMethod().Invoke(this.Target, null);
                    this.command.Execute(vargs.Value);
                    return true;
                }
            }

            return false;
        }

        private void OnNotifyChanged(object sender, EventArgs args)
        {
            if (args is ValueChangedArgs pargs && pargs.Target == this.Name)
            {
                RaiseNotifyChanged(sender, args);
            }
        }
    }
}
