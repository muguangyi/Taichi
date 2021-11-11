/*
 * This file is part of Taichi project.
 *
 * (c) MuGuangyi <muguangyi@hotmail.com>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 */

using System;
using Taichi.Foundation;
using Taichi.UI;

namespace Taichi.Gameplay
{
    public class OverlayScreen : Trait, IScreen
    {
        [Resolve] private static IScreenManager manager = null;

        private UI.IScreen screen = null;

        [Accessible]
        public string ScreenName
        {
            get => this.screen?.Name;
            set
            {
                if (this.screen?.Name != value)
                {
                    this.screen = manager.OpenScreen(value);
                }
            }
        }

        [Accessible]
        public void OpenWindow(string window, Type windowView = null)
        {
            this.screen.OpenWindow(window, windowView);
        }

        [Accessible]
        public void CloseWindow(string window)
        {
            this.screen.CloseWindow(window);
        }

        protected override void OnInit()
        {
            base.OnInit();

            this.screen = manager.DefaultScreen;
        }
    }
}
