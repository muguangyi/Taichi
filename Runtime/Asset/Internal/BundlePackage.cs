/*
 * This file is part of Taichi project.
 *
 * (c) MuGuangyi <muguangyi@hotmail.com>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 */

using Taichi.Async;
using System;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Taichi.Asset.Internal
{
    internal class BundlePackage : AssetPackage
    {
        private readonly string name = string.Empty;
        private readonly string path = string.Empty;
        private AssetBundleCreateRequest request = null;
        protected AssetBundle bundle = null;

        public BundlePackage(string name, PackageDepot depot) : base(depot)
        {
            this.name = name;
            this.path = name.ToLower();
        }

        public override void Open()
        {
            if (this.state == PackageState.Opened)
            {
                return;
            }

            var dependencies = this.depot.FindDependencies(this.name);
            if (dependencies != null)
            {
                for (var i = 0; i < dependencies.Length; ++i)
                {
                    if (this.depot.FetchPackage(dependencies[i], out AssetPackage p))
                    {
                        Refer(p).Open();
                    }
                }
            }

            // Force to finish the request and use the result directly if it has been started.
            // Because you can't call LoadFromFile if the bundle file is loading async.
            this.bundle = this.request != null ? this.request.assetBundle : AssetBundle.LoadFromFile(ComposePath(this.path));

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

            var dependencies = this.depot.FindDependencies(this.name);
            if (dependencies != null)
            {
                for (var i = 0; i < dependencies.Length; ++i)
                {
                    if (this.depot.FetchPackage(dependencies[i], out AssetPackage p))
                    {
                        wait.Wait(Refer(p).OpenAsync());
                    }
                }
            }

            this.request = AssetBundle.LoadFromFileAsync(ComposePath(this.path));
            var async = new AsyncBundleCreateAsset(this.request);
            async.Completed += a => this.bundle = (AssetBundle)a.GetResult();

            wait.Wait(async);

            return wait;
        }

        public override IAsset Load(string asset, Type type)
        {
            if (this.state != PackageState.Opened)
            {
                Open();
            }

            var a = LoadAsset(asset, type);
            return type == typeof(Scene) ? new SceneAsset(a, this) : new Asset(a, this);
        }

        public override IAsync<IAsset> LoadAsync(string asset, Type type)
        {
            if (this.state == PackageState.Invalid)
            {
                OpenAsync();
            }

            var async = new AsyncBundleAsset(this, asset, type);
            async.Wait(new AsyncWaitPackage(this));

            return async;
        }

        protected override void OnDispose()
        {
            this.state = PackageState.Invalid;

            if (this.bundle != null)
            {
                this.bundle.Unload(false);
                this.bundle = null;
            }

            base.OnDispose();
        }

        internal object LoadAsset(string asset, Type type)
        {
            if (type == typeof(Scene))
            {
                return SceneManager.LoadScene(asset, new LoadSceneParameters(LoadSceneMode.Additive));
            }
            else
            {
                return this.bundle.LoadAsset(asset, type);
            }
        }

        internal AsyncOperation LoadAssetAsync(string asset, Type type)
        {
            if (type == typeof(Scene))
            {
                return SceneManager.LoadSceneAsync(asset, new LoadSceneParameters(LoadSceneMode.Additive));
            }
            else
            {
                return this.bundle.LoadAssetAsync(asset, type);
            }
        }

        internal object[] LoadSubAssets(string asset, Type type)
        {
            return this.bundle.LoadAssetWithSubAssets(asset, type);
        }

        internal AssetBundleRequest LoadSubAssetsAsync(string asset, Type type)
        {
            return this.bundle.LoadAssetWithSubAssetsAsync(asset, type);
        }

        private string ComposePath(string path)
        {
            var relativePath = AssetManifest.BundleFolder + path;
            var persistentPath = Path.Combine(Application.persistentDataPath, relativePath);
            if (File.Exists(persistentPath))
            {
                return persistentPath;
            }

            return Path.Combine(Application.streamingAssetsPath, relativePath);
        }
    }
}
