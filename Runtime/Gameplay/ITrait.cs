/*
 * This file is part of Taichi project.
 *
 * (c) MuGuangyi <muguangyi@hotmail.com>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 */

using Taichi.Binding;

namespace Taichi.Gameplay
{
    public delegate void ValueChangeHandler(object sender, ValueChangedArgs args);

    public interface ITrait
    {
        event ValueChangeHandler ValueChanged;

        bool HasValue(string name);
        object Get(string name);
        void Set(string name, object value);

        bool HasMethod(string method);
        void Call(string method, params object[] args);
        object CallWithReturn(string method, params object[] args);
    }
}
