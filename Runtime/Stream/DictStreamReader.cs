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
    /// Dict stream reader.
    /// </summary>
    public sealed class DictStreamReader : DictStream
    {
        private bool validate = false;
        private readonly StringStreamReader stringReader = null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="validate"></param>
        public DictStreamReader(System.IO.Stream stream, bool validate = false)
            : base(stream)
        {
            this.validate = validate;
            this.stringReader = new StringStreamReader(stream);

            if (this.validate)
            {
                var offset = this.stringReader.ReadInteger();
                Seek(offset);
                {
                    while (this.Position < this.Length)
                    {
                        var key = this.stringReader.ReadString();
                        var count = this.stringReader.ReadInteger();
                        var offsets = new Queue<long>();
                        for (var i = 0; i < count; ++i)
                        {
                            offsets.Enqueue(this.stringReader.ReadInteger());
                        }
                        this.keyMap.Add(key, offsets);
                    }
                }
            }
            Seek(9);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public byte ReadKeyByte(string key)
        {
            return (MoveStreamOffset(key) ? (byte)this.stringReader.ReadByte() : (byte)0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public byte[] ReadKeyBytes(string key)
        {
            return (MoveStreamOffset(key) ? this.stringReader.ReadBytes() : null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public long ReadKeyInteger(string key)
        {
            return (MoveStreamOffset(key) ? this.stringReader.ReadInteger() : 0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public double ReadKeyFloat(string key)
        {
            return (MoveStreamOffset(key) ? this.stringReader.ReadFloat() : 0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string ReadKeyString(string key)
        {
            return (MoveStreamOffset(key) ? this.stringReader.ReadString() : string.Empty);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="separater"></param>
        /// <returns></returns>
        public string ReadKeyIndexString(string key, string separater)
        {
            return (MoveStreamOffset(key) ? this.stringReader.ReadIndexString(separater) : string.Empty);
        }

        private long GetOffset(string key)
        {
            Queue<long> offsets = null;
            if (this.keyMap.TryGetValue(GenerateKey(key), out offsets))
            {
                return offsets.Dequeue();
            }

            return -1;
        }

        private bool MoveStreamOffset(string key)
        {
            long offset = 0;
            if (this.validate)
            {
                offset = GetOffset(key);
                if (offset >= 0)
                {
                    Seek(offset);
                }
            }

            return (offset >= 0);
        }
    }
}