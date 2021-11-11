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

namespace Taichi.Asset
{
    public abstract class AssetDepot : IAssetDepot
    {
        public virtual bool EditorBundleMode { protected get; set; } = false;

        public virtual void Init()
        { }

        public virtual bool Exists(string asset, Type type)
        {
            return false;
        }

        public virtual IAsset Load(string asset, Type type)
        {
            throw new NotImplementedException();
        }

        public virtual IAsync<IAsset> LoadAsync(string asset, Type type)
        {
            throw new NotImplementedException();
        }
    }
}
