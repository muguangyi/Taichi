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
    [CustomEditor(typeof(Compare))]
    public class CompareInspector : NodeInspector
    {
        public override void OnInspectorGUI()
        {
            var compare = this.target as Compare;
            LangTypeInspector.OnGUI(compare.Type);

            base.OnInspectorGUI();
        }
    }
}