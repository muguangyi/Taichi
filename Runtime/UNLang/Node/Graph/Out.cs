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
    /// Out module to define a sub graph return value.
    /// </summary>
    [NodeInterface("Out", "UNLang/Graph/")]
    [GraphInterface(SpotType.Out)]
    public sealed class Out : LangNode
    {
        private LangSpot s = null;
        private LangSpot ss = null;

        public override void Init()
        {
            this.Type = new LangType();
            this.Type.Dispatcher.AddListener(LangType.CHANGE, OnNotify);

            Add(this.s = new LangSpot("", LangType.Category.Any, this, -1, SpotType.In));
            Add(this.ss = new LangSpot("Out", LangType.Category.Any, this, 1, SpotType.Out));
        }

        public override void OnSignal(Spot spot, params object[] args)
        {
            this.ss.Signal(args);
        }

        public void OnChange(string spotName)
        {
            this.ss.Name = spotName;
        }

        public string SpotName
        {
            get
            {
                return this.ss.Name;
            }
        }

        public LangType Type { get; private set; } = null;

        private void OnNotify(object target, Message message)
        {
            this.s.OnChangeTypeCategory(this.Type.Type);
            this.ss.OnChangeTypeCategory(this.Type.Type);
        }
    }
}