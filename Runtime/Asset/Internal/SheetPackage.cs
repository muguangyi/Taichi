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
using UnityEngine;

namespace Taichi.Asset.Internal
{
    internal sealed class SheetPackage : AssetPackage
    {
        private readonly AssetManifest.AssetInfo info = null;
        private BundlePackage package = null;

        public SheetPackage(AssetManifest.AssetInfo info, PackageDepot depot) : base(depot)
        {
            this.info = info;
        }

        public override void Open()
        {
            if (this.state == PackageState.Opened)
            {
                return;
            }

            if (this.depot.FetchPackage(this.info.Package, out AssetPackage p))
            {
                this.package = (BundlePackage)p;
            }

            Refer(this.package).Open();

            this.state = PackageState.Opened;
        }

        public override IAsync OpenAsync()
        {
            if (this.state == PackageState.Opening || this.state == PackageState.Opened)
            {
                return new AsyncWaitPackage(this);
            }

            this.state = PackageState.Opening;

            var wait = new Async.Async();
            wait.Completed += _ => this.state = PackageState.Opened;

            if (this.depot.FetchPackage(this.info.Package, out AssetPackage p))
            {
                this.package = (BundlePackage)p;
            }

            wait.Wait(Refer(this.package).OpenAsync());

            return wait;
        }

        public override IAsset Load(string asset, Type type)
        {
            Open();

            var subAssets = this.package.LoadSubAssets(asset, type);
            return new Asset(subAssets?[0], this);
        }

        public override IAsync<IAsset> LoadAsync(string asset, Type type)
        {
            if (this.state == PackageState.Invalid)
            {
                OpenAsync();
            }

            var async = new AsyncSheetBundleAsset(this, asset, type);
            async.Wait(new AsyncWaitPackage(this));

            return async;
        }

        internal AssetBundleRequest LoadSubAssetsAsync(string asset, Type type)
        {
            return this.package.LoadSubAssetsAsync(asset, type);
        }
    }
}
