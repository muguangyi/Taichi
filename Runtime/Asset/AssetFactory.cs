/*
 * This file is part of Taichi project.
 *
 * (c) MuGuangyi <muguangyi@hotmail.com>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 */

using Taichi.Async;
using Taichi.Asset.Internal;
using System;
using System.Collections.Generic;

namespace Taichi.Asset
{
    public sealed class AssetFactory : IAssetFactory
    {
        private readonly List<IAssetDepot> depots = new List<IAssetDepot>();
        private readonly LinkedList<IAsset> assets = new LinkedList<IAsset>();

        public AssetFactory()
        {
            OpenDepot(new ResourceDepot());
            OpenDepot(new DatabaseDepot());
            OpenDepot(new PackageDepot());
        }

        public bool EditorBundleMode
        {
            set
            {
                foreach (var d in this.depots)
                {
                    d.EditorBundleMode = value;
                }
            }
        }

        public IAsset Load(string asset, Type type)
        {
            for (var i = depots.Count - 1; i >= 0; --i)
            {
                if (depots[i].Exists(asset, type))
                {
                    return Watch(depots[i].Load(asset, type));
                }
            }

            return null;
        }

        public IAsync<IAsset> LoadAsync(string asset, Type type)
        {
            for (var i = this.depots.Count - 1; i >= 0; --i)
            {
                if (this.depots[i].Exists(asset, type))
                {
                    var async = this.depots[i].LoadAsync(asset, type);
                    async.Completed += a => Watch((IAsset)a.GetResult());
                    return async;
                }
            }

            return Async<IAsset>.Null;
        }

        public void OpenDepot(IAssetDepot depot)
        {
            this.depots.Add(depot);
        }

        private void OnResolve()
        {
            for (var i = 0; i < this.depots.Count; ++i)
            {
                this.depots[i].Init();
            }
        }

        private void OnUpdate(float deltaTime)
        {
            var n = this.assets.First;
            while (n != null)
            {
                // Only dispose one asset per frame.
                if (n.Value.IsDead)
                {
                    n.Value.Dispose();
                    break;
                }

                n = n.Next;
            }
        }

        private IAsset Watch(IAsset asset)
        {
            asset.OnDestroy += UnWatch;
            return this.assets.AddLast(asset).Value;
        }

        private void UnWatch(IAsset asset)
        {
            asset.OnDestroy -= UnWatch;
            this.assets.Remove(asset);
        }
    }
}
