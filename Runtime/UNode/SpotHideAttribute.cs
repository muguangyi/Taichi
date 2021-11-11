/*
 * This file is part of Taichi project.
 *
 * (c) MuGuangyi <muguangyi@hotmail.com>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 */

using System;

namespace Taichi.UNode
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public sealed class SpotHideAttribute : Attribute
    {
        public SpotHideAttribute(string name)
        {
            this.Name = name;
        }

        public string Name { get; } = null;
    }
}