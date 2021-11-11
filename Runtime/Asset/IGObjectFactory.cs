/*
 * This file is part of Taichi project.
 *
 * (c) MuGuangyi <muguangyi@hotmail.com>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 */

using Taichi.Async;
using UnityEngine;

namespace Taichi.Asset
{
    public interface IGObjectFactory
    {
        GameObject Instantiate(string asset);
        IAsync<GameObject> InstantiateAsync(string asset);
    }
}
