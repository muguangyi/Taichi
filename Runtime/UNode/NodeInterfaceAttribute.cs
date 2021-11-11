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
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class NodeInterfaceAttribute : Attribute
    {
        public NodeInterfaceAttribute(string tag, string category = null)
        {
            this.Tag = tag;
            this.Category = category ?? string.Empty;
        }

        public string Tag { get; } = null;

        public string Category { get; } = null;
    }
}