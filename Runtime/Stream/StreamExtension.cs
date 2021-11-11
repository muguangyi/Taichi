/*
 * This file is part of Taichi project.
 *
 * (c) MuGuangyi <muguangyi@hotmail.com>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 */

using System;
using System.Text;

namespace Taichi.Stream
{
    /// <summary>
    /// Stream extension.
    /// </summary>
    static class StreamExtension
    {
        private const byte FLOAT32 = 0xca;
        private const byte FLOAT64 = 0xcb;
        private const byte UINT8 = 0xcc;
        private const byte UINT16 = 0xcd;
        private const byte UINT32 = 0xce;
        private const byte UINT64 = 0xcf;
        private const byte INT8 = 0xd0;
        private const byte INT16 = 0xd1;
        private const byte INT32 = 0xd2;
        private const byte INT64 = 0xd3;

        /// <summary>
        /// Write integer value.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="value"></param>
        public static void WriteInteger(this System.IO.Stream stream, long value)
        {
            if (value >= -128 && value <= 127)
            {
                stream.WriteByte(INT8);
                stream.WriteByte((byte)value);
            }
            else if (value >= short.MinValue && value <= short.MaxValue)
            {
                stream.WriteByte(INT16);
                stream.WriteByte((byte)(value << 16 >> 24));
                stream.WriteByte((byte)(value << 24 >> 24));
            }
            else if (value >= int.MinValue && value <= int.MaxValue)
            {
                stream.WriteByte(INT32);
                stream.WriteByte((byte)(value >> 24));
                stream.WriteByte((byte)(value << 8 >> 24));
                stream.WriteByte((byte)(value << 16 >> 24));
                stream.WriteByte((byte)(value << 24 >> 24));
            }
            else
            {
                stream.WriteByte(INT64);
                stream.WriteByte((byte)(value >> 56));
                stream.WriteByte((byte)(value << 8 >> 56));
                stream.WriteByte((byte)(value << 16 >> 56));
                stream.WriteByte((byte)(value << 24 >> 56));
                stream.WriteByte((byte)(value << 32 >> 56));
                stream.WriteByte((byte)(value << 40 >> 56));
                stream.WriteByte((byte)(value << 48 >> 56));
                stream.WriteByte((byte)(value << 56 >> 56));
            }
        }

        /// <summary>
        /// Write float value.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="value"></param>
        public static void WriteFloat(this System.IO.Stream stream, double value)
        {
            //�Ƴ���,FLOAT32��Ϊת���ᵼ�¾����쳣
            var bytes = BitConverter.GetBytes(value);
            stream.WriteByte(FLOAT64);
            foreach (var b in bytes)
            {
                stream.WriteByte(b);
            }
        }

        /// <summary>
        /// Write string value.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="value"></param>
        public static void WriteString(this System.IO.Stream stream, string value)
        {
            var bytes = Encoding.UTF8.GetBytes(value);
            stream.WriteInteger(bytes.Length);
            stream.Write(bytes, 0, bytes.Length);
        }

        /// <summary>
        /// Read integer value.
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static long ReadInteger(this System.IO.Stream stream)
        {
            var result = stream.ReadByte();
            if (result == -1)
            {
                throw new Exception("Has reached the end of the stream");
            }

            switch (result)
            {
            case INT8:
                return (sbyte)stream.ReadByte();
            case INT16:
                return (short)((stream.ReadByte() << 8) +
                              (stream.ReadByte()));
            case INT32:
                return (int)((stream.ReadByte() << 24) +
                              (stream.ReadByte() << 16) +
                              (stream.ReadByte() << 8) +
                              (stream.ReadByte()));
            default:
                long value = 0;
                for (var i = 0; i < 8; i++)
                {
                    value = value << 8;
                    value |= (byte)stream.ReadByte();
                }
                return value;
            }
        }

        /// <summary>
        /// Read float value.
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static double ReadFloat(this System.IO.Stream stream)
        {
            var index = 0;
            var type = stream.ReadByte();

            var bytes = new byte[8];
            for (var i = index; i < 8; ++i)
            {
                bytes[i] = (byte)stream.ReadByte();
            }

            return BitConverter.ToDouble(bytes, 0);
        }

        /// <summary>
        /// Read string value.
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static string ReadString(this System.IO.Stream stream)
        {
            var count = (int)stream.ReadInteger();
            var bytes = new byte[count];
            stream.Read(bytes, 0, count);
            return Encoding.UTF8.GetString(bytes);
        }
    }
}