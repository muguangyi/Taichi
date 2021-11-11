/*
 * This file is part of Taichi project.
 *
 * (c) MuGuangyi <muguangyi@hotmail.com>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 */

using Taichi.Binding;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Taichi.UI
{
    public class UnityNode : ValueNode
    {
        private sealed class UnityEventHandler
        {
            private readonly UnityNode node = null;

            public UnityEventHandler(UnityNode node, UnityEvent evt)
            {
                this.node = node;
                evt.AddListener(OnEventAction);
            }

            private void OnEventAction()
            {
                this.node.RaiseNotifyChanged(this.node.behaviour, new ValueChangedArgs(""));
            }
        }

        private sealed class UnityEventHandler<T>
        {
            private readonly UnityNode node = null;

            public UnityEventHandler(UnityNode node, UnityEvent<T> evt)
            {
                this.node = node;
                evt.AddListener(OnEventAction);
            }

            private void OnEventAction(T value)
            {
                this.node.RaiseNotifyChanged(this.node.behaviour, new ValueChangedArgs("", value));
            }
        }

        private readonly Behaviour behaviour = null;

        public UnityNode(Behaviour behaviour, string name) : base(behaviour, name)
        {
            this.behaviour = behaviour;
            BindEventHandler(name);
        }

        private void BindEventHandler(string evt)
        {
            var t = this.behaviour.GetType();
            var p = t.GetProperty(evt);
            if (typeof(UnityEventBase).IsAssignableFrom(p.PropertyType))
            {
                var e = p.GetGetMethod().Invoke(this.behaviour, null);
                var parameterTypes = GetEventParametersType(p.PropertyType);
                switch (parameterTypes.Length)
                {
                case 0:
                    new UnityEventHandler(this, (UnityEvent)e);
                    break;
                case 1:
                    Activator.CreateInstance(typeof(UnityEventHandler<>).MakeGenericType(parameterTypes[0]), this, e);
                    break;
                }
            }
        }

        private Type[] GetEventParametersType(Type type)
        {
            var info = type.GetMethod("Invoke");
            if (info == null)
            {
                return new Type[0];
            }

            var parameters = info.GetParameters();
            if (parameters == null || parameters.Length <= 0)
            {
                return new Type[0];
            }

            return parameters.Select(a => a.ParameterType).ToArray();
        }
    }
}
