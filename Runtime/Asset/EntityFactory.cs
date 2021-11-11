/*
 * This file is part of Taichi project.
 *
 * (c) MuGuangyi <muguangyi@hotmail.com>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 */

using Taichi.Async;
using Taichi.Foundation;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace Taichi.Asset
{
    public sealed class EntityFactory : IEntityFactory
    {
        private sealed class AsyncCreateEntity : Async<Entity>
        {
            private readonly EntityFactory factory = null;
            private readonly IAsync<IAsset> request = null;
            private readonly EntityManager entityManager = default;

            public AsyncCreateEntity(EntityFactory factory, IAsync<IAsset> request, EntityManager entityManager)
            {
                this.factory = factory;
                this.request = request;
                this.entityManager = entityManager;

                Wait(request);
            }

            protected override bool OnStart()
            {
                var prefab = this.request.GetResult();
                if (prefab != null)
                {
                    SetResult(this.factory.CreateEntity(prefab, this.entityManager));
                }

                return true;
            }
        }

        private sealed class AsyncCreateEntities : Async<NativeArray<Entity>>
        {
            private readonly EntityFactory factory = null;
            private readonly IAsync<IAsset> request = null;
            private readonly NativeArray<Entity> entities = default;
            private readonly EntityManager entityManager = default;

            public AsyncCreateEntities(EntityFactory factory, IAsync<IAsset> request, NativeArray<Entity> entities, EntityManager entityManager)
            {
                this.factory = factory;
                this.request = request;
                this.entities = entities;
                this.entityManager = entityManager;

                Wait(request);
            }

            protected override bool OnStart()
            {
                var prefab = this.request.GetResult();
                if (prefab != null)
                {
                    SetResult(this.factory.CreateEntities(prefab, this.entities, this.entityManager));
                }

                return true;
            }
        }

        [Resolve] private static IAssetFactory factory = null;

        private readonly LinkedList<(Entity, IAsset)> instants = new LinkedList<(Entity, IAsset)>();

        public Entity Instantiate(string asset, EntityManager entityManager = default)
        {
            return CreateEntity(factory.Load(asset, typeof(GameObject)), entityManager);
        }

        public NativeArray<Entity> Instantiate(string asset, NativeArray<Entity> entities, EntityManager entityManager = default)
        {
            return CreateEntities(factory.Load(asset, typeof(GameObject)), entities, entityManager);
        }

        public IAsync<Entity> InstantiateAsync(string asset, EntityManager entityManager = default)
        {
            var req = factory.LoadAsync(asset, typeof(GameObject));
            return new AsyncCreateEntity(this, req, entityManager);
        }

        public IAsync<NativeArray<Entity>> InstantiateAsync(string asset, NativeArray<Entity> entities, EntityManager entityManager = default)
        {
            var req = factory.LoadAsync(asset, typeof(GameObject));
            return new AsyncCreateEntities(this, req, entities, entityManager);
        }

        private Entity CreateEntity(IAsset prefab, EntityManager entityManager)
        {
            if (prefab == null)
            {
                return default;
            }

            entityManager = entityManager == default ? World.DefaultGameObjectInjectionWorld.EntityManager : entityManager;
            var settings = GameObjectConversionSettings.FromWorld(entityManager.World, null);

            var prefabEntity = GameObjectConversionUtility.ConvertGameObjectHierarchy(prefab.Cast<GameObject>(), settings);
            var e = entityManager.Instantiate(prefabEntity);
            this.instants.AddLast((e, prefab));

            return e;
        }

        private NativeArray<Entity> CreateEntities(IAsset prefab, NativeArray<Entity> entities, EntityManager entityManager)
        {
            if (prefab != null)
            {
                entityManager = entityManager == default ? World.DefaultGameObjectInjectionWorld.EntityManager : entityManager;
                var settings = GameObjectConversionSettings.FromWorld(entityManager.World, null);

                var prefabEntity = GameObjectConversionUtility.ConvertGameObjectHierarchy(prefab.Cast<GameObject>(), settings);
                entityManager.Instantiate(prefabEntity, entities);
                foreach (var e in entities)
                {
                    this.instants.AddLast((e, prefab));
                }
            }

            return entities;
        }

        private void OnUpdate(float deltaTime)
        {
            var n = this.instants.First;
            while (n != null)
            {
                var i = n.Value;
                if (i.Item1 == Entity.Null)
                {
                    i.Item2.Dispose();
                    this.instants.Remove(n);
                    return;
                }

                n = n.Next;
            }
        }
    }
}
