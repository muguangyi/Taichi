/*
 * This file is part of Taichi project.
 *
 * (c) MuGuangyi <muguangyi@hotmail.com>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 */

using System.IO;
using UnityEngine;

namespace Taichi.Configure
{
    public abstract class Settings : ScriptableObject, ISerializationCallbackReceiver
    {
        public virtual void OnInit()
        { }

        public virtual void OnBeforeSerialize()
        { }

        public virtual void OnAfterDeserialize()
        { }
    }

    public abstract class RuntimeSettings : Settings
    { }

    public abstract class RuntimeSettings<T> : RuntimeSettings where T : RuntimeSettings
    {
        private const string RelativePath = "Taichi.Settings";
        private const string FolderPath = "Assets/Resources/" + RelativePath;

        private static T instance = default;

        public static string Name => typeof(T).Name;

        public static T Fetch()
        {
            Init();

            return instance;
        }

        public static void Flush()
        {
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(Fetch());
            UnityEditor.AssetDatabase.SaveAssets();
#endif
        }

        private static void Init()
        {
            if (instance == null)
            {
                instance = (T)Resources.Load($"{RelativePath}/{Name}");
#if UNITY_EDITOR
                if (instance == null)
                {
                    instance = CreateInstance<T>();
                    instance.OnInit();

                    if (!Directory.Exists(FolderPath))
                    {
                        Directory.CreateDirectory(FolderPath);
                    }

                    var filePath = Path.Combine(FolderPath, $"{Name}.asset");
                    UnityEditor.AssetDatabase.CreateAsset(instance, filePath);
                }
#endif

                if (instance == null)
                {
                    throw new InvalidDataException($"{Name} instance is invalid");
                }
            }
        }
    }

#if UNITY_EDITOR
    public abstract class EditorSettings : Settings
    { }

    public abstract class EditorSettings<T> : EditorSettings where T : EditorSettings
    {
        private const string FolderPath = "Assets/Taichi.Settings/Editor";

        private static T instance = default;

        public static string Name => typeof(T).Name;

        public static T Fetch()
        {
            Init();

            return instance;
        }

        public static void Flush()
        {
            UnityEditor.EditorUtility.SetDirty(Fetch());
            UnityEditor.AssetDatabase.SaveAssets();
        }

        private static void Init()
        {
            if (instance == null)
            {
                var filePath = Path.Combine(FolderPath, $"{Name}.asset");
                instance = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(filePath);
                if (instance == null)
                {
                    instance = CreateInstance<T>();

                    if (!Directory.Exists(FolderPath))
                    {
                        Directory.CreateDirectory(FolderPath);
                    }

                    UnityEditor.AssetDatabase.CreateAsset(instance, filePath);
                }

                instance.OnInit();
            }
        }
    }
#endif
}
