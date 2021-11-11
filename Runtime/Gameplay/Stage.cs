/*
 * This file is part of Taichi project.
 *
 * (c) MuGuangyi <muguangyi@hotmail.com>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 */

using System;

namespace Taichi.Gameplay
{
    public class Stage : IStage
    {
        public event Action<IStage> Destroyed;
        public event Action<IStage, float> Tick;

        internal void Init()
        {
            OnInit();
        }

        internal void Update(float deltaTime)
        {
            OnUpdate(deltaTime);
            this.Tick?.Invoke(this, deltaTime);
        }

        protected virtual void OnInit()
        { }

        protected virtual void OnDestroy()
        { }

        protected virtual void OnUpdate(float deltaTime)
        { }
    }
}
