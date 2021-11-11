/*
 * This file is part of Taichi project.
 *
 * (c) MuGuangyi <muguangyi@hotmail.com>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 */

using Taichi.Binding;

namespace Taichi.Gameplay
{
    public class Location : Trait
    {
        [Accessible]
        public float X { get; private set; } = 0;

        [Accessible]
        public float Y { get; private set; } = 0;

        [Accessible]
        public float Z { get; private set; } = 0;

        [Accessible]
        public void SetLocation(float x, float y, float z = 0)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
            NotifyValueChanged(this, new ValueChangedArgs("Location"));
        }
    }
}
