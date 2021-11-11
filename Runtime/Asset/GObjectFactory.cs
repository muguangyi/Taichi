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
using UnityEngine;

namespace Taichi.Asset
{
    public sealed class GObjectFactory : IGObjectFactory
    {
        private sealed class AsyncCreateGameObject : Async<GameObject>
        {
            private readonly GObjectFactory factory = null;
            private readonly IAsync<IAsset> request = null;

            public AsyncCreateGameObject(GObjectFactory factory, IAsync<IAsset> request)
            {
                this.factory = factory;
                this.request = request;

                Wait(request);
            }

            protected override bool OnStart()
            {
                var prefab = this.request.GetResult();
                if (prefab != null)
                {
                    SetResult(this.factory.CreateGameObject(prefab));
                }

                return true;
            }
        }

        [Resolve] private static IAssetFactory factory = null;

        private readonly LinkedList<(GameObject, IAsset)> instants = new LinkedList<(GameObject, IAsset)>();

        public GameObject Instantiate(string asset)
        {
            return CreateGameObject(factory.Load(asset, typeof(GameObject)));
        }

        public IAsync<GameObject> InstantiateAsync(string asset)
        {
            var req = factory.LoadAsync(asset, typeof(GameObject));
            return new AsyncCreateGameObject(this, req);
        }

        private GameObject CreateGameObject(IAsset prefab)
        {
            if (prefab == null)
            {
                return null;
            }

            var instance = Object.Instantiate(prefab.Cast<GameObject>());
            this.instants.AddLast((instance, prefab));

            return instance;
        }

        private void OnUpdate(float deltaTime)
        {
            var n = this.instants.First;
            while (n != null)
            {
                var i = n.Value;
                if (i.Item1 == null || i.Item1.Equals(null))
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
