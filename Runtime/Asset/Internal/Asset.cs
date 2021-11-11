/*
 * This file is part of Taichi project.
 *
 * (c) MuGuangyi <muguangyi@hotmail.com>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 */

using System;

namespace Taichi.Asset.Internal
{
    internal class Asset : IAsset
    {
        protected object asset = null;
        private WeakReference weakObject = null;
        private RefObject refObject = null;

        public Asset(object asset, RefObject refObject = null)
        {
            this.asset = asset;
            this.weakObject = new WeakReference(asset);
            this.refObject = refObject;
            this.refObject?.Retain();
        }

        public event Action<IAsset> OnDestroy;

        public bool IsDead => !this.weakObject.IsAlive || this.asset == null || this.asset.Equals(null);

        public void Dispose()
        {
            this.OnDestroy?.Invoke(this);
            OnDispose();

            this.refObject?.Release();
            this.refObject = null;
            this.weakObject = null;
            this.asset = null;
        }

        public T Cast<T>()
        {
            return this.asset != null ? (T)this.asset : default;
        }

        protected virtual void OnDispose()
        { }
    }
}
