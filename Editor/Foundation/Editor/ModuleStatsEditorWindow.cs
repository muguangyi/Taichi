/*
 * This file is part of Taichi project.
 *
 * (c) MuGuangyi <muguangyi@hotmail.com>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 */

using System.Collections;
using Taichi.Editor;
using Taichi.Foundation.Internal;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Taichi.Foundation.Editor
{
    public sealed class ModuleStatsEditorWindow : EditorWindow
    {
        private IList modules = null;
        private ReorderableList list = null;

        [MenuItem("[ Taichi ]/Analysis/Module Stats...", priority = MenuPriority.Analysis)]
        private static void OnOpen()
        {
            var window = GetWindow<ModuleStatsEditorWindow>("Module Stats");
            window.minSize = new Vector2(800, 400);
        }

        private void OnEnable()
        {
            Refresh();
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        private void OnGUI()
        {
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.BeginVertical(GUILayout.Width(200));
                {
                    this.list.DoLayoutList();
                }
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndHorizontal();
        }

        private void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            Refresh();
        }

        private void Refresh()
        {
            this.modules = Assembler.Modules;
            this.list = new ReorderableList(this.modules, typeof(IModule), false, false, false, false);
            this.list.drawElementCallback = OnDrawElementCallback;
        }

        private void OnDrawElementCallback(Rect rect, int index, bool isActive, bool isFocused)
        {
            var module = (IModule)this.modules[index];
            GUI.Label(rect, module.Type.Name);
        }
    }
}
