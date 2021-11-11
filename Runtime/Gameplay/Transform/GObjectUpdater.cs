/*
 * This file is part of Taichi project.
 *
 * (c) MuGuangyi <muguangyi@hotmail.com>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 */

using Taichi.Binding;
using UnityEngine;

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
    public class GObjectUpdater : Feature
    {
        private Transform transform = null;

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

        protected override void OnInit()
        {
            base.OnInit();

            var async = this.Actor.Call<IViewableNode, string>("Retrieve", null);
            var node = async.GetResult();
            node.Completed += n =>
            {
                this.transform = ((GameObject)n.View).transform;
                ApplyScale();
                ApplyLocationAndRotation();
            };
        }

        private void ApplyLocationAndRotation()
        {
            if (this.transform == null)
            {
                return;
            }

            var x = this.Actor.Get<float>("X");
            var y = this.Actor.Get<float>("Y");
            var z = this.Actor.Get<float>("Z");
            var roll = this.Actor.Get<float>("Roll");
            var pitch = this.Actor.Get<float>("Pitch");
            var yaw = this.Actor.Get<float>("Yaw");
            this.transform.SetPositionAndRotation(new Vector3(x, y, z), Quaternion.Euler(roll, pitch, yaw));
        }

        private void ApplyScale()
        {
            if (this.transform == null)
            {
                return;
            }

            var scaleX = this.Actor.Get<float>("ScaleX");
            var scaleY = this.Actor.Get<float>("ScaleY");
            var scaleZ = this.Actor.Get<float>("ScaleZ");
            this.transform.localScale = new Vector3(scaleX, scaleY, scaleZ);
        }
    }
}

