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
using System.Text;
using Taichi.Logger;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.U2D;

namespace Taichi.Asset
{
    [Serializable]
    public sealed class AssetManifest : ScriptableObject, ISerializationCallbackReceiver
    {
        public static readonly string BundleFolder = "Bundle/";
        public static readonly string ShaderBundleName = "shaders";
        public static readonly string BundleExtension = ".bundle";

        [Serializable]
        public class AssetInfo
        {
            public string Name;
            public string Path;
            public string Type;
            public string Package;
        }

        [SerializeField]
        public AssetInfo[] Assets;

        private Dictionary<string, AssetInfo> assets = new Dictionary<string, AssetInfo>();

        public bool Exists(string asset, Type type)
        {
            return this.assets.ContainsKey(ComposeUID(asset, type));
        }

        public AssetInfo NameToInfo(string name, Type type = null)
        {
            if (this.assets.TryGetValue(ComposeUID(name, type), out AssetInfo info))
            {
                return info;
            }

            foreach (var i in this.assets)
            {
                if (i.Key.StartsWith(name))
                {
                    return i.Value;
                }
            }

            return null;
        }

        public void OnBeforeSerialize()
        {
            var arr = this.assets.ToArray();
            this.Assets = new AssetInfo[arr.Length];
            for (var i = 0; i < arr.Length; ++i)
            {
                this.Assets[i] = arr[i].Value;
            }
        }

        public void OnAfterDeserialize()
        {
            this.assets.Clear();
            for (var i = 0; i < this.Assets.Length; ++i)
            {
                var a = this.Assets[i];
                this.assets.Add(ComposeUID(a.Name, a.Type), a);
            }
        }

        internal static string BundleName(string name)
        {
            if (name == nameof(AssetManifest) || name == ShaderBundleName)
            {
                return name;
            }

            return name + BundleExtension;
        }

        private static string NormalizeTypeName(Type type)
        {
            if (type == null)
            {
                return string.Empty;
            }

#if UNITY_EDITOR
            return type == typeof(SceneAsset) ? nameof(Scene) : type.Name;
#else
            return type.Name;
#endif
        }

        private static string ComposeUID(string name, string typeName)
        {
            var sb = new StringBuilder();
            sb.Append(name).Append(".").Append(typeName);

            return sb.ToString();
        }

        private static string ComposeUID(string name, Type type)
        {
            return ComposeUID(name, NormalizeTypeName(type));
        }

#if UNITY_EDITOR
        public void Add(AssetInfo info)
        {
            this.assets.Add(ComposeUID(info.Name, info.Type), info);
        }

        public Dictionary<string, AssetInfo> Refresh()
        {
            this.assets.Clear();

            // Collect assets.
            var settings = AssetSettings.Fetch();
            var guids = AssetDatabase.FindAssets("", settings.AssetFolders);
            for (var i = 0; i < guids.Length; ++i)
            {
                var guid = guids[i];
                var path = AssetDatabase.GUIDToAssetPath(guid);
                // Ignore folder.
                if (!AssetDatabase.IsValidFolder(path))
                {
                    var name = FormatAssetName(path);
                    var type = AssetDatabase.GetMainAssetTypeAtPath(path);
                    this.assets.Add(ComposeUID(name, type), new AssetInfo
                    {
                        Name = name,
                        Path = path,
                        Type = NormalizeTypeName(type),
                        Package = null,
                    });

                    if (type == typeof(Texture2D))
                    {
                        var packagePath = Path.GetDirectoryName(path);
                        var sprites = AssetDatabase.LoadAllAssetsAtPath(path).Where(a => a is Sprite).Cast<Sprite>();
                        foreach (var sprite in sprites)
                        {
                            var spritePath = Path.Combine(packagePath, sprite.name).Replace("\\", "/");
                            var spriteName = FormatAssetName(spritePath);
                            var uid = ComposeUID(spriteName, nameof(Sprite));
                            if (this.assets.ContainsKey(uid))
                            {
                                this.Warn($"Duplicated asset: <{spriteName}>");
                                continue;
                            }

                            this.assets.Add(uid, new AssetInfo
                            {
                                Name = spriteName,
                                Path = sprite.name,
                                Type = nameof(Sprite),
                                Package = name,
                            });
                        }
                    }
                    else if (type == typeof(SpriteAtlas))
                    {
                        var atlas = AssetDatabase.LoadAssetAtPath<SpriteAtlas>(path);
                        var sprites = new Sprite[atlas.spriteCount];
                        atlas.GetSprites(sprites);
                        foreach (var sprite in sprites)
                        {
                            var spriteName = sprite.name.Replace("(Clone)", "");
                            this.assets.Add(ComposeUID(spriteName, nameof(Sprite)), new AssetInfo
                            {
                                Name = spriteName,
                                Path = spriteName,
                                Type = nameof(Sprite),
                                Package = name,
                            });
                        }
                    }
                }
            }

            // Collect scenes.
            var scenes = EditorBuildSettings.scenes;
            for (var i = 0; i < scenes.Length; ++i)
            {
                var scene = scenes[i];
                if (scene.enabled && !string.IsNullOrEmpty(scene.path))
                {
                    var name = Path.GetFileName(scene.path);
                    this.assets.Add(ComposeUID(name, nameof(Scene)), new AssetInfo
                    {
                        Name = name,
                        Path = scene.path,
                        Type = nameof(Scene),
                        Package = null,
                    });
                }
            }

            return this.assets;
        }

        public static AssetManifest Fetch()
        {
            var filePath = GetManifestFilePath();
            var manifest = AssetDatabase.LoadAssetAtPath<AssetManifest>(filePath);
            if (manifest == null)
            {
                manifest = CreateInstance<AssetManifest>();
                AssetDatabase.CreateAsset(manifest, filePath);
            }

            return manifest;
        }

        public static void Flush(AssetManifest manifest)
        {
            var origin = Fetch();
            origin.assets = manifest.assets;
            EditorUtility.SetDirty(origin);
        }

        public static bool IsSelf(AssetInfo info)
        {
            return info.Name == nameof(AssetManifest);
        }

        public static string FormatAssetName(string path)
        {
            var settings = AssetSettings.Fetch();
            var paths = settings.AssetFolders;
            for (var i = 0; i < paths.Length; ++i)
            {
                if (path.StartsWith(paths[i]))
                {
                    path = path.Substring(paths[i].Length + 1);
                    break;
                }
            }

            return path.Substring(0, path.Length - Path.GetExtension(path).Length);
        }

        private static string GetManifestFilePath()
        {
            return $"{AssetSettings.Fetch().ManifestFolder}/{nameof(AssetManifest)}.asset";
        }
#endif
    }
}
