/*
 * This file is part of Taichi project.
 *
 * (c) MuGuangyi <muguangyi@hotmail.com>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 */

using Taichi.Async;
using Taichi.Asset;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Taichi.UI.Internal
{
    internal sealed class GObjectProvidder : IViewProvider
    {
        private sealed class GObjectView : Async<GameObject>
        {
            public GObjectView(GameObject go)
            {
                SetResult(go);
            }
        }

        private const string RootObjectName = "_UI";

        private readonly IGObjectFactory factory = null;
        private GameObject root = null;

        public GObjectProvidder(IGObjectFactory factory)
        {
            this.factory = factory ?? throw new MissingReferenceException("IGObjectFactory");

            Setup();
        }

        public bool EditorBundleMode { private get; set; } = false;

        public IAsync<GameObject> LoadAsync(string view)
        {
            var path = $"{view}/{view}";
#if UNITY_EDITOR
            return !this.EditorBundleMode ? LoadByDatabase(path) : LoadByFactory(path);
#else
            return LoadByFactory(path);
#endif
        }

        private void Setup()
        {
            this.root = GameObject.Find(RootObjectName) ?? new GameObject(RootObjectName);
            var camera = this.root.GetComponent<Camera>();
            if (camera == null)
            {
                camera = this.root.AddComponent<Camera>();
                camera.orthographic = true;
                camera.clearFlags = CameraClearFlags.Depth;
            }
        }

#if UNITY_EDITOR
        private IAsync<GameObject> LoadByDatabase(string path)
        {
            path = Path.Combine(UISettings.Fetch().PrefabFolder, $"{path}.prefab");
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            var go = Object.Instantiate(prefab);
            go.transform.SetParent(this.root.transform);
            var canvas = go.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            return new GObjectView(go);
        }
#endif

        private IAsync<GameObject> LoadByFactory(string path)
        {
            var req = this.factory.InstantiateAsync(path);
            req.Completed += t =>
            {
                var go = (GameObject)t.GetResult();
                go.transform.SetParent(this.root.transform);
            };

            return req;
        }
    }
}
