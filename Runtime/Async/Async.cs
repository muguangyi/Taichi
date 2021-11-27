/*
 * This file is part of Taichi project.
 *
 * (c) MuGuangyi <muguangyi@hotmail.com>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 */

using System;
using System.Collections.Generic;
using System.Threading;
using Taichi.Async.Internal;

namespace Taichi.Async
{
    public partial class Async : IAsync
    {
        protected enum AsyncState
        {
            Suspend,
            NotStart,
            Running,
            Succeed,
            Canceled,
        }

        private AsyncCanceller canceller = null;
        private object result = null;
        private AsyncState lastState = AsyncState.NotStart;
        private Action continuation = null;
        private volatile int _wait = 0;

        public Async()
        {
            Start(this);
        }

        public event Action<IAsync> Completed;

        public object Current => this.result;

        public IAsyncCanceller Canceller
        {
            get => this.canceller;

            set
            {
                this.canceller?.Remove(this);
                this.canceller = (AsyncCanceller)value;
                this.canceller?.Add(this);
            }
        }

        public bool IsCompleted => this.State == AsyncState.Succeed || this.State == AsyncState.Canceled;

        public object GetResult()
        {
            return this.result;
        }

        public bool MoveNext()
        {
            if (this.IsCompleted)
            {
                return false;
            }

            return Update();
        }

        public void Reset()
        {
            this.State = AsyncState.NotStart;
            this.Completed = null;
            this.continuation = null;
            OnReset();
        }

        public IAsync Wait(IAsync task)
        {
            Suspend();

            Interlocked.Increment(ref _wait);
            task.Completed += _ =>
            {
                if (Interlocked.Decrement(ref _wait) == 0)
                {
                    Resume();
                    Update(); // Update status in the same frame.
                }
            };

            return this;
        }

        public void Resume()
        {
            if (this.State == AsyncState.Suspend)
            {
                this.State = this.lastState;
            }
        }

        public void Suspend()
        {
            if (this.State != AsyncState.Suspend)
            {
                this.lastState = this.State;
                this.State = AsyncState.Suspend;
            }
        }

        public void Cancel(bool invokeCompleted)
        {
            if (this.State == AsyncState.Succeed || this.State == AsyncState.Canceled)
            {
                return;
            }

            this.State = AsyncState.Canceled;
            OnCancel();

            if (invokeCompleted)
            {
                this.Completed?.Invoke(this);
                this.continuation?.Invoke();
            }
        }

        public void OnCompleted(Action continuation)
        {
            if (IsCompleted)
            {
                this.Completed?.Invoke(this);
                continuation?.Invoke();
            }
            else
            {
                this.continuation += continuation;
            }
        }

        public IAsync GetAwaiter()
        {
            return this;
        }

        protected AsyncState State { get; private set; } = AsyncState.NotStart;

        protected void SetResult(object result)
        {
            this.result = result;
        }

        protected virtual bool OnStart()
        {
            return true;
        }

        protected virtual AsyncState OnUpdate()
        {
            return AsyncState.Succeed;
        }

        protected virtual void OnEnd()
        { }

        protected virtual void OnReset()
        { }

        protected virtual void OnCancel()
        { }

        private bool Update()
        {
            if (this.State == AsyncState.Suspend)
            {
                return true;
            }

            if (this.State == AsyncState.NotStart)
            {
                if (!OnStart())
                {
                    return true;
                }

                this.State = AsyncState.Running;
            }

            this.State = OnUpdate();
            if (this.State == AsyncState.Succeed)
            {
                OnEnd();
                this.Completed?.Invoke(this);
                this.continuation?.Invoke();
                this.Canceller = null;
            }

            return this.State != AsyncState.Succeed;
        }

        private static readonly LinkedList<IAsync> tasks = new LinkedList<IAsync>();
        private static readonly Stack<LinkedListNode<IAsync>> nodeCache = new Stack<LinkedListNode<IAsync>>();

        internal static void Tick(float deltaTime)
        {
            var n = tasks.First;
            while (n != null)
            {
                var c = n;
                n = n.Next;
                if (c.Value.IsCompleted || !c.Value.MoveNext())
                {
                    try
                    {
                        tasks.Remove(c);
                        c.Value = null;
                        nodeCache.Push(c);
                    }
                    catch
                    { }
                }
            }
        }

        private static void Start(IAsync task)
        {
            if (nodeCache.Count > 0)
            {
                var n = nodeCache.Pop();
                n.Value = task;
                tasks.AddLast(n);
            }
            else
            {
                tasks.AddLast(task);
            }
        }
    }

    public partial class Async<TResult> : Async, IAsync<TResult>
    {
        public new TResult GetResult()
        {
            return (TResult)base.GetResult();
        }

        public new IAsync<TResult> Wait(IAsync task)
        {
            base.Wait(task);

            return this;
        }

        public new IAsync<TResult> GetAwaiter()
        {
            return this;
        }
    }
}
