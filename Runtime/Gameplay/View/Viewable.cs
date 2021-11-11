/*
 * This file is part of Taichi project.
 *
 * (c) MuGuangyi <muguangyi@hotmail.com>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 */

using Taichi.Binding;

namespace Taichi.Gameplay
{
    public abstract class Viewable : Trait
    {
        protected IViewableNode root = null;

        [Accessible]
        public IViewableNode Retrieve(string path = null)
        {
            return this.root.Retrieve(path);
        }

        internal void NotifyAssetReferenceChanged(IViewableNode n, string assetReference)
        {
            NotifyValueChanged(n, new ValueChangedArgs("AssetReference", assetReference));
        }
    }
}
