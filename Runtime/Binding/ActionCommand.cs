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
    public class ActionCommand : Command
    {
        private readonly Action action = null;

        public ActionCommand(Action action)
        {
            this.action = action;
        }

        public override void Execute(object value)
        {
            this.action?.Invoke();
        }
    }
}
