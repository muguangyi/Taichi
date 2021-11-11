/*
 * This file is part of Taichi project.
 *
 * (c) MuGuangyi <muguangyi@hotmail.com>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 */

using System;
using System.Collections.Generic;
using Taichi.Async;
using Taichi.Binding;
using UnityEngine;

namespace Taichi.UI
{
    public abstract class Window : View, IWindow
    {
        private sealed class AsyncLoadWindow : Async<IWindow>
        {
            private readonly Window window = null;
            private readonly IAsync<GameObject> request = null;

            public AsyncLoadWindow(Window window)
            {
                SetResult(this.window = window);
                Wait(this.request = window.Provider.LoadAsync(window.Name));
            }

            protected override bool OnStart()
            {
                this.window.GameObject = this.request.GetResult();
                this.window.state = WindowState.Opened;
                this.window.OnOpen();

                return true;
            }
        }

        private sealed class AsyncWaitWindow : Async<IWindow>
        {
            private readonly Window window = null;

            public AsyncWaitWindow(Window window)
            {
                SetResult(this.window = window);
            }

            protected override AsyncState OnUpdate()
            {
                return this.window.state == WindowState.Opened ? AsyncState.Succeed : AsyncState.Running;
            }
        }

        private enum WindowState
        {
            Unloaded,
            Loading,
            Opened,
            Closed,
        }

        private WindowState state = WindowState.Unloaded;
        private readonly Dictionary<string, Widget> widgets = new Dictionary<string, Widget>();
        private readonly Dictionary<string, Window> windows = new Dictionary<string, Window>();

        public string Name { get; internal set; } = string.Empty;

        public void OpenWindow(string window, Type windowView = null)
        {
            if (!this.windows.TryGetValue(window, out Window w))
            {
                this.windows.Add(window, w = (Window)Activator.CreateInstance(windowView ?? typeof(EmptyWindow)));
                w.Name = window;
                w.Provider = this.Provider;
            }

            w.OpenAsync();
        }

        public void CloseWindow(string window)
        {
            if (this.windows.TryGetValue(window, out Window w))
            {
                w.Close();
                this.windows.Remove(window);
            }
        }

        protected virtual void OnOpen()
        { }

        protected virtual void OnClose()
        { }

        internal IViewProvider Provider { private get; set; } = null;

        internal IAsync<IWindow> OpenAsync()
        {
            var wait = new AsyncWaitWindow(this);
            if (this.state == WindowState.Unloaded)
            {
                this.state = WindowState.Loading;
                wait.Wait(new AsyncLoadWindow(this));
            }

            return wait;
        }

        internal void Close()
        {

        }
    }

    public abstract class Window<TViewModel> : Window where TViewModel : ViewModel, new()
    {
        protected TViewModel ViewModel { get; private set; } = new TViewModel();

        protected void Bind(INotifyCaller caller, INotifyCallee callee)
        {
            caller.NotifyChanged += callee.Notify;
        }
    }
}
