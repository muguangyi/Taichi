/*
 * This file is part of Taichi project.
 *
 * (c) MuGuangyi <muguangyi@hotmail.com>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 */

using ILRuntime.Runtime.Intepreter;
using System.Runtime.CompilerServices;

namespace Taichi.ILRuntime.Adaptor
{
    [ScriptAdaptor]
    public sealed class AsyncStateMachineAdaptor : IAsyncStateMachine, IScriptAdaptor
    {
        private readonly IScriptContext context = null;
        private readonly IScriptMethod methodMoveNext = null;
        private readonly IScriptMethod methodSetStateMachine = null;

        public AsyncStateMachineAdaptor(IScriptContext context)
        {
            this.context = context;
            this.methodMoveNext = context.GetMethod("MoveNext");
            this.methodSetStateMachine = context.GetMethod("SetStateMachine", 1);
        }

        public ILTypeInstance ILInstance => this.context.Instance;

        public void MoveNext()
        {
            this.methodMoveNext?.Invoke();
        }

        public void SetStateMachine(IAsyncStateMachine stateMachine)
        {
            this.methodSetStateMachine?.Invoke(stateMachine);
        }
    }
}
