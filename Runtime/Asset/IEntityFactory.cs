/*
 * This file is part of Taichi project.
 *
 * (c) MuGuangyi <muguangyi@hotmail.com>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 */

using Taichi.Async;
using Unity.Collections;
using Unity.Entities;

namespace Taichi.Asset
{
    public interface IEntityFactory
    {
        Entity Instantiate(string asset, EntityManager entityManager = default);
        NativeArray<Entity> Instantiate(string asset, NativeArray<Entity> entities, EntityManager entityManager = default);
        IAsync<Entity> InstantiateAsync(string asset, EntityManager entityManager = default);
        IAsync<NativeArray<Entity>> InstantiateAsync(string asset, NativeArray<Entity> entities, EntityManager entityManager = default);
    }
}
