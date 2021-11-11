/*
 * This file is part of Taichi project.
 *
 * (c) MuGuangyi <muguangyi@hotmail.com>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 */

using System.Collections.Generic;

namespace Taichi.Async.Internal
{
    internal class AsyncCanceller : IAsyncCanceller
    {
        private readonly Dictionary<int, IAsync> _asyncs = new Dictionary<int, IAsync>();

        public void Dispose()
        {
            Cancel();
        }

        public void Cancel(bool invokeCompleted = false)
        {
            foreach (var async in _asyncs.Values)
            {
                async.Cancel(invokeCompleted);
            }
            _asyncs.Clear();
        }

        internal void Add(IAsync async)
        {
            if (async == null)
            {
                return;
            }

            _asyncs.Add(async.GetHashCode(), async);
        }

        internal void Remove(IAsync async)
        {
            if (async == null)
            {
                return;
            }

            _asyncs.Remove(async.GetHashCode());
        }
    }
}