/*
 * This file is part of Taichi project.
 *
 * (c) MuGuangyi <muguangyi@hotmail.com>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 */

#if UNITY_EDITOR
using Taichi.Configure;
using System.IO;

namespace Taichi.UI
{
    public sealed class UISettings : EditorSettings<UISettings>
    {
        public string SceneFolder = "Assets/UI/Scenes";
        public string PrefabFolder = "Assets/UI/Prefabs";

        public override void OnInit()
        {
            if (!Directory.Exists(this.SceneFolder))
            {
                Directory.CreateDirectory(this.SceneFolder);
            }

            if (!Directory.Exists(this.PrefabFolder))
            {
                Directory.CreateDirectory(this.PrefabFolder);
            }
        }
    }
}
#endif
