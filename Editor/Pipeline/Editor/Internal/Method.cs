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
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEditor;

namespace Taichi.Pipeline.Editor.Internal
{
    internal class Method : IMethod
    {
        public const BindingFlags TypeBindingFlags = BindingFlags.Static | BindingFlags.Public | BindingFlags.InvokeMethod | BindingFlags.DeclaredOnly;

        private string typeName = string.Empty;
        private string methodName = string.Empty;

        private object[] argValues = null;

        public string Name => string.IsNullOrEmpty(this.methodName) ? "Select a method" : this.methodName;

        public bool IsValid => !string.IsNullOrEmpty(this.methodName);

        public string[] ArgNames { get; private set; } = new string[0];
        public Type[] ArgTypes { get; private set; } = new Type[0];

        public MethodInfo MethodInfo
        {
            set
            {
                if (value == null)
                {
                    return;
                }

                this.typeName = value.DeclaringType.AssemblyQualifiedName;
                this.methodName = value.Name;

                var ps = value.GetParameters();
                this.ArgNames = ps.Select(p => p.Name).ToArray();
                this.ArgTypes = ps.Select(p => p.ParameterType).ToArray();
                this.argValues = new object[ps.Length];
            }
        }

        public object Clone()
        {
            var clone = new Method();
            clone.typeName = this.typeName;
            clone.methodName = this.methodName;
            clone.ArgNames = this.ArgNames.ToArray();
            clone.ArgTypes = this.ArgTypes.ToArray();
            clone.argValues = this.argValues.ToArray();

            return clone;
        }

        public object GetArg(int index)
        {
            if (index < 0 || index >= this.argValues.Length)
            {
                return null;
            }

            return this.argValues[index];
        }

        public void SetArg(int index, object value)
        {
            if (index >= 0 && index < this.argValues.Length)
            {
                this.argValues[index] = value;
            }
        }

        public void Invoke()
        {
            try
            {
                var t = Type.GetType(this.typeName);
                var methodInfo = t.GetMethod(this.methodName, TypeBindingFlags, Type.DefaultBinder, this.ArgTypes, null);
                methodInfo?.Invoke(null, this.argValues);

            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }

        public MethodData Export()
        {
            using (var stream = new MemoryStream())
            {
                using (var writer = new BinaryWriter(stream))
                {
                    SerializeArgs(writer, this.ArgTypes, this.argValues);
                    return new MethodData
                    {
                        TypeName = this.typeName,
                        MethodName = this.methodName,
                        ArgTypes = this.ArgTypes.Select(t => t.AssemblyQualifiedName).ToArray(),
                        Args = stream.ToArray(),
                    };
                }
            }
        }

        public void Import(MethodData data)
        {
            if (!string.IsNullOrEmpty(data.TypeName) && !string.IsNullOrEmpty(data.MethodName))
            {
                this.typeName = data.TypeName ?? string.Empty;
                this.methodName = data.MethodName ?? string.Empty;
                this.ArgNames = data.ArgNames ?? new string[0];
                this.ArgTypes = data.ArgTypes != null ? data.ArgTypes.Select(t => Type.GetType(t)).ToArray() : new Type[0];

                using (var reader = new BinaryReader(new MemoryStream(data.Args)))
                {
                    this.argValues = DeserializeArgs(reader, this.ArgTypes);
                }
            }
        }

        private static void SerializeArgs(BinaryWriter writer, Type[] argTypes, object[] args)
        {
            for (var i = 0; i < argTypes.Length; ++i)
            {
                SerializeValue(writer, argTypes[i], args[i]);
            }
        }

        private static object[] DeserializeArgs(BinaryReader reader, Type[] argTypes)
        {
            var args = new object[argTypes.Length];
            for (var i = 0; i < argTypes.Length; ++i)
            {
                args[i] = DeserializeValue(reader, argTypes[i]);
            }

            return args;
        }

        private static void SerializeValue(BinaryWriter writer, Type type, object value)
        {
            value = value ?? type.New();
            if (type.IsValueType)
            {
                if (type.IsEnum)
                {
                    type = Enum.GetUnderlyingType(type);
                    value = Convert.ChangeType(value, type);
                    SerializeValue(writer, type, value);
                }
                else if (type == typeof(bool))
                {
                    writer.Write((bool)value);
                }
                else if (type == typeof(short))
                {
                    writer.Write((short)value);
                }
                else if (type == typeof(ushort))
                {
                    writer.Write((ushort)value);
                }
                else if (type == typeof(int))
                {
                    writer.Write((int)value);
                }
                else if (type == typeof(uint))
                {
                    writer.Write((uint)value);
                }
                else if (type == typeof(long))
                {
                    writer.Write((long)value);
                }
                else if (type == typeof(ulong))
                {
                    writer.Write((ulong)value);
                }
                else if (type == typeof(float))
                {
                    writer.Write((float)value);
                }
                else if (type == typeof(double))
                {
                    writer.Write((double)value);
                }
                else
                {
                    throw new NotSupportedException($"NOT support the object type: {type}");
                }
            }
            else if (type == typeof(string))
            {
                writer.Write((string)value);
            }
            else if (type.IsArray)
            {
                var arr = (Array)value;
                writer.Write(arr.Length);
                for (var i = 0; i < arr.Length; ++i)
                {
                    SerializeValue(writer, type.GetElementType(), arr.GetValue(i));
                }
            }
            else if (type == typeof(TextAsset))
            {
                var assetPath = AssetDatabase.GetAssetPath((TextAsset)value);
                writer.Write(assetPath);
            }
            else
            {
                throw new NotSupportedException($"NOT support the object type: {type}");
            }
        }

        private static object DeserializeValue(BinaryReader reader, Type type)
        {
            if (type.IsValueType)
            {
                if (type.IsEnum)
                {
                    return Enum.ToObject(type, DeserializeValue(reader, Enum.GetUnderlyingType(type)));
                }
                else if (type == typeof(bool))
                {
                    return reader.ReadBoolean();
                }
                else if (type == typeof(short))
                {
                    return reader.ReadInt16();
                }
                else if (type == typeof(ushort))
                {
                    return reader.ReadUInt16();
                }
                else if (type == typeof(int))
                {
                    return reader.ReadInt32();
                }
                else if (type == typeof(uint))
                {
                    return reader.ReadUInt32();
                }
                else if (type == typeof(long))
                {
                    return reader.ReadInt64();
                }
                else if (type == typeof(ulong))
                {
                    return reader.ReadUInt64();
                }
                else if (type == typeof(float))
                {
                    return reader.ReadSingle();
                }
                else if (type == typeof(double))
                {
                    return reader.ReadDouble();
                }
            }
            else if (type == typeof(string))
            {
                return reader.ReadString();
            }
            else if (type.IsArray)
            {
                var length = reader.ReadInt32();
                var arr = Array.CreateInstance(type.GetElementType(), length);
                for (var i = 0; i < length; ++i)
                {
                    arr.SetValue(DeserializeValue(reader, type.GetElementType()), i);
                }

                return arr;
            }
            else if (type == typeof(TextAsset))
            {
                var assetPath = reader.ReadString();
                return AssetDatabase.LoadAssetAtPath<TextAsset>(assetPath);
            }

            throw new NotSupportedException($"NOT support the object type: {type}");
        }
    }
}
