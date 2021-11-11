/*
 * This file is part of Taichi project.
 *
 * (c) MuGuangyi <muguangyi@hotmail.com>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 */

using Taichi.Gameplay.Internal;

namespace Taichi.Gameplay
{
    public sealed class UEntity : Viewable
    {
        public UEntity()
        {
            this.root = new UEntityNode(this);
        }
    }
}
