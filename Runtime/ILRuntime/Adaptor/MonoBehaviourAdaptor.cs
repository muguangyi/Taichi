/*
 * This file is part of Taichi project.
 *
 * (c) MuGuangyi <muguangyi@hotmail.com>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 */

using ILRuntime.Runtime.Intepreter;
using UnityEngine;

namespace Taichi.ILRuntime.Adaptor
{
    [ScriptAdaptor]
    public sealed class MonoBehaviourAdaptor : MonoBehaviour, IScriptAdaptor
    {
        private readonly IScriptContext context = null;
        private IScriptMethod awakeMethod = null;
        private IScriptMethod startMethod = null;
        private IScriptMethod updateMethod = null;

        public MonoBehaviourAdaptor(IScriptContext context)
        {
            this.context = context;
            this.awakeMethod = this.context.GetMethod("Awake");
            this.startMethod = this.context.GetMethod("Start");
            this.updateMethod = this.context.GetMethod("Update");
        }

        public ILTypeInstance ILInstance => this.context.Instance;

        public void Awake()
        {
            this.awakeMethod?.Invoke();
        }

        public void Start()
        {
            this.startMethod?.Invoke();
        }

        public void Update()
        {
            this.updateMethod?.Invoke();
        }

        public override string ToString()
        {
            return this.context.ToString();
        }
    }
}
