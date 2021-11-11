/*
 * This file is part of Taichi project.
 *
 * (c) MuGuangyi <muguangyi@hotmail.com>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 */

using ILRuntime.Runtime.Intepreter;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Taichi.ILRuntime.Adaptor
{
    [ScriptAdaptor]
    public sealed class CoroutineAdaptor : IEnumerator<object>, IEnumerator, IDisposable, IScriptAdaptor
    {
        private readonly IScriptContext context = null;
        private readonly IScriptMethod methodCurrent = null;
        private readonly IScriptMethod methodDispose = null;
        private readonly IScriptMethod methodMoveNext = null;
        private readonly IScriptMethod methodReset = null;

        public CoroutineAdaptor(IScriptContext context)
        {
            this.context = context;
            this.methodCurrent = context.GetMethod("get_Current") ?? context.GetMethod("System.Collections.IEnumerator.get_Current");
            this.methodDispose = context.GetMethod("Dispose") ?? context.GetMethod("System.IDisposable.Dispose");
            this.methodMoveNext = context.GetMethod("MoveNext");
            this.methodReset = context.GetMethod("Reset");
        }

        public ILTypeInstance ILInstance => this.context.Instance;

        public object Current => this.methodCurrent?.Invoke();

        public void Dispose()
        {
            this.methodDispose?.Invoke();
        }

        public bool MoveNext()
        {
            return this.methodMoveNext != null ? (bool)this.methodMoveNext.Invoke() : false;
        }

        public void Reset()
        {
            this.methodReset?.Invoke();
        }

        public override string ToString()
        {
            return this.context.ToString();
        }
    }
}
