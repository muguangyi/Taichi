/*
 * This file is part of Taichi project.
 *
 * (c) MuGuangyi <muguangyi@hotmail.com>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 */

using LunarConsolePlugin;
using UnityEngine;

namespace Taichi.Console
{
    public sealed class AppConsole : IAppConsole
    {
        private GameObject container = null;
        private LunarConsole console = null;

        private void OnResolve()
        {
            this.container = new GameObject("AppConsole");
            this.console = this.container.AddComponent<LunarConsole>();
            Object.DontDestroyOnLoad(this.container);
        }
    }
}
