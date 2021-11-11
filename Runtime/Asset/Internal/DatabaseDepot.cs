/*
 * This file is part of Taichi project.
 *
 * (c) MuGuangyi <muguangyi@hotmail.com>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 */

using System;
using System.Linq;
using Taichi.Async;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
#endif

namespace Taichi.Asset.Internal
{
    internal sealed class DatabaseDepot : AssetDepot
    {
        private AssetManifest manifest = null;

        public override void Init()
        {
            base.Init();

#if UNITY_EDITOR
            this.manifest = ScriptableObject.CreateInstance<AssetManifest>();
            this.manifest.Refresh();
#endif
        }

        public override bool Exists(string asset, Type type)
        {
#if UNITY_EDITOR
            return this.EditorBundleMode ? false : this.manifest.Exists(asset, type);
#else
            return false;
#endif
        }

        public override IAsset Load(string asset, Type type)
        {
#if UNITY_EDITOR
            var info = this.manifest.NameToInfo(asset, type);
            if (!string.IsNullOrEmpty(info?.Path))
            {
                if (type == typeof(Scene))
                {
                    return new SceneAsset(EditorSceneManager.LoadSceneInPlayMode(info.Path, new LoadSceneParameters(LoadSceneMode.Additive)));
                }
                else if (type == typeof(Sprite))
                {
                    var packInfo = this.manifest.NameToInfo(info.Package);
                    if (packInfo.Type == nameof(Texture2D))
                    {
                        return new Asset(AssetDatabase.LoadAllAssetsAtPath(packInfo.Path).First(a => a is Sprite && a.name == info.Path));
                    }
                }
                else
                {
                    return new Asset(AssetDatabase.LoadAssetAtPath(info.Path, type));
                }
            }
#endif
            return null;
        }

        public override IAsync<IAsset> LoadAsync(string asset, Type type)
        {
#if UNITY_EDITOR
            var info = this.manifest.NameToInfo(asset, type);
            if (!string.IsNullOrEmpty(info?.Path))
            {
                if (type == typeof(Scene))
                {
                    return new AsyncEditorSceneAsset(EditorSceneManager.LoadSceneAsyncInPlayMode(info.Path, new LoadSceneParameters(LoadSceneMode.Additive)), asset);
                }
                else if (type == typeof(Sprite))
                {
                    var packInfo = this.manifest.NameToInfo(info.Package);
                    if (packInfo.Type == nameof(Texture2D))
                    {
                        return Async<IAsset>.WithResult(new Asset(AssetDatabase.LoadAllAssetsAtPath(packInfo.Path).First(a => a is Sprite && a.name == info.Path)));
                    }
                }
                else
                {
                    return Async<IAsset>.WithResult(new Asset(AssetDatabase.LoadAssetAtPath(info.Path, type)));
                }
            }
#endif
            return Async<IAsset>.Null;
        }
    }
}
