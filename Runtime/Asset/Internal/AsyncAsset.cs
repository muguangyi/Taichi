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
        private interface IAssetOperation
        {
            bool IsCompleted { get; }
            IAsset GetAsset();
        }

        private class SyncAssetOperation : IAssetOperation
        {
            private readonly IAsset asset = null;

            public SyncAssetOperation(BundlePackage package, string asset, Type type)
            {
                this.asset = new Asset(package.LoadAsset(asset, type), package);
            }

            public bool IsCompleted => true;

            public IAsset GetAsset()
            {
                return this.asset;
            }
        }

        private class AsyncAssetOperation : IAssetOperation
        {
            private readonly BundlePackage package = null;
            private readonly AsyncOperation operation = null;
            private readonly string asset = null;
            private readonly Type type = null;

            public AsyncAssetOperation(BundlePackage package, string asset, Type type)
            {
                this.package = package;
                this.operation = package.LoadAssetAsync(asset, type);
                this.asset = asset;
                this.type = type;
            }

            public bool IsCompleted => this.operation.isDone;

            public IAsset GetAsset()
            {
                if (this.type == typeof(Scene))
                {
                    return new SceneAsset(SceneManager.GetSceneByName(this.asset), this.package);
                }
                else if (this.operation is AssetBundleRequest req)
                {
                    return new Asset(req.asset, this.package);
                }

                return null;
            }
        }

        private readonly BundlePackage package = null;
        private readonly string asset = null;
        private readonly Type type = null;
        private IAssetOperation request = null;

        public AsyncBundleAsset(BundlePackage package, string asset, Type type)
        {
            this.package = package;
            this.asset = asset;
            this.type = type;

            this.package.Retain();
        }

        protected override bool OnStart()
        {
            if (this.type == typeof(GameObject))
            {
                this.request = new SyncAssetOperation(this.package, this.asset, this.type);
            }
            else
            {
                this.request = new AsyncAssetOperation(this.package, this.asset, this.type);
            }
            return true;
        }

        protected override AsyncState OnUpdate()
        {
            if (this.request.IsCompleted)
            {
                SetResult(this.request.GetAsset());
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
