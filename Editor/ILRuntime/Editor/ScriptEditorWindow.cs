/*
 * This file is part of Taichi project.
 *
 * (c) MuGuangyi <muguangyi@hotmail.com>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 */

using Taichi.Editor;
using UnityEditor;

namespace Taichi.ILRuntime.Editor
{
    public sealed class ScriptEditorWindow : EditorWindow
    {
        [MenuItem("[ Taichi ]/ILRuntime/Build", priority = MenuPriority.ILRuntime)]
        private static void ForceGenWrappers()
        {
            var settings = ILRuntimeSettings.Fetch();
            if (settings.Assemblies.Length == 0)
            {
                if (EditorUtility.DisplayDialog("IL", "No IL assemblies need to be build, please check ILRuntimeSettings!", "Open ILRuntimeSettings..."))
                {
                    AssetDatabase.OpenAsset(settings);
                }
                return;
            }

            EditorUtility.DisplayProgressBar("IL", "Generate script related code ...", 0);
            {
                ScriptEditor.Generate();
                AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
            }
            EditorUtility.ClearProgressBar();
        }

        [MenuItem("[ Taichi ]/ILRuntime/Clean", priority = MenuPriority.ILRuntime)]
        private static void ForceClean()
        {
            EditorUtility.DisplayProgressBar("IL", "Clean script related code ...", 0);
            {
                ScriptEditor.ForceClean();
                AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
            }
            EditorUtility.ClearProgressBar();
        }
    }
}

