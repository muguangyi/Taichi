/*
 * This file is part of Taichi project.
 *
 * (c) MuGuangyi <muguangyi@hotmail.com>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 */

using UnityEngine.SceneManagement;

namespace Taichi.Asset.Internal
{
    internal sealed class SceneAsset : Asset
    {
        public SceneAsset(object asset, RefObject refObject = null) : base(asset, refObject)
        { }

        protected override void OnDispose()
        {
            SceneManager.UnloadSceneAsync((Scene)this.asset);
        }
    }
}
