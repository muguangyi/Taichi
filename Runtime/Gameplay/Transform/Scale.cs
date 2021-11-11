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
    public class Scale : Trait
    {
        [Accessible]
        public float ScaleX { get; private set; } = 1;

        [Accessible]
        public float ScaleY { get; private set; } = 1;

        [Accessible]
        public float ScaleZ { get; private set; } = 1;

        [Accessible]
        public void SetScale(float x, float y, float z)
        {
            this.ScaleX = x;
            this.ScaleY = y;
            this.ScaleZ = z;
            NotifyValueChanged(this, new ValueChangedArgs("Scale"));
        }
    }
}
