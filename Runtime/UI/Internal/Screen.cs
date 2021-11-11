/*
 * This file is part of Taichi project.
 *
 * (c) MuGuangyi <muguangyi@hotmail.com>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 */

using System;
using System.Collections.Generic;

namespace Taichi.UI.Internal
{
    internal sealed class Screen : IScreen, IDisposable
    {
        private readonly IViewProvider provider = null;
        private readonly Dictionary<string, Window> windows = new Dictionary<string, Window>();

        public Screen(string name, IViewProvider provider)
        {
            this.Name = name;
            this.provider = provider;
        }

        public string Name { get; } = null;

        public void Dispose()
        {
        }

        public void OpenWindow(string window, Type windowView = null)
        {
            if (!this.windows.TryGetValue(window, out Window w))
            {
                this.windows.Add(window, w = (Window)Activator.CreateInstance(windowView ?? typeof(EmptyWindow)));
                w.Name = window;
                w.Provider = this.provider;
            }

            w.OpenAsync();
        }

        public void CloseWindow(string window)
        {
            if (this.windows.TryGetValue(window, out Window w))
            {
                w.Close();
                this.windows.Remove(window);
            }
        }
    }
}
