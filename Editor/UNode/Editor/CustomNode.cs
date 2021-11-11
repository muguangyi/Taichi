/*
 * This file is part of Taichi project.
 *
 * (c) MuGuangyi <muguangyi@hotmail.com>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 */

using System;
using UnityEngine;

namespace Taichi.UNode.Editor
{
    class CustomNode
    {
        private readonly Type type = null;

        public CustomNode(Type type)
        {
            this.type = type;
            var attrs = type.GetCustomAttributes(typeof(NodeInterfaceAttribute), false);
            var attr = attrs[0] as NodeInterfaceAttribute;
            this.Tag = attr.Category + attr.Tag;
        }

        public Node CreateInstance()
        {
            return (Node)ScriptableObject.CreateInstance(this.type);
        }

        public string Tag { get; } = null;
    }
}