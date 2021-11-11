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
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Taichi.Pipeline.Editor.Internal
{
    internal sealed class MethodEditorWindow : EditorWindow
    {
        [InitializeOnLoad]
        private static class TypeLib
        {
            private static readonly List<Type> types = new List<Type>();

            static TypeLib()
            {
                types.Clear();

                var asms = AppDomain.CurrentDomain.GetAssemblies();
                foreach (var a in asms)
                {
                    types.AddRange(a.GetTypes());
                }
            }

            public static Type[] Search(string typeName)
            {
                return types.Where(t => t.Name.ToLower().Contains(typeName.ToLower()) && t.GetMethods(Method.TypeBindingFlags).Length > 0).ToArray();
            }

            public static MethodInfo[] ListMethods(Type type)
            {
                return type.GetMethods(Method.TypeBindingFlags);
            }
        }

        private IStep step = null;
        private string searchType = string.Empty;
        private Type[] searchResult = null;
        private Type viewType = null;
        private Vector2 scrollPos = Vector2.zero;

        public static void OpenWindowFor(IStep step)
        {
            var window = GetWindowWithRect<MethodEditorWindow>(new Rect(0, 0, 700, 500), true);
            window.titleContent = new GUIContent("Project Method Searcher");
            window.step = step;
        }

        private void OnGUI()
        {
            var result = EditorGUILayout.TextField("Search: ", this.searchType);
            if (result != this.searchType)
            {
                this.searchType = result;
                this.searchResult = TypeLib.Search(this.searchType);
            }

            this.scrollPos = EditorGUILayout.BeginScrollView(this.scrollPos, GUI.skin.box, GUILayout.Width(680), GUILayout.Height(400));
            {
                if (this.searchResult != null)
                {
                    for (var i = 0; i < this.searchResult.Length; ++i)
                    {
                        var t = this.searchResult[i];
                        var foldout = this.viewType == t;
                        var show = EditorGUILayout.Foldout(foldout, t.Name);
                        if (show != foldout)
                        {
                            this.viewType = show ? t : null;
                        }

                        if (show)
                        {
                            var methods = TypeLib.ListMethods(t);
                            for (var j = 0; j < methods.Length; ++j)
                            {
                                var m = methods[j];
                                if (GUILayout.Button(FormatMethod(m), EditorStyles.textField))
                                {
                                    this.step.Method.MethodInfo = m;
                                    Close();
                                }
                            }
                        }
                    }
                }
            }
            EditorGUILayout.EndScrollView();
        }

        private string FormatMethod(MethodInfo m)
        {
            var format = new StringBuilder();
            format.Append(m.ReturnParameter.ParameterType.Name);
            format.Append(" ");
            format.Append(m.Name);
            format.Append("(");
            var ps = m.GetParameters();
            format.Append(string.Join(", ", ps.Select(p => $"{p.ParameterType.Name} {p.Name}")));
            format.Append(")");

            return format.ToString();
        }
    }
}
