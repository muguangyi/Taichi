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
    public class ValueChangedArgs : EventArgs
    {
        public ValueChangedArgs(string target, object value = null)
        {
            this.Target = target;
            this.Value = value;
        }

        public string Target { get; private set; } = string.Empty;
        public object Value { get; private set; } = null;
    }
}
