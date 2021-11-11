/*
 * This file is part of Taichi project.
 *
 * (c) MuGuangyi <muguangyi@hotmail.com>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 */

using Unity.Entities;

namespace Taichi.Gameplay.Internal
{
    internal class UEntityNode : ViewableNode
    {
        private Entity entity = default;

        public UEntityNode(Viewable container) : base(container)
        { }

        public override object View => this.entity;

        internal Entity Entity
        {
            set
            {
                entity = value;
            }
        }
    }
}
