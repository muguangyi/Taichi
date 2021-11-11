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
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Taichi.Stream
{
    /// <summary>
    /// Dict stream.
    /// </summary>
    public abstract class DictStream : IDisposable
    {
        private const int KEY_MAX_LENGTH = 32;

        private readonly System.IO.Stream stream = null;
        protected Stack<string> keyStack = null;
        protected Dictionary<string, Queue<long>> keyMap = null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        public DictStream(System.IO.Stream stream)
        {
            this.stream = stream;

            this.keyStack = new Stack<string>();
            this.keyMap = new Dictionary<string, Queue<long>>();
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual void Dispose()
        {
            this.keyStack = null;
            this.keyMap = null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="action"></param>
        public virtual void Blob(string key, Action action)
        {
            this.keyStack.Push(key);
            {
                action();
            }
            this.keyStack.Pop();
        }

        /// <summary>
        /// 
        /// </summary>
        protected long Position
        {
            get
            {
                return this.stream.Position;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected long Length
        {
            get
            {
                return this.stream.Length;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        protected string GenerateKey(string key)
        {
            var keyPath = string.Join("-", this.keyStack.ToArray()) + "-" + key;
            if (keyPath.Length <= KEY_MAX_LENGTH)
            {
                return keyPath;
            }

            var keyBytes = Encoding.UTF8.GetBytes(keyPath);
            var md5 = new MD5CryptoServiceProvider();
            var result = md5.ComputeHash(keyBytes, 0, keyBytes.Length);
            var builder = new StringBuilder();
            for (var i = 0; i < result.Length; ++i)
            {
                builder.Append(result[i].ToString("x2"));
            }

            return builder.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="offset"></param>
        /// <returns></returns>
        protected long Seek(long offset)
        {
            return this.stream.Seek(offset, SeekOrigin.Begin);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="length"></param>
        protected void SetLength(long length)
        {
            this.stream.SetLength(length);
        }
    }
}