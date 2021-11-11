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
    public class Rotation : Trait
    {
        [Accessible]
        public float Roll { get; private set; } = 0;

        [Accessible]
        public float Pitch { get; private set; } = 0;

        [Accessible]
        public float Yaw { get; private set; } = 0;

        [Accessible]
        public void SetRotation(float roll, float pitch, float yaw)
        {
            this.Roll = roll;
            this.Pitch = pitch;
            this.Yaw = yaw;
            NotifyValueChanged(this, new ValueChangedArgs("Rotation"));
        }
    }
}
