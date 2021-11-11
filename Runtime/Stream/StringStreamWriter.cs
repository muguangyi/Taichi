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
    /// 字符串流同步写入器。
    /// </summary>
    sealed class StringStreamWriter : StringStream
    {
        private int stringIndex = 0;
        private Dictionary<string, int> stringMap = null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        public StringStreamWriter(System.IO.Stream stream)
            : base(stream)
        {
            this.stringIndex = 0;
            this.stringMap = new Dictionary<string, int>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public void WriteByte(byte value)
        {
            this.stream.WriteByte(value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public void WriteBytes(byte[] value)
        {
            var length = (null == value ? 0 : value.Length);
            this.stream.WriteInteger(length);
            if (length > 0)
            {
                this.stream.Write(value, 0, length);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public void WriteInteger(long value)
        {
            this.stream.WriteInteger(value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public void WriteFloat(double value)
        {
            this.stream.WriteFloat(value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public void WriteString(string value)
        {
            this.stream.WriteString(value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="separater"></param>
        public void WriteIndexString(string value, string separater)
        {
            var segments = value.Split(separater[0]);

            this.stream.WriteInteger(segments.Length);
            for (var i = 0; i < segments.Length; ++i)
            {
                var index = -1;
                if (this.stringMap.TryGetValue(segments[i], out index))
                {
                    stream.WriteByte(TINDEX);
                    stream.WriteInteger(index);
                }
                else
                {
                    stream.WriteByte(STRING);
                    stream.WriteString(segments[i]);
                    this.stringMap.Add(segments[i], (++this.stringIndex));
                }
            }
        }
    }
}