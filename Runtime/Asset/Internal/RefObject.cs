/*
 * This file is part of Taichi project.
 *
 * (c) MuGuangyi <muguangyi@hotmail.com>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 */

using System.Collections.Generic;

namespace Taichi.Asset.Internal
{
    internal abstract class RefObject
    {
        private int refCount = 0;
        private readonly LinkedList<RefObject> objects = new LinkedList<RefObject>();

        public int Retain()
        {
            return ++this.refCount;
        }

        public int Release()
        {
            if (--this.refCount == 0)
            {
                var n = this.objects.First;
                while (n != null)
                {
                    n.Value.Release();
                    n = n.Next;
                }
                this.objects.Clear();

                OnDispose();
            }

            return this.refCount;
        }

        public T Refer<T>(T obj) where T : RefObject
        {
            obj.Retain();
            this.objects.AddLast(obj);

            return obj;
        }

        protected virtual void OnDispose()
        { }
    }
}
