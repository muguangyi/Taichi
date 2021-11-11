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
    public interface IViewableActor : IActor
    {
        float X { get; }
        float Y { get; }
        float Z { get; }
        float Roll { get; }
        float Pitch { get; }
        float Yaw { get; }
        float ScaleX { get; }
        float ScaleY { get; }
        float ScaleZ { get; }

        IViewableNode Retrieve(string path = null);
        void SetLocation(float x, float y, float z = 0);
        void SetRotation(float roll, float pitch, float yaw);
        void SetScale(float x, float y, float z);
    }
}
