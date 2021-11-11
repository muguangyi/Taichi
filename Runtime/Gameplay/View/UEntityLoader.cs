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
using Unity.Entities;
using Taichi.Binding;

namespace Taichi.Gameplay
{
    public sealed class UEntityLoader : Feature
    {
        [Resolve] private static IEntityFactory factory = null;

        [OnChangeValue("AssetReference")]
        private void OnAssetReferenceChanged(object sender, ValueChangedArgs args)
        {
            var node = (UEntityNode)sender;
            var assetReference = (string)args.Value;
            factory.InstantiateAsync(assetReference).Completed += a =>
            {
                var entity = (Entity)a.GetResult();
                node.Entity = entity;
            };
        }
    }
}
