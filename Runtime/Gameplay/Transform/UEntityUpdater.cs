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
    [RequireValue("X")]
    [RequireValue("Y")]
    [RequireValue("Z")]
    [RequireValue("Roll")]
    [RequireValue("Pitch")]
    [RequireValue("Yaw")]
    [RequireValue("ScaleX")]
    [RequireValue("ScaleY")]
    [RequireValue("ScaleZ")]
    public class UEntityUpdater : Feature
    {
        [OnChangeValue("Location")]
        public void OnLocationChanged(object sender, ValueChangedArgs args)
        {
            ApplyLocationAndRotation();
        }

        [OnChangeValue("Rotation")]
        public void OnRotationChanged(object sender, ValueChangedArgs args)
        {
            ApplyLocationAndRotation();
        }

        [OnChangeValue("Scale")]
        public void OnScaleChanged(object sender, ValueChangedArgs args)
        {
            ApplyScale();
        }

        private void ApplyLocationAndRotation()
        {
        }

        private void ApplyScale()
        {
        }
    }
}
