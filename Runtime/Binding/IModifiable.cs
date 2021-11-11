/*
 * This file is part of Taichi project.
 *
 * (c) MuGuangyi <muguangyi@hotmail.com>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 */

namespace Taichi.Binding
{
    public interface IModifiable : INotifyCaller
    {
        void Set(ref object target, object value, string propertyName);
        void Set<T>(ref T target, T value, string propertyName);
    }
}
