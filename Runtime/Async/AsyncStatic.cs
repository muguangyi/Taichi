/*
 * This file is part of Taichi project.
 *
 * (c) MuGuangyi <muguangyi@hotmail.com>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 */

using System;
using System.Threading;
using System.Threading.Tasks;
using Taichi.Async.Internal;
using Taichi.Logger;

namespace Taichi.Async
{
    public partial class Async
    {
        private sealed class AsyncNull : Async
        { }

        private sealed class AsyncResult : Async
        {
            public AsyncResult(object result)
            {
                SetResult(result);
            }
        }

        private sealed class AsyncTask : Async
        {
            private readonly CancellationTokenSource cancelSource = new CancellationTokenSource();
            private readonly Task task = null;

            public AsyncTask(Func<Task> func)
            {
                this.task = Task.Factory.StartNew(func, this.cancelSource.Token).Unwrap();
            }

            protected override AsyncState OnUpdate()
            {
                if (this.task.IsCompleted)
                {
                    if (this.task.Exception != null)
                    {
                        Log.Fatal(this.task.Exception.InnerException?.Message);
                    }

                    return AsyncState.Succeed;
                }

                return AsyncState.Running;
            }

            protected override void OnCancel()
            {
                this.cancelSource.Cancel();
            }
        }

        public static IAsync Null => new AsyncNull();

        public static IAsyncCanceller NewCanceller()
        {
            return new AsyncCanceller();
        }

        public static IAsync WithResult(object result)
        {
            return new AsyncResult(result);
        }

        public static IAsync Run(Func<Task> func)
        {
            return new AsyncTask(func);
        }
    }

    public partial class Async<TResult>
    {
        private sealed class AsyncNull : Async<TResult>
        { }

        private sealed class AsyncResult : Async<TResult>
        {
            public AsyncResult(TResult result)
            {
                SetResult(result);
            }
        }

        private sealed class AsyncTask : Async<TResult>
        {
            private readonly CancellationTokenSource cancelSource = new CancellationTokenSource();
            private readonly Task<TResult> task = null;

            public AsyncTask(Func<Task<TResult>> func)
            {
                this.task = Task.Factory.StartNew(func, this.cancelSource.Token).Unwrap();
            }

            public new TResult GetResult()
            {
                return this.task.Result;
            }

            protected override AsyncState OnUpdate()
            {
                if (this.task.IsCompleted)
                {
                    if (this.task.Exception != null)
                    {
                        Log.Fatal(this.task.Exception.InnerException?.Message);
                    }

                    return AsyncState.Succeed;
                }

                return AsyncState.Running;
            }

            protected override void OnCancel()
            {
                this.cancelSource.Cancel();
            }
        }

        public new static IAsync<TResult> Null => new AsyncNull();

        public static IAsync<TResult> WithResult(TResult result)
        {
            return new AsyncResult(result);
        }

        public static IAsync<TResult> Run(Func<Task<TResult>> func)
        {
            return new AsyncTask(func);
        }
    }
}
