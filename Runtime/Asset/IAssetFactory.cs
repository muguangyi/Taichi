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
    public interface IAssetFactory
    {
        bool EditorBundleMode { set; }

        IAsset Load(string asset, Type type);
        IAsync<IAsset> LoadAsync(string asset, Type type);

        void OpenDepot(IAssetDepot depot);
    }
}
