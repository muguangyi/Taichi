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
using Taichi.Asset.Editor.Internal;
using UnityEditor;
using UnityEngine;

namespace Taichi.Asset.Editor
{
    public static class AssetBuilder
    {
        public static void Build(string outputFolder, BuildTarget platform, BuildAssetBundleOptions options)
        {
            try
            {
                var p = new Procedure();
                p.Build(outputFolder, platform, options);
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
        }

        public static void BuildAndCopy(string outputFolder, BuildTarget platform, BuildAssetBundleOptions options)
        {
            Build(outputFolder, platform, options);
            CopyToStreaming();
        }

        private static void CopyToStreaming()
        {
            var settings = AssetSettings.Fetch();

            var bundleFileName = nameof(AssetBundleManifest).ToLower();
            var bundle = AssetBundle.LoadFromFile(Path.Combine(settings.OutputFolder, bundleFileName));
            var bundleManifest = bundle.LoadAsset<AssetBundleManifest>(nameof(AssetBundleManifest));
            bundle.Unload(false);

            var assetManifest = AssetManifest.Fetch();
            var targetPath = Path.Combine(Application.streamingAssetsPath, AssetManifest.BundleFolder);
            if (!Directory.Exists(targetPath))
            {
                Directory.CreateDirectory(targetPath);
            }

            foreach (var b in bundleManifest.GetAllAssetBundles())
            {
                File.Copy(Path.Combine(settings.OutputFolder, b), Path.Combine(targetPath, b), true);
            }

            File.Copy(Path.Combine(settings.OutputFolder, bundleFileName), Path.Combine(targetPath, bundleFileName), true);

            // Remove unused bundles.
            var validBundles = new HashSet<string>(bundleManifest.GetAllAssetBundles());
            validBundles.Add(bundleFileName);

            var dir = new DirectoryInfo(targetPath);
            var files = dir.GetFiles();
            foreach (var f in files)
            {
                if (f.Name.EndsWith(".manifest"))
                {
                    continue;
                }

                if (!validBundles.Contains(f.Name))
                {
                    try
                    {
                        Debug.Log($"Delete {f.FullName}");
                        File.Delete(f.FullName);
                        File.Delete($"{f.FullName}.manifest");
                    }
                    catch (Exception ex)
                    {
                        Debug.LogException(ex);
                        throw ex;
                    }
                }
            }

            AssetDatabase.Refresh();
        }
    }
}
