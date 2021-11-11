/*
 * This file is part of Taichi project.
 *
 * (c) MuGuangyi <muguangyi@hotmail.com>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 */

using System.Collections.Generic;

namespace Taichi.Stream
{
    /// <summary>
    /// 字符串流同步读取器。
    /// </summary>
    sealed class StringStreamReader : StringStream
    {
        private readonly List<string> stringTable = null;

        /// <summary>
        /// 构造函数。
        /// </summary>
        /// <param name="stream"></param>
        public StringStreamReader(System.IO.Stream stream)
            : base(stream)
        {
            this.stringTable = new List<string>();
        }

        /// <summary>
        /// 读一个字节。
        /// </summary>
        /// <returns></returns>
        public byte ReadByte()
        {
            return (byte)this.stream.ReadByte();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public byte[] ReadBytes()
        {
            var length = (int)this.stream.ReadInteger();
            var bytes = new byte[length];
            if (length > 0)
            {
                this.stream.Read(bytes, 0, length);
            }

            return bytes;
        }

        /// <summary>
        /// 读一个整数。
        /// </summary>
        /// <returns></returns>
        public long ReadInteger()
        {
            return this.stream.ReadInteger();
        }

        /// <summary>
        /// 读一个浮点数。
        /// </summary>
        /// <returns></returns>
        public double ReadFloat()
        {
            return this.stream.ReadFloat();
        }

        /// <summary>
        /// 读一个字符串。
        /// </summary>
        /// <returns></returns>
        public string ReadString()
        {
            return this.stream.ReadString();
        }

        /// <summary>
        /// 读一个索引字符串。
        /// </summary>
        /// <param name="separater"></param>
        /// <returns></returns>
        public string ReadIndexString(string separater)
        {
            var length = this.stream.ReadInteger();
            var segments = new string[length];
            for (var i = 0; i < length; ++i)
            {
                var flag = this.stream.ReadByte();
                switch (flag)
                {
                case TINDEX:
                    var index = (int)this.stream.ReadInteger();
                    segments[i] = this.stringTable[index - 1];
                    break;
                case STRING:
                    var data = this.stream.ReadString();
                    this.stringTable.Add(data);
                    segments[i] = data;
                    break;
                }
            }

            return string.Join(separater, segments);
        }
    }
}