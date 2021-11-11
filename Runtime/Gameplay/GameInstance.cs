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
    public class GameInstance : IGameInstance
    {
        private Stage stage = null;

        public IStage Stage => this.stage;

        public void Goto(IStage stage)
        {
            // TODO: Setup the process for switching stages.

            this.stage = (Stage)stage;
            this.stage?.Init();
        }

        protected virtual Type[] Contexts => new Type[]
        {
            typeof(Actor),
        };

        protected void OnUpdate(float deltaTime)
        {
            this.stage?.Update(deltaTime);
        }
    }
}

