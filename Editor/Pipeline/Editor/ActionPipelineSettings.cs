/*
 * This file is part of Taichi project.
 *
 * (c) MuGuangyi <muguangyi@hotmail.com>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 */

using Taichi.Configure;
using Taichi.Pipeline.Editor.Internal;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

namespace Taichi.Pipeline.Editor
{
    public sealed class ActionPipelineSettings : EditorSettings<ActionPipelineSettings>
    {
        [SerializeField]
        [HideInInspector]
        private PipeLineData[] pDatas;

        [SerializeField]
        [HideInInspector]
        private StepData[] eDatas;

        private readonly List<IPipeline> pipelines = new List<IPipeline>();

        public override void OnBeforeSerialize()
        {
            this.pDatas = new PipeLineData[this.Pipelines.Count];
            for (var i = 0; i < this.pDatas.Length; ++i)
            {
                this.pDatas[i] = this.pipelines[i].Export();
            }

            this.eDatas = new StepData[this.ExcutingSteps.Count];
            for (var i = 0; i < this.eDatas.Length; ++i)
            {
                this.eDatas[i] = this.ExcutingSteps.Dequeue().Export();
            }
        }

        public override void OnInit()
        {
            for (var i = 0; i < this.pDatas.Length; ++i)
            {
                var p = new Internal.Pipeline();
                p.Import(pDatas[i]);
                this.pipelines.Add(p);
            }

            for (var i = 0; i < this.eDatas.Length; ++i)
            {
                var s = new Step();
                s.Import(this.eDatas[i]);
                this.ExcutingSteps.Enqueue(s);
            }
        }

        internal IList Pipelines => this.pipelines;
        internal Queue<IStep> ExcutingSteps { get; } = new Queue<IStep>();

        internal IPipeline AddPipeline(IPipeline pipeline)
        {
            this.pipelines.Add(pipeline);

            return pipeline;
        }

        internal bool RemovePipeline(IPipeline pipeline)
        {
            return pipeline != null ? this.pipelines.Remove(pipeline) : false;
        }

        internal IPipeline FindPipeline(string name)
        {
            foreach (var p in this.pipelines)
            {
                if (p.Name == name)
                {
                    return p;
                }
            }

            return null;
        }
    }
}
