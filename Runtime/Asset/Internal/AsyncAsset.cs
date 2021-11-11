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
using UnityEngine.SceneManagement;

namespace Taichi.Asset.Internal
{
    internal sealed class AsyncResourceAsset : Async<IAsset>
    {
        private readonly ResourceRequest request = null;

        public AsyncResourceAsset(ResourceRequest request)
        {
            this.request = request;
        }

        protected override AsyncState OnUpdate()
        {
            if (this.request.isDone)
            {
                SetResult(new Asset(this.request.asset));
                return AsyncState.Succeed;
            }

            return AsyncState.Running;
        }
    }

    internal sealed class AsyncBundleCreateAsset : Async.Async
    {
        private readonly AssetBundleCreateRequest request = null;

        public AsyncBundleCreateAsset(AssetBundleCreateRequest request)
        {
            this.request = request;
        }

        protected override AsyncState OnUpdate()
        {
            if (this.request.isDone)
            {
                SetResult(this.request.assetBundle);
                return AsyncState.Succeed;
            }

            return AsyncState.Running;
        }
    }

    internal sealed class AsyncWaitPackage : Async.Async
    {
        private readonly AssetPackage package = null;

        public AsyncWaitPackage(AssetPackage package)
        {
            this.package = package;
            this.package.Retain();
        }

        protected override AsyncState OnUpdate()
        {
            if (this.package.IsOpened)
            {
                this.package.Release();
                return AsyncState.Succeed;
            }

            return AsyncState.Running;
        }
    }

    internal sealed class AsyncBundleAsset : Async<IAsset>
    {
        private readonly BundlePackage package = null;
        private readonly string asset = null;
        private readonly Type type = null;
        private AsyncOperation request = null;

        public AsyncBundleAsset(BundlePackage package, string asset, Type type)
        {
            this.package = package;
            this.asset = asset;
            this.type = type;

            this.package.Retain();
        }

        protected override bool OnStart()
        {
            this.request = this.package.LoadAssetAsync(this.asset, this.type);
            return true;
        }

        protected override AsyncState OnUpdate()
        {
            if (this.request.isDone)
            {
                if (this.type == typeof(Scene))
                {
                    SetResult(new SceneAsset(SceneManager.GetSceneByName(this.asset), this.package));
                }
                else if (this.request is AssetBundleRequest req)
                {
                    SetResult(new Asset(req.asset, this.package));
                }

                this.package.Release();
                return AsyncState.Succeed;
            }

            return AsyncState.Running;
        }
    }

    internal sealed class AsyncSheetBundleAsset : Async<IAsset>
    {
        private readonly SheetPackage package = null;
        private readonly string asset = null;
        private readonly Type type = null;
        private AssetBundleRequest request = null;
        private IAsset result = null;

        public AsyncSheetBundleAsset(SheetPackage package, string asset, Type type)
        {
            this.package = package;
            this.asset = asset;
            this.type = type;

            this.package.Retain();
        }

        protected override bool OnStart()
        {
            this.request = this.package.LoadSubAssetsAsync(asset, type);
            return true;
        }

        protected override AsyncState OnUpdate()
        {
            if (this.request.isDone)
            {
                SetResult(this.result = new Asset(this.request.allAssets?[0], this.package));

                this.package.Release();
                return AsyncState.Succeed;
            }

            return AsyncState.Running;
        }
    }

    internal sealed class AsyncEditorSceneAsset : Async<IAsset>
    {
        private readonly AsyncOperation request = null;
        private readonly string name = null;

        public AsyncEditorSceneAsset(AsyncOperation request, string name)
        {
            this.request = request;
            this.name = name;
        }

        protected override AsyncState OnUpdate()
        {
            if (this.request.isDone)
            {
                SetResult(new SceneAsset(SceneManager.GetSceneByName(this.name)));

                return AsyncState.Succeed;
            }

            return AsyncState.Running;
        }
    }
}
