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
using System.Linq;
using UnityEditor;
using UnityEngine.SceneManagement;

namespace Taichi.Asset.Editor.Internal
{
    internal sealed class BundleNode : Node<BundleNode>
    {
        private readonly AssetNode mainAsset = null;
        private readonly HashSet<AssetNode> assets = new HashSet<AssetNode>();

        public BundleNode(string id, string name, string variant, AssetNode mainAsset)
        {
            this.ID = id;
            this.Name = AssetManifest.BundleName(name);
            this.Variant = variant;
            this.mainAsset = mainAsset;

            Include(mainAsset);
        }

        public string ID { get; } = string.Empty;
        public string Name { get; } = string.Empty;
        public string Variant { get; } = null;
        public bool Combinable => this.mainAsset.Info.Type != nameof(Scene);

        public void Include(AssetNode asset)
        {
            asset.Bundle = this;
            this.assets.Add(asset);

            if (this.assets.Count > 1 && this.mainAsset.Info.Type == nameof(Scene))
            {
                throw new InvalidOperationException($"can't include <{asset.Info.Name}> into <{this.Name}> scene bundle");
            }
        }

        public void Setup()
        {
            foreach (var a in this.assets)
            {
                foreach (var d in a.Depends)
                {
                    Depend(d.Bundle);
                }
            }
        }

        public bool IsReducible()
        {
            if (this.Refers.Count != 1)
            {
                return false;
            }

            var refer = this.Refers.First();
            if (refer.mainAsset.Info.Type == nameof(Scene) || this.assets.Any(a => a.IsLoadable))
            {
                return false;
            }

            if (refer.Variant != this.Variant)
            {
                return false;
            }

            return true;
        }

        public bool Reduce()
        {
            if (!IsReducible())
            {
                return false;
            }

            var refer = this.Refers.First();
            refer.Break(this);

            var depends = this.Depends.ToArray();
            foreach (var d in depends)
            {
                Break(d);
            }

            foreach (var a in this.assets)
            {
                if (a.IsLoadable)
                {
                    refer.Include(a);
                }
            }
            this.assets.Clear();

            foreach (var d in depends)
            {
                refer.Depend(d);
            }

            return true;
        }

        public bool Combine(BundleNode bundle)
        {
            var refers = bundle.Refers.ToArray();
            foreach (var b in refers)
            {
                b.Break(bundle);
                b.Depend(this);
            }

            var depends = bundle.Depends.ToArray();
            foreach (var b in depends)
            {
                bundle.Break(b);
                Depend(b);
            }

            foreach (var a in bundle.assets)
            {
                Include(a);
            }

            return true;
        }

        public AssetBundleBuild ToBundleBuild()
        {
            return new AssetBundleBuild
            {
                assetBundleName = this.Name,
                assetBundleVariant = this.Variant,
                assetNames = (from a in this.assets select a.Info.Path).ToArray(),
            };
        }
    }
}
