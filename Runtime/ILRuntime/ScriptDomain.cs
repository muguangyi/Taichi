/*
 * This file is part of Taichi project.
 *
 * (c) MuGuangyi <muguangyi@hotmail.com>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 */

using ILRuntime.Mono.Cecil.Pdb;
using Taichi.ILRuntime.Internal;
using System;
using System.IO;
using System.Reflection;
using System.Threading;
using UnityEngine;
using ILDomain = global::ILRuntime.Runtime.Enviorment.AppDomain;
using Taichi.Async;

namespace Taichi.ILRuntime
{
    public sealed class ScriptDomain : IScriptDomain
    {
        private const string CLRBindingsType = "ILRuntime.Runtime.Generated.CLRBindings";
        private const string CLRDelegateBindingsType = "Taichi.ILRuntime.Generated.CLRDelegateBindings";
        private const string InitializeMethod = "Initialize";

        private class AsyncLoadAssembly : Async.Async
        {
            private readonly ILDomain domain = null;
            private readonly IAsync<byte[]> asm = null;
            private readonly IAsync<byte[]> pdb = null;

            public AsyncLoadAssembly(ILDomain domain, IAsync<byte[]> asm, IAsync<byte[]> pdb = null)
            {
                this.domain = domain;
                this.asm = asm;
                this.pdb = pdb;
            }

            protected override AsyncState OnUpdate()
            {
                if (this.asm.IsCompleted && (this.pdb == null || this.pdb.IsCompleted))
                {
                    this.domain.LoadAssembly(
                        new MemoryStream(this.asm.GetResult()),
                        this.pdb != null ? new MemoryStream(this.pdb.GetResult()) : null,
                        new PdbReaderProvider());
                    return AsyncState.Succeed;
                }

                return AsyncState.Running;
            }
        }

        private readonly ILDomain domain = new ILDomain();

        public bool ScriptMode { get; set; } = true;

        public IScriptLoader Loader { private get; set; } = null;

        public void LoadAssembly(string assembly)
        {
            if (!this.ScriptMode)
            {
                return;
            }

            if (this.Loader == null)
            {
                throw new MissingFieldException("Loader is null, please set it first!");
            }

            var pdb = Debug.isDebugBuild ? this.Loader.Load(assembly + ".pdb") : null;
            this.domain.LoadAssembly(
                new MemoryStream(this.Loader.Load(assembly)),
                pdb != null ? new MemoryStream(pdb) : null,
                new PdbReaderProvider());
        }

        public IAsync LoadAssemblyAsync(string assembly)
        {
            if (!this.ScriptMode)
            {
                return Async.Async.Null;
            }

            if (this.Loader == null)
            {
                throw new MissingFieldException("Loader is null, please set it first!");
            }

            return new AsyncLoadAssembly(
                this.domain,
                this.Loader.LoadAsync(assembly),
                Debug.isDebugBuild ? this.Loader.LoadAsync(assembly + ".pdb") : null);
        }

        public void Start(string type, string staticMethod)
        {
            if (this.ScriptMode)
            {
                Prepare();

                this.domain.Invoke(type, staticMethod, null);
            }
            else
            {
                var entryType = FindType(type);
                if (entryType == null)
                {
                    throw new MissingReferenceException(type);
                }

                var method = entryType.GetMethod(staticMethod, BindingFlags.Public | BindingFlags.Static);
                if (method == null)
                {
                    throw new MissingFieldException(staticMethod);
                }

                method.Invoke(null, null);
            }
        }

        private void Prepare()
        {
#if DEBUG && (UNITY_EDITOR || UNITY_ANDROID || UNITY_IPHONE)
            this.domain.UnityMainThreadID = Thread.CurrentThread.ManagedThreadId;
#endif

            if (Debug.isDebugBuild)
            {
                if (this.domain.DebugService.IsDebuggerAttached)
                {
                    this.domain.DebugService.StopDebugService();
                }

                this.domain.DebugService.StartDebugService(56000);
            }

            // Extend Assembler for ILRuntime.
            ScriptExtension.Extend(this.domain);

            // Setup environment.
            ScriptEnvironment.Setup(this.domain);

            // Try to enable CLR binding by searching all assemblies.
            var asms = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var a in asms)
            {
                var t = a.GetType(CLRBindingsType);
                if (t != null)
                {
                    var init = t.GetMethod(InitializeMethod, BindingFlags.Public | BindingFlags.Static);
                    init?.Invoke(null, new object[] { this.domain });
                    break;
                }
            }

            foreach (var a in asms)
            {
                var types = a.GetTypes();
                var t = a.GetType(CLRDelegateBindingsType);
                if (t != null)
                {
                    var init = t.GetMethod(InitializeMethod, BindingFlags.Public | BindingFlags.Static);
                    init?.Invoke(null, new object[] { this.domain });
                    break;
                }
            }
        }

        private static Type FindType(string module)
        {
            var t = Type.GetType(module);
            if (t == null)
            {
                var assemblies = AppDomain.CurrentDomain.GetAssemblies();
                foreach (var asm in assemblies)
                {
                    t = asm.GetType(module);
                    if (t != null)
                    {
                        break;
                    }
                }
            }

            return t;
        }
    }
}
