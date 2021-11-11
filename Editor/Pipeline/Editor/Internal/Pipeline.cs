/*
 * This file is part of Taichi project.
 *
 * (c) MuGuangyi <muguangyi@hotmail.com>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 */

using System;
using System.Collections.Generic;
using System.Linq;

namespace Taichi.Pipeline.Editor.Internal
{
    internal class Pipeline : IPipeline
    {
        private readonly List<IStep> steps = new List<IStep>();

        public Pipeline(string name = null)
        {
            this.Name = name;
        }

        public string Name { get; set; } = "";

        public IStep[] Steps => this.steps.ToArray();

        public object Clone()
        {
            var clone = new Pipeline();
            clone.Name = $"{this.Name} Copy";
            for (var i = 0; i < this.steps.Count; ++i)
            {
                clone.steps.Add((IStep)this.steps[i].Clone());
            }

            return clone;
        }

        public void AddStep(IStep step)
        {
            this.steps.Add(step);
        }

        public bool RemoveStep(IStep step)
        {
            return this.steps.Remove(step);
        }

        public void SwapSteps(int i, int j)
        {
            if (i < 0 || i >= this.steps.Count || j < 0 || j >= this.steps.Count || i == j)
            {
                return;
            }

            var t = this.steps[i];
            this.steps[i] = this.steps[j];
            this.steps[j] = t;
        }

        public PipeLineData Export()
        {
            return new PipeLineData
            {
                Name = this.Name,
                StepDatas = this.steps.Select(s => s.Export()).ToArray(),
            };
        }

        public void Import(PipeLineData data)
        {
            this.Name = data.Name;
            foreach (var d in data.StepDatas)
            {
                var s = new Step();
                s.Import(d);
                this.steps.Add(s);
            }
        }
    }
}
