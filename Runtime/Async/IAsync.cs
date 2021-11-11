/*
 * This file is part of Taichi project.
 *
 * (c) MuGuangyi <muguangyi@hotmail.com>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 */

using System;
using System.Collections;
using System.Runtime.CompilerServices;

namespace Taichi.Async
{
    public interface IAsync : IEnumerator, INotifyCompletion
    {
        event Action<IAsync> Completed;

        IAsyncCanceller Canceller { set; }

        bool IsCompleted { get; }

        object GetResult();

        IAsync Wait(IAsync task);

        void Resume();
        void Suspend();
        void Cancel(bool invokeCompleted);

        IAsync GetAwaiter();
    }

    public interface IAsync<TResult> : IAsync
    {
        new TResult GetResult();

        new IAsync<TResult> Wait(IAsync task);

        new IAsync<TResult> GetAwaiter();
    }
}
