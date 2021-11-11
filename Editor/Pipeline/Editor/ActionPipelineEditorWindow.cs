/*
 * This file is part of Taichi project.
 *
 * (c) MuGuangyi <muguangyi@hotmail.com>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 */

using Taichi.Editor;
using Taichi.Pipeline.Editor.Internal;
using System;
using UnityEditor;
using UnityEngine;
using UnityEditorInternal;
using System.Collections;

namespace Taichi.Pipeline.Editor
{
    public sealed class ActionPipelineEditorWindow : EditorWindow
    {
        private const int ButtonWidth = 120;
        private const int ButtonHeight = 40;
        private static readonly GUILayoutOption ButtonWidthOption = GUILayout.Width(ButtonWidth);
        private static readonly GUILayoutOption ButtonHeightOption = GUILayout.Height(ButtonHeight);
        private const int IconSize = 30;
        private static readonly GUILayoutOption IconWidthOption = GUILayout.Width(IconSize);
        private static readonly GUILayoutOption IconHeightOption = GUILayout.Height(IconSize);

        private class Styles
        {
            private static GUIStyle editIconStyle = null;

            public static GUIStyle EditIconStyle
            {
                get
                {
                    if (editIconStyle == null)
                    {
                        editIconStyle = new GUIStyle((GUIStyle)"Command")
                        {
                            fixedWidth = IconSize,
                            fixedHeight = IconSize,
                        };
                    }

                    return editIconStyle;
                }
            }
        }

        private ReorderableList pipelineList = null;
        private ReorderableList stepList = null;
        private IMethod method = null;
        private Vector2 scrollPos = Vector2.zero;

        [MenuItem("[ Taichi ]/Pipeline/Action Pipeline...", priority = MenuPriority.Pipeline)]
        private static void OpenWindow()
        {
            var window = GetWindowWithRect<ActionPipelineEditorWindow>(new Rect(0, 0, 800, 510), true);
            window.titleContent = new GUIContent("Action Pipeline");
        }

        private IPipeline CurrentPipeline
        {
            get
            {
                if (this.pipelineList.index < 0 || this.pipelineList.index >= this.pipelineList.list.Count)
                {
                    return null;
                }

                return (IPipeline)this.pipelineList.list[this.pipelineList.index];
            }
        }

        private void OnEnable()
        {
            Refresh();
        }

        private void OnDisable()
        {
            ActionPipelineSettings.Flush();
        }

        private void OnGUI()
        {
            EditorGUI.BeginDisabledGroup(ActionPipeline.IsBuilding);
            {
                EditorGUILayout.BeginHorizontal();
                {
                    OnListViewGUI();

                    var pipeline = this.CurrentPipeline;
                    if (pipeline != null)
                    {
                        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                        {
                            pipeline.Name = EditorGUILayout.TextField("Pipeline: ", pipeline.Name);

                            this.scrollPos = EditorGUILayout.BeginScrollView(this.scrollPos, GUI.skin.box, GUILayout.Width(530), GUILayout.Height(400));
                            {
                                var steps = pipeline.Steps;
                                for (var i = 0; i < steps.Length; ++i)
                                {
                                    var step = steps[i];
                                    bool editing = false;
                                    EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
                                    {
                                        if (step.Method == this.method)
                                        {
                                            step.Name = EditorGUILayout.TextField("Step: ", step.Name);
                                        }
                                        else
                                        {
                                            EditorGUILayout.BeginVertical();
                                            {
                                                if (string.IsNullOrEmpty(step.Name))
                                                {
                                                    step.Selected = EditorGUILayout.ToggleLeft($"Step {i}", step.Selected);
                                                }
                                                else
                                                {
                                                    step.Selected = EditorGUILayout.ToggleLeft(step.Name, step.Selected);
                                                }

                                                if (step.Method.IsValid)
                                                {
                                                    EditorGUILayout.LabelField(step.Method.Name);
                                                }
                                                else
                                                {
                                                    EditorGUILayout.LabelField(step.Method.Name, new GUIStyle(EditorStyles.label) { normal = new GUIStyleState { textColor = Color.red } });
                                                }
                                            }
                                            EditorGUILayout.EndVertical();
                                        }
                                        GUILayout.Space(20);

                                        if (GUILayout.Button(EditorGUIUtility.IconContent("preAudioPlayOn"), IconWidthOption, IconHeightOption))
                                        {
                                            ActionPipeline.ExecuteStep(step);
                                            GUIUtility.ExitGUI();
                                        }

                                        if (GUILayout.Button("↑", IconWidthOption, IconHeightOption))
                                        {
                                            pipeline.SwapSteps(i, i - 1);
                                        }

                                        if (GUILayout.Button("↓", IconWidthOption, IconHeightOption))
                                        {
                                            pipeline.SwapSteps(i, i + 1);
                                        }

                                        if (GUILayout.Button(EditorGUIUtility.IconContent("TreeEditor.Trash"), IconWidthOption, IconHeightOption))
                                        {
                                            if (EditorUtility.DisplayDialog("BuildPipeline", "Are you sure to delete the step?", "YES", "NO"))
                                            {
                                                pipeline.RemoveStep(step);
                                            }
                                        }

                                        var state = step.Method == this.method;
                                        editing = GUILayout.Toggle(state, EditorGUIUtility.IconContent("editicon.sml"), Styles.EditIconStyle);
                                        if (editing != state)
                                        {
                                            this.method = editing ? step.Method : null;
                                        }
                                    }
                                    EditorGUILayout.EndHorizontal();

                                    if (editing)
                                    {
                                        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                                        {
                                            EditorGUILayout.BeginHorizontal();
                                            {
                                                EditorGUILayout.LabelField("Method: ", GUILayout.Width(50));
                                                if (GUILayout.Button(step.Method.Name, EditorStyles.textField))
                                                {
                                                    MethodEditorWindow.OpenWindowFor(step);
                                                }
                                            }
                                            EditorGUILayout.EndHorizontal();

                                            OnMethodGUI(step.Method);
                                        }
                                        EditorGUILayout.EndVertical();
                                    }
                                }

                                if (GUILayout.Button("+"))
                                {
                                    pipeline.AddStep(new Step("New Step"));
                                }
                            }
                            EditorGUILayout.EndScrollView();

                            EditorGUILayout.Separator();

                            EditorGUILayout.BeginHorizontal();
                            {
                                if (GUILayout.Button("Execute All", ButtonWidthOption, ButtonHeightOption))
                                {
                                    if (EditorUtility.DisplayDialog("BuildPipeline", "Are you sure to execute the pipeline?", "YES", "NO"))
                                    {
                                        ActionPipeline.Run(pipeline.Name);
                                        GUIUtility.ExitGUI();
                                    }
                                }
                                EditorGUILayout.Space();
                                if (GUILayout.Button("Execute Selection", ButtonWidthOption, ButtonHeightOption))
                                {
                                    if (EditorUtility.DisplayDialog("BuildPipeline", "Are you sure to execute the pipeline?", "YES", "NO"))
                                    {
                                        ActionPipeline.Run(pipeline.Name, true);
                                        GUIUtility.ExitGUI();
                                    }
                                }
                                EditorGUILayout.Space();
                                if (GUILayout.Button("Clone Pipeline", ButtonWidthOption, ButtonHeightOption))
                                {
                                    if (EditorUtility.DisplayDialog("BuildPipeline", "Are you sure to clone the pipeline?", "YES", "NO"))
                                    {
                                        var settings = ActionPipelineSettings.Fetch();
                                        settings.AddPipeline((IPipeline)pipeline.Clone());
                                        Refresh();
                                    }
                                }
                            }
                            EditorGUILayout.EndHorizontal();

                            EditorGUILayout.Separator();
                        }
                        EditorGUILayout.EndVertical();
                    }
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUI.EndDisabledGroup();
        }

        private void OnListViewGUI()
        {
            EditorGUILayout.BeginVertical(GUILayout.Width(250));
            {
                this.pipelineList?.DoLayoutList();
            }
            EditorGUILayout.EndVertical();
        }

        private void Refresh()
        {
            this.pipelineList = new ReorderableList(ActionPipelineSettings.Fetch().Pipelines, typeof(IPipeline), true, false, true, true);
            this.pipelineList.drawElementCallback = OnDrawPipelineCallback;
            this.pipelineList.onAddCallback = OnAddPipelineCallback;
            this.pipelineList.onRemoveCallback = OnRemovePipelineCallback;
            this.pipelineList.index = 0;
        }

        private void OnDrawPipelineCallback(Rect rect, int index, bool isActive, bool isFocused)
        {
            var pipeline = (IPipeline)this.pipelineList.list[index];
            EditorGUI.LabelField(rect, pipeline.Name);
        }

        private void OnAddPipelineCallback(ReorderableList list)
        {
            var settings = ActionPipelineSettings.Fetch();
            settings.AddPipeline(new Internal.Pipeline("Untitled"));
            Refresh();
        }

        private void OnRemovePipelineCallback(ReorderableList list)
        {
            var settings = ActionPipelineSettings.Fetch();
            settings.RemovePipeline(this.CurrentPipeline);
            Refresh();
        }

        private void OnDrawStepCallback(Rect rect, int index, bool isActive, bool isFocused)
        {
            if (this.CurrentPipeline == null)
            {
                return;
            }

            var pipeline = this.CurrentPipeline;
            var step = pipeline.Steps[index];

            bool editing = false;
            EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
            {
                if (step.Method == this.method)
                {
                    step.Name = EditorGUILayout.TextField("Step: ", step.Name);
                }
                else
                {
                    EditorGUILayout.BeginVertical();
                    {
                        if (string.IsNullOrEmpty(step.Name))
                        {
                            step.Selected = EditorGUILayout.ToggleLeft($"Step {index}", step.Selected);
                        }
                        else
                        {
                            step.Selected = EditorGUILayout.ToggleLeft(step.Name, step.Selected);
                        }

                        if (step.Method.IsValid)
                        {
                            EditorGUILayout.LabelField(step.Method.Name);
                        }
                        else
                        {
                            EditorGUILayout.LabelField(step.Method.Name, new GUIStyle(EditorStyles.label) { normal = new GUIStyleState { textColor = Color.red } });
                        }
                    }
                    EditorGUILayout.EndVertical();
                }
                GUILayout.Space(20);

                if (GUILayout.Button(EditorGUIUtility.IconContent("preAudioPlayOn"), IconWidthOption, IconHeightOption))
                {
                    ActionPipeline.ExecuteStep(step);
                    GUIUtility.ExitGUI();
                }

                if (GUILayout.Button(EditorGUIUtility.IconContent("TreeEditor.Trash"), IconWidthOption, IconHeightOption))
                {
                    if (EditorUtility.DisplayDialog("BuildPipeline", "Are you sure to delete the step?", "YES", "NO"))
                    {
                        pipeline.RemoveStep(step);
                    }
                }

                var state = step.Method == this.method;
                editing = GUILayout.Toggle(state, EditorGUIUtility.IconContent("editicon.sml"), Styles.EditIconStyle);
                if (editing != state)
                {
                    this.method = editing ? step.Method : null;
                }
            }
            EditorGUILayout.EndHorizontal();

            if (editing)
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                {
                    EditorGUILayout.BeginHorizontal();
                    {
                        EditorGUILayout.LabelField("Method: ", GUILayout.Width(50));
                        if (GUILayout.Button(step.Method.Name, EditorStyles.textField))
                        {
                            MethodEditorWindow.OpenWindowFor(step);
                        }
                    }
                    EditorGUILayout.EndHorizontal();

                    OnMethodGUI(step.Method);
                }
                EditorGUILayout.EndVertical();
            }
        }

        private void OnMethodGUI(IMethod method)
        {
            if (!method.IsValid)
            {
                return;
            }

            var argNames = method.ArgNames;
            var argTypes = method.ArgTypes;
            EditorGUILayout.BeginVertical();
            {
                for (var i = 0; i < argNames.Length; ++i)
                {
                    var argName = argNames[i];
                    var argType = argTypes[i];
                    EditorGUILayout.BeginHorizontal();
                    {
                        EditorGUILayout.LabelField($"{argName}", GUILayout.Width(50));
                        method.SetArg(i, OnMethodParameterGUI(argType, method.GetArg(i)));
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }
            EditorGUILayout.EndVertical();
        }

        private object OnMethodParameterGUI(Type type, object value)
        {
            if (type.IsEnum)
            {
                if (IsEnumMask(type))
                {
                    value = EditorGUILayout.EnumFlagsField("", (Enum)(value ?? Activator.CreateInstance(type)));
                }
                else
                {
                    value = EditorGUILayout.EnumPopup("", (Enum)(value ?? Activator.CreateInstance(type)));
                }
            }
            else if (type.IsArray)
            {
                var arr = value != null ? (Array)value : Array.CreateInstance(type.GetElementType(), 0);
                EditorGUILayout.BeginVertical();
                {
                    for (int i = 0; i < arr.Length;)
                    {
                        EditorGUILayout.BeginHorizontal();
                        {
                            arr.SetValue(OnMethodParameterGUI(type.GetElementType(), arr.GetValue(i)), i);
                            if (GUILayout.Button("-", GUILayout.Width(30)))
                            {
                                arr = arr.Remove(i);
                            }
                            else
                            {
                                ++i;
                            }
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                    if (GUILayout.Button("+"))
                    {
                        arr = arr.Append(null);
                    }
                }
                EditorGUILayout.EndVertical();
                value = arr;
            }
            else if (type == typeof(float) || type == typeof(double))
            {
                value = EditorGUILayout.FloatField((float)(value ?? 0));
            }
            else if (type == typeof(int) || type == typeof(uint) || type == typeof(short) || type == typeof(ushort) ||type == typeof(long) || type == typeof(ulong))
            {
                value = EditorGUILayout.IntField((int)(value ?? 0));
            }
            else if (type == typeof(string))
            {
                value = EditorGUILayout.TextField((string)(value ?? string.Empty));
            }
            else if (type == typeof(bool))
            {
                value = EditorGUILayout.Toggle((bool)(value ?? false));
            }
            else if (type == typeof(TextAsset))
            {
                value = EditorGUILayout.ObjectField("Text Asset", (TextAsset)value, type, false);
            }

            return value;
        }

        private static bool IsEnumMask(Type type)
        {
            var values = Enum.GetValues(type);
            bool isMask = true;
            for (int i = 0; i < values.Length; ++i)
            {
                var v = (uint)Convert.ChangeType(values.GetValue(i), typeof(uint));
                if ((v & (v - 1)) != 0)
                {
                    isMask = false;
                    break;
                }
            }

            return isMask;
        }
    }
}
