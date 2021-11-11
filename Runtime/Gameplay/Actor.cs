/*
 * This file is part of Taichi project.
 *
 * (c) MuGuangyi <muguangyi@hotmail.com>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 */

using Taichi.Foundation;
using Taichi.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Taichi.Gameplay
{
    public class Actor : Atom, IActor
    {
        [Resolve] private static IGameInstance game = null;

        private IStage stage = null;
        private readonly Dictionary<string, Feature> features = new Dictionary<string, Feature>();

        public Actor(uint index = 0) : base(index)
        {
            OnInit();

            this.stage = game.Stage ?? throw new InvalidOperationException("the actor is not on a valid stage");
            OnStage();
        }

        public IStage Stage
        {
            get => this.stage;

            set
            {
                if (this.stage == value)
                {
                    return;
                }

                OffStage();
                this.stage = value;
                OnStage();
            }
        }

        public override void Dispose()
        {
            OffStage();
            OnDestroy();

            base.Dispose();
        }

        public bool AddFeature(Type featureType)
        {
            if (!featureType.IsSubclassOf(typeof(Feature)))
            {
                return false;
            }

            var name = featureType.Name;
            if (this.features.ContainsKey(name))
            {
                return false;
            }

            var feature = (Feature)Activator.CreateInstance(featureType);
            if (feature == null)
            {
                return false;
            }

            var dep = MissFeatureDependency(feature);
            if (!string.IsNullOrEmpty(dep))
            {
                throw new MissingFieldException($"<{name}> behaviour depenency - <{dep}> is missing");
            }

            feature.Actor = this;
            feature.Init();
            this.features.Add(name, feature);

            StartFeature(feature);

            return true;
        }

        public bool RemoveFeature(Type featureType)
        {
            var name = featureType.Name;
            if (this.features.TryGetValue(name, out Feature feature))
            {
                ClearTargetHandlers(feature);
                feature.Destroy();
                return true;
            }

            return false;
        }

        protected virtual void OnInit()
        { }

        protected virtual void OnUpdate(float deltaTime)
        { }

        protected virtual void OnDestroy()
        { }

        private void OnStage()
        {
            if (this.stage != null)
            {
                this.stage.Destroyed += OnStageDestroy;
                this.stage.Tick += OnStageTick;
            }
        }

        private void OffStage()
        {
            if (this.stage != null)
            {
                this.stage.Tick -= OnStageTick;
                this.stage.Destroyed -= OnStageDestroy;
            }
        }

        private void OnStageDestroy(IStage stage)
        {
            if (this.stage == stage)
            {
                Dispose();
            }
        }

        private void OnStageTick(IStage stage, float deltaTime)
        {
            OnUpdate(deltaTime);

            foreach (var i in this.features)
            {
                i.Value?.Update(deltaTime);
            }
        }

        private string MissFeatureDependency(IFeature feature)
        {
            var t = feature.GetType();
            var attrs = t.GetCustomAttributes(typeof(RequireValueAttribute), true);
            for (var i = 0; i < attrs.Length; ++i)
            {
                var a = (RequireValueAttribute)attrs[i];
                if (!string.IsNullOrEmpty(a.Name) && !HasValue(a.Name, a.Mode))
                {
                    return a.Name;
                }
            }

            return null;
        }

        private void StartFeature(Feature feature)
        {
            var t = feature.GetType();

            var trackMethods = t.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                                .Where(m => m.GetCustomAttribute(typeof(OnChangeValueAttribute), false) != null)
                                .ToArray();
            foreach (var m in trackMethods)
            {
                try
                {
                    var att = (OnChangeValueAttribute)m.GetCustomAttribute(typeof(OnChangeValueAttribute), false);
                    var action = (ValueChangeHandler)Delegate.CreateDelegate(typeof(ValueChangeHandler), feature, m);
                    AddValueChange(att.Name, action, feature);
                }
                catch (Exception ex)
                {
                    this.Error(ex);
                }
            }

            var startMethod = t.GetMethod("Start", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            startMethod?.Invoke(feature, null);
        }
    }
}
