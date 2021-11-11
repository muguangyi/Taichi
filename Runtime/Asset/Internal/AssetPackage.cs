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

namespace Taichi.Asset.Internal
{
    internal abstract class AssetPackage : RefObject
    {
        protected enum PackageState
        {
            Invalid = -1,
            Opening,
            Opened,
        }

        protected readonly PackageDepot depot = null;
        protected PackageState state = PackageState.Invalid;

        public AssetPackage(PackageDepot depot)
        {
            this.depot = depot;
        }

        public bool IsOpened => this.state == PackageState.Opened;

        public virtual void Open()
        {
            this.state = PackageState.Opened;
        }

        public virtual IAsync OpenAsync()
        {
            return null;
        }

        public virtual IAsset Load(string asset, Type type)
        {
            return null;
        }

        public virtual IAsync<IAsset> LoadAsync(string asset, Type type)
        {
            return null;
        }
    }
}
