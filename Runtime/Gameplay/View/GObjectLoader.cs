/*
 * This file is part of Taichi project.
 *
 * (c) MuGuangyi <muguangyi@hotmail.com>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 */

using Taichi.Foundation;
using Taichi.Asset;
using Taichi.Gameplay.Internal;
using Taichi.Binding;

namespace Taichi.Gameplay
{
    public sealed class GObjectLoader : Feature
    {
        [Resolve] private static IGObjectFactory factory = null;

        [OnChangeValue("AssetReference")]
        private void OnAssetReferenceChanged(object sender, ValueChangedArgs args)
        {
            var node = (GObjectNode)sender;
            var assetReference = (string)args.Value;
            factory.InstantiateAsync(assetReference).Completed += a =>
            {
                var go = (UnityEngine.GameObject)a.GetResult();
                node.Transform = go.transform;
            };
        }
    }
}
