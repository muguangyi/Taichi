/*
 * This file is part of Taichi project.
 *
 * (c) MuGuangyi <muguangyi@hotmail.com>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 */

using UnityEditor;

namespace Taichi.Asset.Editor.Internal
{
    internal class AssetNode : Node<AssetNode>
    {
        protected readonly Procedure procedure = null;
        private bool setuped = false;

        public AssetNode(Procedure procedure, AssetManifest.AssetInfo info)
        {
            this.procedure = procedure;
            this.Info = info;
        }

        public AssetManifest.AssetInfo Info { get; } = null;
        public BundleNode Bundle { get; set; } = null;
        public bool IsLoadable => !string.IsNullOrEmpty(this.Info.Name);

        public void Setup()
        {
            if (this.setuped)
            {
                return;
            }

            OnSetup();

            this.setuped = true;

            foreach (var n in this.Depends)
            {
                n.Setup();
            }
        }

        protected virtual void OnSetup()
        {
            var dependencies = AssetDatabase.GetDependencies(this.Info.Path, false);
            foreach (var file in dependencies)
            {
                if (!IsIgnoreFile(file))
                {
                    Depend(this.procedure.FindAsset(file));
                }
            }
        }

        internal static bool IsIgnoreFile(string file)
        {
            return file.EndsWith(".cs", true, null) ||
                   file.EndsWith(".js", true, null) ||
                   file.EndsWith(".dll", true, null) ||
                   file.EndsWith("LightingData.asset", true, null) ||
                   file.EndsWith(".tpsheet", true, null);
        }
    }
}
