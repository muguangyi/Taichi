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
    public abstract class Feature : IFeature
    {
        public IActor Actor { get; internal set; } = null;

        internal void Init()
        {
            OnInit();
        }

        internal void Update(float deltaTime)
        {
            OnUpdate(deltaTime);
        }

        internal void Destroy()
        {
            OnDestroy();

            this.Actor = null;
        }

        protected virtual void OnInit()
        { }

        protected virtual void OnUpdate(float deltaTime)
        { }

        protected virtual void OnDestroy()
        { }
    }
}
