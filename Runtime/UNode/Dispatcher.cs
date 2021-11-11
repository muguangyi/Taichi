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

namespace Taichi.UNode
{
    /// <summary>
    /// 消息分发器。
    /// </summary>
    public sealed class Dispatcher : IDisposable
    {
        private readonly object target = null;
        private Dictionary<string, List<Action<object, Message>>> listeners = new Dictionary<string, List<Action<object, Message>>>();

        /// <summary>
        /// 构造函数。
        /// </summary>
        /// <param name="target">发送的主体。</param>
        public Dispatcher(object target)
        {
            this.target = target;
        }

        /// <summary>
        /// 析构函数。
        /// </summary>
        public void Dispose()
        {
            if (null != this.listeners)
            {
                this.listeners.Clear();
                this.listeners = null;
            }
        }

        /// <summary>
        /// 添加监听函数句柄。
        /// </summary>
        /// <param name="type">监听的消息类型。</param>
        /// <param name="handler">函数句柄。</param>
        public void AddListener(string type, Action<object, Message> handler)
        {
            if (null == handler)
            {
                return;
            }

            if (!this.listeners.TryGetValue(type, out List<Action<object, Message>> handlers))
            {
                handlers = new List<Action<object, Message>>();
                this.listeners.Add(type, handlers);
            }

            var index = handlers.IndexOf(handler);
            if (index < 0)
            {
                handlers.Add(handler);
            }
        }

        /// <summary>
        /// 删除监听函数句柄。
        /// </summary>
        /// <param name="type">监听的消息类型。</param>
        /// <param name="handler">函数句柄。</param>
        public void RemoveListener(string type, Action<object, Message> handler)
        {
            if (null == handler)
            {
                return;
            }

            if (this.listeners.TryGetValue(type, out List<Action<object, Message>> handlers))
            {
                handlers.Remove(handler);
            }
        }

        /// <summary>
        /// 广播消息。
        /// </summary>
        /// <param name="message">消息。</param>
        /// <param name="resetHandlersAfterSend">是否在发送后清空监听的函数句柄。</param>
        public void Notify(Message message, bool resetHandlersAfterSend = false)
        {
            if (null == this.listeners)
            {
                return;
            }

            if (Message.ANY == message.Type)
            {
                foreach (var pair in this.listeners)
                {
                    NotifyHandlers(pair.Value, message);
                }

                if (resetHandlersAfterSend)
                {
                    ResetAll();
                }
            }
            else
            {
                if (this.listeners.TryGetValue(message.Type, out List<Action<object, Message>> handlers))
                {
                    NotifyHandlers(handlers, message);

                    if (resetHandlersAfterSend)
                    {
                        Reset(message.Type);
                    }
                }
                if (this.listeners.TryGetValue(Message.ANY, out handlers))
                {
                    NotifyHandlers(handlers, message);
                }
            }
        }

        private void Reset(string type)
        {
            if (null != this.listeners)
            {
                if (this.listeners.TryGetValue(type, out List<Action<object, Message>> handlers))
                {
                    handlers.Clear();
                }
            }
        }

        private void ResetAll()
        {
            if (null != this.listeners)
            {
                this.listeners.Clear();
            }
        }

        private void NotifyHandlers(List<Action<object, Message>> handlers, Message message)
        {
            var count = handlers.Count;
            for (var i = count - 1; i >= 0; --i)
            {
                handlers[i](this.target, message);
            }
        }
    }
}