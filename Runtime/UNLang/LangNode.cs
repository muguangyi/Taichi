/*
 * This file is part of Taichi project.
 *
 * (c) MuGuangyi <muguangyi@hotmail.com>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 */

using Taichi.UNode;

namespace Taichi.UNLang
{
    /// <summary>
    /// Abstract base script node of UNLang.
    /// </summary>
    public abstract class LangNode : Node
    {
        public override void Init()
        { }

        public override byte[] Export()
        {
            return null;
        }

        public override void Import(byte[] data)
        { }

        public override void Loaded()
        { }

        public override void OnSignal(Spot spot, params object[] args)
        { }

        public void Begin(LangInstance instance)
        {
            this.Instance = instance;
            OnBegin();
        }

        protected virtual void OnBegin()
        { }

        protected LangInstance Instance { get; private set; } = null;
    }
}