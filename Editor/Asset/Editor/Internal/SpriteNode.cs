/*
 * This file is part of Taichi project.
 *
 * (c) MuGuangyi <muguangyi@hotmail.com>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 */

namespace Taichi.Asset.Editor.Internal
{
    internal class SpriteNode : AssetNode
    {
        public SpriteNode(Procedure procedure, AssetManifest.AssetInfo info) : base(procedure, info)
        { }

        protected override void OnSetup()
        {
            var package = this.Info.Package;
            if (!string.IsNullOrEmpty(package))
            {
                var info = AssetManifest.Fetch().NameToInfo(package);
                Depend(this.procedure.FindAsset(info?.Path));
                return;
            }

            base.OnSetup();
        }
    }
}
