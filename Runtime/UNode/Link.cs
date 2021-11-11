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
    public sealed class Link : IDisposable
    {
        public Link(Spot s0, Spot s1)
        {
            this.S0 = s0;
            this.S1 = s1;
            s0.Connect(this);
            s1.Connect(this);

            this.Dispatcher = new Dispatcher(this);
        }

        public void Dispose()
        {
            this.S0.Disconnect(this);
            this.S1.Disconnect(this);

            this.Dispatcher.Notify(Message.Destroy);
            this.Dispatcher.Dispose();

            this.S0 = null;
            this.S1 = null;
            this.Dispatcher = null;
        }

        public Spot GetLinkedSpot(Spot spot)
        {
            if (spot == this.S0)
            {
                return this.S1;
            }
            else
            {
                return this.S0;
            }
        }

        public Spot GetInSpot()
        {
            return SpotType.In == this.S0.Type ? this.S0 : this.S1;
        }

        public Spot GetOutSpot()
        {
            return SpotType.Out == this.S0.Type ? this.S0 : this.S1;
        }

        public Spot S0 { get; private set; } = null;

        public Spot S1 { get; private set; } = null;

        public Dispatcher Dispatcher { get; private set; } = null;
    }
}