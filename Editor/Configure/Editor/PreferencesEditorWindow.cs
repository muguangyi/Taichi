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
using UnityEngine;

namespace Taichi.Configure.Editor
{
    public sealed class PreferencesEditorWindow : EditorWindow
    {
        private const float RowHeight = 25;
        private const float Marge = 10;
        private static readonly GUILayoutOption RowHeightOption = GUILayout.Height(RowHeight);
        private static readonly GUILayoutOption ButtonWidth = GUILayout.Width(100);

        [MenuItem("[ Taichi ]/Preferences...", priority = MenuPriority.Preferences)]
        private static void OnOpen()
        {
            var window = GetWindow<PreferencesEditorWindow>("Preferences");

            var guids = AssetDatabase.FindAssets("t:" + typeof(Settings));
            window.minSize = new Vector2(400, (guids.Length + 5) * (RowHeight + Marge));
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("Settings:");
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            {
                var guids = AssetDatabase.FindAssets("t:" + typeof(RuntimeSettings));
                if (guids.Length > 0)
                {
                    EditorGUILayout.LabelField("Runtime");
                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    {
                        for (var i = 0; i < guids.Length; ++i)
                        {
                            var path = AssetDatabase.GUIDToAssetPath(guids[i]);
                            var asset = AssetDatabase.LoadAssetAtPath(path, typeof(RuntimeSettings));
                            EditorGUILayout.BeginHorizontal(EditorStyles.textField);
                            {
                                EditorGUILayout.LabelField(asset.name, RowHeightOption);
                                if (GUILayout.Button("Open", ButtonWidth, RowHeightOption))
                                {
                                    AssetDatabase.OpenAsset(asset);
                                }
                            }
                            EditorGUILayout.EndHorizontal();
                        }
                    }
                    EditorGUILayout.EndVertical();

                    EditorGUILayout.Separator();
                }

                guids = AssetDatabase.FindAssets("t:" + typeof(EditorSettings));
                if (guids.Length > 0)
                {
                    EditorGUILayout.LabelField("Editor");
                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    {
                        for (var i = 0; i < guids.Length; ++i)
                        {
                            var path = AssetDatabase.GUIDToAssetPath(guids[i]);
                            var asset = AssetDatabase.LoadAssetAtPath(path, typeof(EditorSettings));
                            EditorGUILayout.BeginHorizontal(EditorStyles.textField);
                            {
                                EditorGUILayout.LabelField(asset.name, RowHeightOption);
                                if (GUILayout.Button("Open", ButtonWidth, RowHeightOption))
                                {
                                    AssetDatabase.OpenAsset(asset);
                                }
                            }
                            EditorGUILayout.EndHorizontal();
                        }
                    }
                    EditorGUILayout.EndVertical();
                }
            }
            EditorGUILayout.EndVertical();
        }
    }
}
