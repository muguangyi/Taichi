/*
 * This file is part of Taichi project.
 *
 * (c) MuGuangyi <muguangyi@hotmail.com>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 */

using System;

namespace Taichi.Stream
{
    abstract class StringStream : IDisposable
    {
        protected const byte TINDEX = 0x00;
        protected const byte STRING = 0x01;

        protected readonly System.IO.Stream stream = null;

        public StringStream(System.IO.Stream stream)
        {
            this.stream = stream;
        }

        public virtual void Dispose()
        {
            this.stream?.Close();
        }
    }
}