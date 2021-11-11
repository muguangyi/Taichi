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
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.U2D;

namespace Taichi.Asset.Editor.Internal
{
    internal sealed class Procedure
    {
        private readonly Dictionary<string, (string, string)> predefines = new Dictionary<string, (string, string)>();
        private readonly Dictionary<string, AssetNode> assets = new Dictionary<string, AssetNode>();
        private readonly Dictionary<string, BundleNode> bundles = new Dictionary<string, BundleNode>();

        public Procedure()
        {
            AssetDatabase.RemoveUnusedAssetBundleNames();
            var allBundleNames = AssetDatabase.GetAllAssetBundleNames();
            foreach (var bundleName in allBundleNames)
            {
                var split = bundleName.Split('.');
                var variant = split.Length > 1 ? split[1] : null;

                var assetPaths = AssetDatabase.GetAssetPathsFromAssetBundle(bundleName);
                foreach (var assetPath in assetPaths)
                {
                    this.predefines.Add(assetPath, (bundleName, variant));
                }
            }
        }

        public void Build(string outputFolder, BuildTarget platform, BuildAssetBundleOptions options)
        {
            // Create output folder if it's not exist.
            if (!Directory.Exists(outputFolder))
            {
                Directory.CreateDirectory(outputFolder);
            }

            // Collect build information.
            Collect();

            // Build asset bundles.
            var builds = (from b in this.bundles select b.Value.ToBundleBuild()).ToArray();
            AssetBundleManifest bundleManifest = null;
            try
            {
                bundleManifest = BuildPipeline.BuildAssetBundles(outputFolder, builds, options, platform);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            // Save AssetBundleManifest result file.
            if (bundleManifest != null)
            {
                var resultName = Path.Combine(outputFolder, new DirectoryInfo(outputFolder).Name);
                var bundleManifestName = Path.Combine(outputFolder, nameof(AssetBundleManifest).ToLower());
                File.Copy(resultName, bundleManifestName, true);
                File.Delete(resultName);

                // Delete all .manifest files.
                var files = Directory.GetFiles(outputFolder, "*.manifest");
                foreach (var f in files)
                {
                    File.Delete(f);
                }
            }
        }

        internal AssetNode FindAsset(string path)
        {
            if (!this.assets.TryGetValue(path, out AssetNode node))
            {
                this.assets.Add(path, node = CreateAssetNode(new AssetManifest.AssetInfo
                {
                    Name = null, // NOTE: Set name to null to indiate the asset is unloadable.
                    Path = path,
                    Type = AssetDatabase.GetMainAssetTypeAtPath(path).Name,
                    Package = null,
                }));
            }

            return node;
        }

        private void Collect()
        {
            var dict = AssetManifest.Fetch().Refresh();

            foreach (var i in dict)
            {
                if (this.assets.ContainsKey(i.Value.Path))
                {
                    Debug.LogWarning($"Duplicated asset: <{i.Value.Path}>");
                    continue;
                }

                this.assets.Add(i.Value.Path, CreateAssetNode(i.Value));
            }

            var nodes = this.assets.Values.ToArray();
            for (var i = 0; i < nodes.Length; ++i)
            {
                var n = nodes[i];
                n.Setup();
            }

            foreach (var i in this.assets)
            {
                ConnectBundle(i.Value);
            }

            foreach (var i in this.bundles)
            {
                i.Value.Setup();
            }

            do
            {
                var list = (from i in this.bundles where i.Value.IsReducible() select i.Value).ToArray();
                foreach (var b in list)
                {
                    if (b.Reduce())
                    {
                        this.bundles.Remove(b.Name);
                    }
                }
            }
            while (ReduceBySameReferrer() > 0);

            var manifest = ScriptableObject.CreateInstance<AssetManifest>();
            var assets = this.assets.Values.ToArray();
            foreach (var a in assets)
            {
                if (!a.IsLoadable)
                {
                    continue;
                }

                // TODO: Handle variant.
                manifest.Add(!string.IsNullOrEmpty(a.Info.Package) ? a.Info : new AssetManifest.AssetInfo
                {
                    Name = a.Info.Name,
                    Path = a.Info.Path,
                    Type = a.Info.Type,
                    Package = a.Bundle.Name,
                });
            }

            AssetManifest.Flush(manifest);
        }

        private void ConnectBundle(AssetNode asset)
        {
            if (!string.IsNullOrEmpty(asset.Info.Package))
            {
                return;
            }

            if (TryGenBundleInfo(asset, out string name, out string variant))
            {
                var id = GenBundleID(name, variant);
                if (!this.bundles.TryGetValue(id, out BundleNode node))
                {
                    this.bundles.Add(id, new BundleNode(id, name, variant, asset));
                }
                else
                {
                    node.Include(asset);
                }
            }
        }

        private bool TryGenBundleInfo(AssetNode asset, out string name, out string variant)
        {
            name = null;
            variant = null;

            // Handle predefined bundles first.
            if (this.predefines.TryGetValue(asset.Info.Path, out (string, string) definition))
            {
                variant = definition.Item2;
                name = string.IsNullOrEmpty(variant) ? definition.Item1 : $"{definition.Item1}.{variant}";
            }
            else
            {
                switch (asset.Info.Type)
                {
                    case nameof(AssetManifest):
                        name = asset.Info.Name;
                        break;
                    case nameof(Shader):
                    case nameof(ShaderVariantCollection):
                        name = AssetManifest.ShaderBundleName;
                        break;
                    default:
                        var guid = AssetDatabase.AssetPathToGUID(asset.Info.Path);
                        name = $"{Path.GetFileNameWithoutExtension(asset.Info.Path).Replace(" ", "_")}_{guid}";
                        break;
                }
            }

            return true;
        }

        private string GenBundleID(string name, string variant)
        {
            return string.IsNullOrEmpty(variant) ? name : $"{name}.{variant}";
        }

        private int ReduceBySameReferrer()
        {
            int reduceSum = 0;

            var moveNext = true;
            while (moveNext)
            {
                moveNext = false;

                var groups = from b in this.bundles where b.Value.Combinable group b.Value by b.Value.ReferrerHash();
                foreach (var g in groups)
                {
                    var group = g.ToArray();
                    if (group.Length > 1 && group[0].Refers.Count > 0)
                    {
                        for (var i = 1; i < group.Length; ++i)
                        {
                            var bundle = group[i];
                            if (group[0].Combine(bundle))
                            {
                                this.bundles.Remove(bundle.ID);
                            }
                        }

                        moveNext = true;
                        ++reduceSum;
                        break;
                    }
                }
            }

            return reduceSum;
        }

        private AssetNode CreateAssetNode(AssetManifest.AssetInfo info)
        {
            switch (info.Type)
            {
            case nameof(Sprite):
                return new SpriteNode(this, info);   
            default:
                return new AssetNode(this, info);
            }
        }
    }
}
