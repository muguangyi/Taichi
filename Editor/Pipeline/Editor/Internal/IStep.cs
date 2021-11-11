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
    internal interface IStep : ICloneable
    {
        string Name { get; set; }
        IMethod Method { get; }
        bool Selected { get; set; }

        StepData Export();
        void Import(StepData data);
    }

    [Serializable]
    internal struct StepData
    {
        public string Name;
        public MethodData MethodData;
        public bool Selected;
    }
}
