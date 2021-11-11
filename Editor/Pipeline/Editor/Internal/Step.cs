/*
 * This file is part of Taichi project.
 *
 * (c) MuGuangyi <muguangyi@hotmail.com>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 */

namespace Taichi.Pipeline.Editor.Internal
{
    internal class Step : IStep
    {
        public Step(string name = null)
        {
            this.Name = name;
        }

        public string Name { get; set; } = null;
        public IMethod Method { get; private set; } = new Method();
        public bool Selected { get; set; } = true;

        public object Clone()
        {
            var clone = new Step();
            clone.Name = this.Name;
            clone.Method = (IMethod)this.Method.Clone();
            clone.Selected = this.Selected;

            return clone;
        }

        public StepData Export()
        {
            return new StepData
            {
                Name = this.Name,
                MethodData = this.Method.Export(),
                Selected = this.Selected,
            };
        }

        public void Import(StepData data)
        {
            this.Name = data.Name;
            this.Method.Import(data.MethodData);
            this.Selected = data.Selected;
        }
    }
}
