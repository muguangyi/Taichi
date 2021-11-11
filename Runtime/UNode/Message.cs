/*
 * This file is part of Taichi project.
 *
 * (c) MuGuangyi <muguangyi@hotmail.com>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 */

using System;

namespace Taichi.UNode
{
    /// <summary>
    /// @details 消息。
    /// </summary>
    public class Message : IDisposable
    {
        /// <summary>
        /// 任意类型。
        /// </summary>
        public const string ANY = "_any";

        /// <summary>
        /// 完成类型。
        /// </summary>
        public const string COMPLETED = "_completed";

        /// <summary>
        /// 销毁类型。
        /// </summary>
        public const string DESTROY = "_destroy";

        /// <summary>
        /// 成功类型。
        /// </summary>
        public const string SUCCESS = "_success";

        /// <summary>
        /// 失败类型。
        /// </summary>
        public const string FAILED = "_failed";

        /// <summary>
        /// 构造函数。
        /// </summary>
        /// <param name="type">消息类型。</param>
        public Message(string type)
        {
            this.Type = type;
        }

        public virtual void Dispose()
        { }

        /// <summary>
        /// 消息类型。
        /// </summary>
        public string Type { get; } = "";

        /// <summary>
        /// 任意消息。
        /// </summary>
        public static Message Any = new Message(Message.ANY);

        /// <summary>
        /// 完成消息。
        /// </summary>
        public static Message Completed = new Message(Message.COMPLETED);

        /// <summary>
        /// 销毁消息。
        /// </summary>
        public static Message Destroy = new Message(Message.DESTROY);

        /// <summary>
        /// 成功消息。
        /// </summary>
        public static Message Success = new Message(Message.SUCCESS);

        /// <summary>
        /// 失败消息。
        /// </summary>
        public static Message Failed = new Message(Message.FAILED);
    }
}
