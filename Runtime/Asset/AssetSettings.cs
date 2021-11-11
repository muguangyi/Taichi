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

namespace Taichi.Asset
{
    public sealed class AssetSettings : EditorSettings<AssetSettings>
    {
        public string ManifestFolder = "Assets/Loadable";
        public string[] AssetFolders = new string[] { "Assets/Loadable" };
        public string OutputFolder = "AssetOutput/";

        public override void OnInit()
        {
            for (var i = 0; i < this.AssetFolders.Length; ++i)
            {
                var folderPath = this.AssetFolders[i];
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }
            }
        }
    }
}
#endif

