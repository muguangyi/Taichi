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
    internal sealed class ResourceDepot : AssetDepot
    {
        public override bool Exists(string asset, Type type)
        {
            return Resources.Load(asset, type) != null;
        }

        public override IAsset Load(string asset, Type type)
        {
            if (type == typeof(Scene))
            {
                throw new InvalidOperationException("can't load scene by Resources.");
            }

            return new Asset(Resources.Load(asset, type));
        }

        public override IAsync<IAsset> LoadAsync(string asset, Type type)
        {
            if (type == typeof(Scene))
            {
                throw new InvalidOperationException("can't load scene by Resources.");
            }

            return new AsyncResourceAsset(Resources.LoadAsync(asset, type));
        }
    }
}
