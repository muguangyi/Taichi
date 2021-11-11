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
    public class Overlayer : Actor, IFrontend
    {
        public void OpenWindow(string window, Type windowView = null)
        {
            Call("OpenWindow", window, windowView);
        }

        public void CloseWindow(string window)
        {
            Call("CloseWindow", window);
        }

        protected override void OnInit()
        {
            base.OnInit();

            AddTrait(typeof(OverlayScreen));
        }
    }
}
