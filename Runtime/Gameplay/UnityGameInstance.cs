/*
 * This file is part of Taichi project.
 *
 * (c) MuGuangyi <muguangyi@hotmail.com>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 */

using System;
using System.Linq;

namespace Taichi.Gameplay
{
    public class UnityGameInstance : GameInstance
    {
        protected override Type[] Contexts => new Type[]
        {
            typeof(GObjectLoader),
            typeof(UEntityLoader),
            typeof(OverlayScreen),
        }
        .Concat(base.Contexts).ToArray();
    }
}