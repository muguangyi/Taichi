/*
 * This file is part of Taichi project.
 *
 * (c) MuGuangyi <muguangyi@hotmail.com>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 */

using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;

namespace Taichi.ILRuntime
{
    internal sealed class ScriptContext : IScriptContext
    {
        private sealed class ScriptMethod : IScriptMethod
        {
            private readonly AppDomain domain = null;
            private readonly object instance = null;
            private readonly IMethod method = null;

            public ScriptMethod(AppDomain domain, object instance, IMethod method)
            {
                this.domain = domain;
                this.instance = instance;
                this.method = method;
            }

            public object Invoke(params object[] args)
            {
                return this.domain.Invoke(this.method, this.instance, args);
            }
        }

        public ScriptContext(AppDomain domain, ILTypeInstance instance)
        {
            this.Domain = domain;
            this.Instance = instance;
        }

        public AppDomain Domain { get; } = null;

        public ILTypeInstance Instance { get; } = null;

        public IScriptMethod GetMethod(string name, int paramCount = 0, bool declaredOnly = false)
        {
            var m = this.Instance.Type.GetMethod(name, paramCount, declaredOnly);
            return m != null ? new ScriptMethod(this.Domain, this.Instance, m) : null;
        }

        public override string ToString()
        {
            var m = this.Domain.ObjectType.GetMethod("ToString", 0);
            m = this.Instance.Type.GetVirtualMethod(m);
            if (m == null || m is ILMethod)
            {
                return this.Instance.ToString();
            }
            else
            {
                return this.Instance.Type.FullName;
            }
        }
    }
}
