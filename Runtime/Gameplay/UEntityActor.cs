/*
 * This file is part of Taichi project.
 *
 * (c) MuGuangyi <muguangyi@hotmail.com>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 */

namespace Taichi.Gameplay
{
    public class UEntityActor : Actor, IViewableActor
    {
        public float X => Get<float>("X");
        public float Y => Get<float>("Y");
        public float Z => Get<float>("Z");
        public float Roll => Get<float>("Roll");
        public float Pitch => Get<float>("Pitch");
        public float Yaw => Get<float>("Yaw");
        public float ScaleX => Get<float>("ScaleX");
        public float ScaleY => Get<float>("ScaleY");
        public float ScaleZ => Get<float>("ScaleZ");

        public IViewableNode Retrieve(string path = null)
        {
            throw new System.NotImplementedException();
        }

        public void SetLocation(float x, float y, float z = 0)
        {
            Call("SetLocation", x, y, z);
        }

        public void SetRotation(float roll, float pitch, float yaw)
        {
            Call("SetRotation", roll, pitch, yaw);
        }

        public void SetScale(float x, float y, float z)
        {
            Call("SetScale", x, y, z);
        }

        protected override void OnInit()
        {
            base.OnInit();

            AddTrait(typeof(UEntity));
            AddTrait(typeof(Location));
            AddTrait(typeof(Rotation));
            AddTrait(typeof(Scale));
            AddFeature(typeof(UEntityLoader));
            AddFeature(typeof(UEntityUpdater));
        }
    }
}
