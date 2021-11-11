/*
 * This file is part of Taichi project.
 *
 * (c) MuGuangyi <muguangyi@hotmail.com>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 */

using System;
using System.Reflection;

namespace Taichi.Pipeline.Editor.Internal
{
    internal interface IMethod : ICloneable
    {
        string Name { get; }
        bool IsValid { get; }
        string[] ArgNames { get; }
        Type[] ArgTypes { get; }
        MethodInfo MethodInfo { set; }

        object GetArg(int index);
        void SetArg(int index, object value);
        void Invoke();

        MethodData Export();
        void Import(MethodData data);
    }

    [Serializable]
    internal struct MethodData
    {
        public string TypeName;
        public string MethodName;
        public string[] ArgNames;
        public string[] ArgTypes;
        public byte[] Args;
    }
}
