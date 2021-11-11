/*
 * This file is part of Taichi project.
 *
 * (c) MuGuangyi <muguangyi@hotmail.com>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 */

using System;

namespace Taichi.Pipeline.Editor.Internal
{
    internal interface IPipeline : ICloneable
    {
        string Name { get; set; }
        IStep[] Steps { get; }

        void AddStep(IStep step);
        bool RemoveStep(IStep step);
        void SwapSteps(int i, int j);

        PipeLineData Export();
        void Import(PipeLineData data);
    }

    [Serializable]
    internal struct PipeLineData
    {
        public string Name;
        public StepData[] StepDatas;
    }
}
