/*
 * This file is part of Taichi project.
 *
 * (c) MuGuangyi <muguangyi@hotmail.com>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 */

using UnityEditor;
using Taichi.UNode.Editor;

namespace Taichi.UNLang.Editor
{
    [CustomEditor(typeof(SetVar))]
    public class SetVarInspector : NodeInspector
    {
        public override void OnInspectorGUI()
        {
            var var = this.target as SetVar;
            var.Scope = (LangVars.Scope)EditorGUILayout.EnumPopup(var.Scope);
            LangTypeInspector.OnGUI(var.Type);
            var.Variable = EditorGUILayout.TextField("Variable:", var.Variable);

            base.OnInspectorGUI();
        }
    }
}