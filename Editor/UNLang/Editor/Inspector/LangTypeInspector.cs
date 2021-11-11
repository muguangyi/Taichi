/*
 * This file is part of Taichi project.
 *
 * (c) MuGuangyi <muguangyi@hotmail.com>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 */

using UnityEditor;

namespace Taichi.UNLang.Editor
{
    public static class LangTypeInspector
    {
        public static void OnGUI(LangType type)
        {
            var t = (LangType.Category)EditorGUILayout.EnumPopup(type.Type);
            if (t != type.Type)
            {
                type.OnChange(t);
            }
        }
    }
}