/*
 * This file is part of Taichi project.
 *
 * (c) MuGuangyi <muguangyi@hotmail.com>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 */

using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;
using System;
using System.Linq;

namespace Taichi.ILRuntime
{
    internal sealed class ScriptBindingAdaptor : CrossBindingAdaptor
    {
        private readonly Type adaptorType = null;

        public ScriptBindingAdaptor(Type adaptorType)
        {
            this.adaptorType = adaptorType ?? throw new ArgumentNullException("adaptorType");
        }

        public override Type BaseCLRType => this.adaptorType.BaseType != typeof(object) ? this.adaptorType.BaseType : null;

        public override Type[] BaseCLRTypes => this.adaptorType.GetInterfaces().Where(t => t != typeof(CrossBindingAdaptorType) && t != typeof(IScriptAdaptor)).ToArray();

        public override Type AdaptorType => this.adaptorType;

        public override object CreateCLRInstance(global::ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance)
        {
            return Activator.CreateInstance(this.adaptorType, new ScriptContext(appdomain, instance));
        }
    }
}
