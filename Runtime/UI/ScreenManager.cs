/*
 * This file is part of Taichi project.
 *
 * (c) MuGuangyi <muguangyi@hotmail.com>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 */

using System.Collections.Generic;
using Taichi.Foundation;
using Taichi.Asset;
using Taichi.UI.Internal;

namespace Taichi.UI
{
    public sealed class ScreenManager : IScreenManager
    {
        private const string DefaultScreenName = "_default";

        [Resolve] private static IGObjectFactory factory = null;

        private readonly Dictionary<string, Screen> screens = new Dictionary<string, Screen>();
        private IViewProvider provider = null;

        public bool EditorBundleMode
        {
            set
            {
                this.provider.EditorBundleMode = value;
            }
        }

        public IScreen DefaultScreen { get; private set; } = null;

        public IScreen OpenScreen(string screen = null)
        {
            screen = string.IsNullOrEmpty(screen) ? DefaultScreenName : screen;
            if (!this.screens.TryGetValue(screen, out Screen s))
            {
                this.screens.Add(screen, s = new Screen(screen, this.provider));
            }

            return s;
        }

        public void CloseScreen(string screen)
        {
            if (string.IsNullOrEmpty(screen))
            {
                return;
            }

            if (this.screens.TryGetValue(screen, out Screen s))
            {
                s.Dispose();
                this.screens.Remove(screen);
            }
        }

        private void OnResolve()
        {
            this.provider = new GObjectProvidder(factory);

            this.DefaultScreen = OpenScreen();
        }
    }
}
