/*
 * This file is part of Taichi project.
 *
 * (c) MuGuangyi <muguangyi@hotmail.com>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 */

using System.Collections.Generic;
using UnityEngine;

namespace Taichi.UNode
{
    public static class NodeVM
    {
        private static Dictionary<string, LinkedList<NodeRunner>> runnercache = new Dictionary<string, LinkedList<NodeRunner>>();
        private static INodeRunnerDebugger debugger = null;
        private static Dictionary<string, HashSet<int>> hashRunners = new Dictionary<string, HashSet<int>>();
        private static Dispatcher dispatcher = new Dispatcher(null);

        public static void Preload(string script, byte[] code)
        {
            var runner = new NodeRunner();
            runner.Import(code);
            Drop(script, runner);
        }

        public static NodeRunner Pick(string script)
        {
            NodeRunner runner = null;
            if (!runnercache.TryGetValue(script, out LinkedList<NodeRunner> runners) || 0 == runners.Count)
            {
                var code = NodeLoader.Load(script);
                runner = new NodeRunner();
                runner.Import(code);
            }
            else
            {
                runner = runners.First.Value;
                runners.RemoveFirst();
            }

            // Debug feature
            if (Application.isEditor)
            {
                if (!hashRunners.TryGetValue(script, out HashSet<int> hashCodes))
                {
                    hashRunners.Add(script, hashCodes = new HashSet<int>());
                }
                hashCodes.Add(runner.GetHashCode());

                dispatcher.AddListener(NodeRunnerDebugMessage.START, runner.NotifyHandler);
                if (null != debugger)
                {
                    debugger.Attach(runner);
                }
            }

            runner.Recover();
            return runner;
        }

        public static void Drop(string script, NodeRunner runner)
        {
            if (!runnercache.TryGetValue(script, out LinkedList<NodeRunner> runners))
            {
                runnercache.Add(script, runners = new LinkedList<NodeRunner>());
            }
            runners.AddLast(runner);

            // Debug feature
            if (Application.isEditor)
            {
                if (hashRunners.TryGetValue(script, out HashSet<int> hashCodes))
                {
                    hashCodes.Remove(runner.GetHashCode());
                }

                dispatcher.RemoveListener(NodeRunnerDebugMessage.START, runner.NotifyHandler);
                if (null != debugger)
                {
                    debugger.Detach(runner);
                }
            }

            runner.Reclaim();
        }

        public static void Destroy()
        {
            foreach (var cache in runnercache)
            {
                var runners = cache.Value;
                while (null != runners.First)
                {
                    runners.First.Value.Clear();
                    runners.RemoveFirst();
                }
            }
            runnercache = null;

            StopDebugger();

            if (null != dispatcher)
            {
                dispatcher.Dispose();
                dispatcher = null;
            }

            hashRunners = null;
        }

        public static INodeRunnerDebugger StartDebugger(string script)
        {
            if (Application.isEditor && null == debugger)
            {
                debugger = new NodeRunnerDebugger(script);
                dispatcher.Notify(new NodeRunnerDebugMessage(debugger));
            }

            return debugger;
        }

        public static void StopDebugger()
        {
            if (Application.isEditor && null != debugger)
            {
                debugger.Dispose();
                debugger = null;
            }
        }

        public static bool IsDebugging
        {
            get
            {
                return (null != debugger);
            }
        }

        public sealed class NodeRunnerDebugMessage : Message
        {
            public const string START = "debug.start";

            public NodeRunnerDebugMessage(INodeRunnerDebugger debugger) : base(START)
            {
                this.Debugger = debugger;
            }

            public INodeRunnerDebugger Debugger { get; } = null;
        }

        private sealed class NodeRunnerDebugger : INodeRunnerDebugger
        {
            private readonly string script = null;
            private HashSet<NodeRunner> runners = new HashSet<NodeRunner>();

            public NodeRunnerDebugger(string script)
            {
                this.script = script;
            }

            public void Dispose()
            {
                this.runners = null;
            }

            public void Attach(NodeRunner runner)
            {
                foreach (var item in hashRunners)
                {
                    if (this.script.Contains(item.Key) && item.Value.Contains(runner.GetHashCode()))
                    {
                        this.runners.Add(runner);
                        break;
                    }
                }
            }

            public void Detach(NodeRunner runner)
            {
                this.runners.Remove(runner);
            }

            public IEnumerator<NodeRunner> Runners
            {
                get
                {
                    return this.runners.GetEnumerator();
                }
            }
        }
    }
}
