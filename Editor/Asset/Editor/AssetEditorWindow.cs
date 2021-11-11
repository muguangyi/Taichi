/*
 * This file is part of Taichi project.
 *
 * (c) MuGuangyi <muguangyi@hotmail.com>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 */

using Taichi.Editor;
using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Taichi.Asset.Editor
{
    public sealed class AssetEditorWindow : EditorWindow
    {
        [MenuItem("[ Taichi ]/Asset/Asset Builder...", priority = MenuPriority.Asset)]
        private static void OpenWindow()
        {
            var window = GetWindowWithRect<AssetEditorWindow>(new Rect(0, 0, 600, 190), true);
            window.titleContent = new GUIContent("Asset Builder");
        }

        private BuildAssetBundleOptions BuildOptions
        {
            get => (BuildAssetBundleOptions)EditorPrefs.GetInt($"{PlayerSettings.productName}.Asset.BuildOptions", (int)BuildAssetBundleOptions.ChunkBasedCompression);
            set => EditorPrefs.SetInt($"{PlayerSettings.productName}.Asset.BuildOptions", (int)value);
        }

        private BuildTarget BuildPlatform
        {
            get => (BuildTarget)EditorPrefs.GetInt($"{PlayerSettings.productName}.Asset.BuildPlatform", (int)BuildTarget.Android);
            set => EditorPrefs.SetInt($"{PlayerSettings.productName}.Asset.BuildPlatform", (int)value);
        }

        private void OnGUI()
        {
            var backgroundColor = GUI.backgroundColor;

            EditorGUILayout.Separator();
            EditorGUILayout.Separator();

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            {
                EditorGUILayout.Separator();
                EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
                {
                    EditorGUILayout.LabelField("Asset Output Path:", GUILayout.Width(150));
                    EditorGUILayout.LabelField(Path.Combine(Environment.CurrentDirectory, AssetSettings.Fetch().OutputFolder));
                    if (GUILayout.Button("Open", GUILayout.Width(100)))
                    {
                        FileUtility.OpenDirectory(Path.Combine(Environment.CurrentDirectory, AssetSettings.Fetch().OutputFolder));
                    }
                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Separator();


                this.BuildOptions = (BuildAssetBundleOptions)EditorGUILayout.EnumPopup("Build Options:", this.BuildOptions);
                EditorGUILayout.Separator();

                this.BuildPlatform = (BuildTarget)EditorGUILayout.EnumPopup("Target Platform:", this.BuildPlatform);
                EditorGUILayout.Separator();

                EditorGUILayout.Space();

                EditorGUILayout.BeginHorizontal();

                GUI.backgroundColor = new Color(0.5f, 0.5f, 0, 0.5f);

                if (GUILayout.Button("      Build     ", StyleUtility.PickStyle(GUI.skin.button, 18), GUILayout.Height(40)))
                {
                    AssetBuilder.Build(AssetSettings.Fetch().OutputFolder, this.BuildPlatform, this.BuildOptions);
                    GUIUtility.ExitGUI(); // Fix "EndLayoutGroup: BeginLayoutGroup must be called first" issue
                }

                GUI.backgroundColor = new Color(0, 0.5f, 0, 0.5f);
                if (GUILayout.Button("Build And Copy", StyleUtility.PickStyle(GUI.skin.button, 18), GUILayout.Height(40)))
                {
                    AssetBuilder.BuildAndCopy(AssetSettings.Fetch().OutputFolder, this.BuildPlatform, this.BuildOptions);
                    GUIUtility.ExitGUI(); // Fix "EndLayoutGroup: BeginLayoutGroup must be called first" issue
                }

                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Separator();

                GUI.backgroundColor = backgroundColor;
            }
            EditorGUILayout.EndVertical();
        }

        public static void OpenDirectory(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                Debug.LogError("Can Not Found AssetDepot OutputPath! ");
                return;
            }

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            string fullpath = Path.GetFullPath(path);
            UnityEditor.EditorUtility.RevealInFinder(fullpath);
        }
    }
}
