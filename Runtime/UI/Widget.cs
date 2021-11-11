/*
 * This file is part of Taichi project.
 *
 * (c) MuGuangyi <muguangyi@hotmail.com>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 */

using System;

namespace Taichi.UI
{
    public abstract class Widget : View, IWidget
    {
        private readonly Window owner = null;

        public Widget(Window owner)
        {
            this.owner = owner;
        }

        public void OpenWindow(string window, Type windowView = null)
        {
            this.owner.OpenWindow(window, windowView);
        }

        public void CloseWindow(string window)
        {
            this.owner.CloseWindow(window);
        }
    }

    public abstract class Widget<TViewModel> : Widget where TViewModel : ViewModel
    {
        public Widget(Window owner) : base(owner)
        { }

        protected TViewModel ViewModel { get; set; } = null;
    }
}
