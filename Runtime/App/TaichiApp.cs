/*
 * This file is part of Taichi project.
 *
 * (c) MuGuangyi <muguangyi@hotmail.com>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 */

using Taichi.Foundation;
using Taichi.Gameplay;
using Taichi.Logger;
using Taichi.Asset;
using Taichi.Console;
using Taichi.Coroutine;
using Taichi.ILRuntime;
using Taichi.UI;
using UnityEngine;

namespace Taichi.App
{
    public sealed class TaichiApp : MonoBehaviour
    {
        [Resolve] private static IAssetFactory factory = null;
        [Resolve] private static IScriptDomain domain = null;
        [Resolve] private static IScreenManager manager = null;

        [Header("Log Settings")]
        public LogLevel LogLevel = LogLevel.Fatal;

        [Header("IL Script Load Mode")]
        public bool ScriptMode = true;

        [Header("Asset Settings")]
        public bool EditorBundleMode = false;

        private void Awake()
        {
            Log.Enabled = Debug.isDebugBuild;
            Log.Configure.Unity(LogLevel);

            // MonoDriver
            Assembler.ImportModule<IMonoDriver, MonoDriver>();

            // Console
            Assembler.ImportModule<IAppConsole, AppConsole>(true);

            // Asset
            Assembler.ImportModule<IAssetFactory, AssetFactory>();
            Assembler.ImportModule<IGObjectFactory, GObjectFactory>();
            Assembler.ImportModule<IEntityFactory, EntityFactory>();

            // UI
            Assembler.ImportModule<IScreenManager, ScreenManager>();

            // Gameplay
            Assembler.ImportModule<IGameInstance, UnityGameInstance>(true);

            // ILRuntime
            Assembler.ImportModule<IScriptDomain, ScriptDomain>();

            Assembler.ImportModuleInstance<TaichiApp, TaichiApp>(this);

            DontDestroyOnLoad(this);
        }

        private void OnResolve()
        {
            factory.EditorBundleMode = this.EditorBundleMode;
            domain.ScriptMode = this.ScriptMode;
            manager.EditorBundleMode = this.EditorBundleMode;
        }

        private void Update()
        {
            Assembler.Tick(Time.deltaTime);
        }
    }
}
