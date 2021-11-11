/*
 * This file is part of Taichi project.
 *
 * (c) MuGuangyi <muguangyi@hotmail.com>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 */

using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Taichi.Editor
{
    public static class StyleUtility
    {
        public static GUIStyle PickStyle(GUIStyle template, int fontSize = -1)
        {
            var style = null != template ? new GUIStyle(template) : new GUIStyle();
            if (-1 != fontSize)
            {
                style.fontSize = fontSize;
            }

            return style;
        }
    }

    public static class FileUtility
    {
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
            EditorUtility.RevealInFinder(fullpath);
        }
    }

    public static class TypeExtension
    {
        public static object New(this Type type)
        {
            if (type.IsArray)
            {
                var et = type.GetElementType();
                return Array.CreateInstance(et, 0);
            }
            else if (type == typeof(string))
            {
                return "";
            }
            else
            {
                return Activator.CreateInstance(type);
            }
        }
    }

    public static class ArrayExtension
    {
        public static Array Append(this Array arr, object value)
        {
            var type = arr.GetType().GetElementType();
            var array = Array.CreateInstance(type, arr.Length + 1);
            Array.Copy(arr, array, arr.Length);
            array.SetValue(value ?? type.New(), arr.Length);

            return array;
        }

        public static Array Remove(this Array arr, int index)
        {
            if (index < 0 || index >= arr.Length)
            {
                return arr;
            }

            var type = arr.GetType().GetElementType();
            var array = Array.CreateInstance(type, arr.Length - 1);
            if (index > 0)
            {
                Array.Copy(arr, array, index);
            }
            if (index < (arr.Length - 1))
            {
                Array.Copy(arr, index + 1, array, index, array.Length - index);
            }

            return array;
        }
    }
}
