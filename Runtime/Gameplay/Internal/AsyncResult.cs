/*
 * This file is part of Taichi project.
 *
 * (c) MuGuangyi <muguangyi@hotmail.com>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 */

using Taichi.Async;

namespace Taichi.Gameplay.Internal
{
    internal class AsyncResult<T> : Async<T>
    {
        public new static AsyncResult<T> Null => new AsyncResult<T>(default);

        public AsyncResult(T result)
        {
            SetResult(result);
        }
    }
}
