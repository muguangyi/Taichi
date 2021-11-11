/*
 * This file is part of Taichi project.
 *
 * (c) MuGuangyi <muguangyi@hotmail.com>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 */

using System;

namespace Taichi.Gameplay
{
    public enum AccessMode
    {
        Get = 0x1,
        Set = 0x2,
    }

    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property)]
    public sealed class AccessibleAttribute : Attribute
    { }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public sealed class RequireValueAttribute : Attribute
    {
        public RequireValueAttribute(string name, AccessMode mode = AccessMode.Get)
        {
            this.Name = name;
            this.Mode = mode;
        }

        public string Name { get; } = null;
        public AccessMode Mode { get; } = AccessMode.Get;
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public sealed class OnChangeValueAttribute : Attribute
    {
        public OnChangeValueAttribute(string name)
        {
            this.Name = name;
        }

        public string Name { get; } = string.Empty;
    }
}
