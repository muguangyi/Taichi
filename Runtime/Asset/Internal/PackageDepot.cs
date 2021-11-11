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
using Taichi.Async;
using UnityEngine;
using UnityEngine.U2D;

namespace Taichi.Asset.Internal
{
    internal sealed class PackageDepot : AssetDepot
    {
        private AssetManifest assetManifest = null;
        private AssetBundleManifest bundleManifest = null;
        private readonly Dictionary<string, AssetPackage> packages = new Dictionary<string, AssetPackage>();

        public override bool EditorBundleMode
        {
            protected get => base.EditorBundleMode;

            set
            {
                base.EditorBundleMode = value;
                if (value)
                {
                    InitManifest();
                }
            }
        }

        public override void Init()
        {
            base.Init();

            if (!Application.isEditor)
            {
                InitManifest();
            }
        }

        public override bool Exists(string asset, Type type)
        {
#if UNITY_EDITOR
            return this.EditorBundleMode ? this.assetManifest.Exists(asset, type) : false;
#else
            return this.assetManifest.Exists(asset, type);
#endif
        }

        public override IAsset Load(string asset, Type type)
        {
            var info = this.assetManifest.NameToInfo(asset, type);
            if (FetchPackage(info?.Package, out AssetPackage p))
            {
                return p.Load(info.Path, type);
            }

            return null;
        }

        public override IAsync<IAsset> LoadAsync(string asset, Type type)
        {
            var info = this.assetManifest.NameToInfo(asset, type);
            if (FetchPackage(info?.Package, out AssetPackage p))
            {
                return p.LoadAsync(info.Path, type);
            }

            return Async<IAsset>.Null;
        }

        internal string[] FindDependencies(string name)
        {
            if (name == nameof(AssetManifest) || name == nameof(AssetBundleManifest))
            {
                return null;
            }

            return this.bundleManifest.GetAllDependencies(name);
        }

        internal bool FetchPackage(string name, out AssetPackage p)
        {
            p = null;

            if (string.IsNullOrEmpty(name))
            {
                return false;
            }

            if (!this.packages.TryGetValue(name, out p))
            {
                var info = this.assetManifest.NameToInfo(name);
                switch (info?.Type)
                {
                case nameof(Texture2D):
                    p = new SheetPackage(info, this);
                    break;
                case nameof(SpriteAtlas):
                    p = new AtlasPackage(info, this);
                    break;
                default:
                    p = new BundlePackage(name, this);
                    break;
                }

                this.packages.Add(name, p);
            }

            return true;
        }

        private void InitManifest()
        {
            // Load AssetManifest data from asset bundle.
            var b = new BundlePackage(nameof(AssetManifest), this);
            using (var a = b.Load(nameof(AssetManifest), typeof(AssetManifest)))
            {
                this.assetManifest = a.Cast<AssetManifest>();
            }

            b = new BundlePackage(nameof(AssetBundleManifest), this);
            using (var a = b.Load(nameof(AssetBundleManifest), typeof(AssetBundleManifest)))
            {
                this.bundleManifest = a.Cast<AssetBundleManifest>();
            }
        }
    }
}
