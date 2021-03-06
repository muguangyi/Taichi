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
    [CustomEditor(typeof(Out))]
    public class OutInspector : NodeInspector
    {
        public override void OnInspectorGUI()
        {
            var op = this.target as Out;
            var spotName = EditorGUILayout.TextField("Spot:", op.SpotName);
            if (spotName != op.SpotName)
            {
                op.OnChange(spotName);
            }
            LangTypeInspector.OnGUI(op.Type);

            base.OnInspectorGUI();
        }
    }
}