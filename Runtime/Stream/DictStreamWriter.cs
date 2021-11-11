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
    /// Dict stream writer.
    /// </summary>
    public sealed class DictStreamWriter : DictStream
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        public DictStreamWriter(System.IO.Stream stream)
            : base(stream)
        {
            Seek(9);
            this.stringWriter = new StringStreamWriter(stream);
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Dispose()
        {
            long offset = this.Position;
            foreach (var pair in this.keyMap)
            {
                this.stringWriter.WriteString(pair.Key);
                var offsets = pair.Value;
                this.stringWriter.WriteInteger(offsets.Count);
                while (offsets.Count > 0)
                {
                    this.stringWriter.WriteInteger(offsets.Dequeue());
                }
            }
            SetLength(this.Position);
            Seek(0);
            this.stringWriter.WriteInteger(offset);

            base.Dispose();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void WriteKeyByte(string key, byte value)
        {
            SetOffset(GenerateKey(key), this.Position);
            this.stringWriter.WriteByte(value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="bytes"></param>
        public void WriteKeyBytes(string key, byte[] bytes)
        {
            SetOffset(GenerateKey(key), this.Position);
            this.stringWriter.WriteBytes(bytes);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void WriteKeyInteger(string key, long value)
        {
            SetOffset(GenerateKey(key), this.Position);
            this.stringWriter.WriteInteger(value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void WriteKeyFloat(string key, double value)
        {
            SetOffset(GenerateKey(key), this.Position);
            this.stringWriter.WriteFloat(value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void WriteKeyString(string key, string value)
        {
            SetOffset(GenerateKey(key), this.Position);
            this.stringWriter.WriteString(value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="separater"></param>
        public void WriteKeyIndexString(string key, string value, string separater)
        {
            SetOffset(GenerateKey(key), this.Position);
            this.stringWriter.WriteIndexString(value, separater);
        }

        private void SetOffset(string key, long offset)
        {
            Queue<long> offsets = null;
            if (!this.keyMap.TryGetValue(key, out offsets))
            {
                this.keyMap[key] = offsets = new Queue<long>();
            }

            offsets.Enqueue(offset);
        }

        private StringStreamWriter stringWriter = null;
    }
}