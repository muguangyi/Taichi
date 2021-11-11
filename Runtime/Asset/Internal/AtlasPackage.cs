/*
 * This file is part of Taichi project.
 *
 * (c) MuGuangyi <muguangyi@hotmail.com>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 */

using System;
using Taichi.Async;
using UnityEngine.U2D;

namespace Taichi.Asset.Internal
{
    internal class AtlasPackage : AssetPackage
    {
        private readonly AssetManifest.AssetInfo info = default;
        private IAsset asset = null;

        public AtlasPackage(AssetManifest.AssetInfo info, PackageDepot depot) : base(depot)
        {
            this.info = info;
        }

        public override void Open()
        {
            if (this.state == PackageState.Opened)
            {
                return;
            }

            if (this.depot.FetchPackage(this.info.Name, out AssetPackage p))
            {
                this.asset = p.Load(this.info.Path, typeof(SpriteAtlas));
            }

            this.state = PackageState.Opened;
        }

        public override IAsset Load(string asset, Type type)
        {
            Open();

            return this.asset != null ? new Asset(this.asset.Cast<SpriteAtlas>().GetSprite(asset)) : null;
        }

        public override IAsync<IAsset> LoadAsync(string asset, Type type)
        {
            return base.LoadAsync(asset, type);
        }

        protected override void OnDispose()
        {
            this.asset?.Dispose();
            this.asset = null;

            base.OnDispose();
        }
    }
}
