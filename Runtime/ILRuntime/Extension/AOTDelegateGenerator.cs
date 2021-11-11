﻿/*
 * This file is part of Taichi project.
 *
 * (c) MuGuangyi <muguangyi@hotmail.com>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 */

using ILRuntime.CLR.Method;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace ILRuntime.Runtime.Enviorment
{
    internal static class AOTDelegateGenerator
    {
        private class DelegateNode
        {
            private bool terminated = false;
            private Dictionary<Type, DelegateNode> types = new Dictionary<Type, DelegateNode>();

            public bool Find(params Type[] types)
            {
                if (types.Length == 0)
                {
                    if (this.terminated)
                    {
                        return true;
                    }

                    this.terminated = true;
                    return false;
                }

                var ts = new Queue<Type>(types);
                var t = ts.Dequeue();
                if (!this.types.TryGetValue(t, out DelegateNode node))
                {
                    this.types.Add(t, node = new DelegateNode());
                }

                return node.Find(ts.ToArray());
            }
        }

        public static void Generate(AppDomain domain, string outputPath)
        {
            if (!Directory.Exists(outputPath))
            {
                Directory.CreateDirectory(outputPath);
            }

            var delegateTypes = CollectDelegates(domain);
            using (var sw = new StreamWriter(outputPath + "/CLRDelegateBindings.cs", false, new UTF8Encoding(false)))
            {
                var sb = new StringBuilder();
                sb.AppendLine(@"// -----------------------------------------------------------------------
// This file is auto generated by Taichi ILRuntime module. DO NOT modify
// it by manual.
// -----------------------------------------------------------------------

namespace Taichi.ILRuntime.Generated
{
    public static class CLRDelegateBindings
    {
        public static readonly System.Type[] AOTDelegateTypes =
        {");

                var actionTypeTree = new DelegateNode();
                var funcTypeTree = new DelegateNode();
                foreach (var t in delegateTypes)
                {
                    var code = GenerateDelegateCode(t.DeclearingType.TypeForCLR, ref actionTypeTree, ref funcTypeTree);
                    if (!string.IsNullOrEmpty(code))
                    {
                        sb.AppendLine(code);
                    }
                }

                sb.AppendLine(@"        };
    }
}");
                sw.Write(Regex.Replace(sb.ToString(), "(?<!\r)\n", "\r\n"));
            }
        }

        private static CLRMethod[] CollectDelegates(AppDomain domain)
        {
            var delegates = new List<CLRMethod>();

            domain.SuppressStaticConstructor = true;
            //Prewarm
            PrewarmDomain(domain);
            //Prewarm twice to ensure GenericMethods are prewarmed properly
            PrewarmDomain(domain);

            var arr = domain.LoadedTypes.Values.ToArray();
            foreach (var type in arr)
            {
                if (type is CLR.TypeSystem.ILType)
                {
                    if (type.TypeForCLR.IsByRef || type.HasGenericParameter)
                    {
                        continue;
                    }

                    var methods = type.GetMethods().ToList();

                    // Append normal constructors.
                    foreach (var i in ((CLR.TypeSystem.ILType)type).GetConstructors())
                    {
                        methods.Add(i);
                    }

                    // Append static constructor.
                    if (((CLR.TypeSystem.ILType)type).GetStaticConstroctor() != null)
                    {
                        methods.Add(((CLR.TypeSystem.ILType)type).GetStaticConstroctor());
                    }

                    foreach (var j in methods)
                    {
                        ILMethod method = j as ILMethod;
                        if (method != null)
                        {
                            if (method.GenericParameterCount > 0 && !method.IsGenericInstance)
                            {
                                continue;
                            }

                            var body = method.Body;
                            foreach (var ins in body)
                            {
                                switch (ins.Code)
                                {
                                    case Intepreter.OpCodes.OpCodeEnum.Newobj:
                                        {
                                            var m = domain.GetMethod(ins.TokenInteger) as CLRMethod;
                                            if (m != null && m.DeclearingType.IsDelegate)
                                            {
                                                delegates.Add(m);
                                            }
                                        }
                                        break;
                                }
                            }
                        }
                    }
                }
            }

            return delegates.ToArray();
        }

        private static void PrewarmDomain(AppDomain domain)
        {
            var arr = domain.LoadedTypes.Values.ToArray();
            //Prewarm
            foreach (var type in arr)
            {
                if (!(type is CLR.TypeSystem.ILType) || type.HasGenericParameter)
                {
                    continue;
                }

                var methods = type.GetMethods().ToList();
                foreach (var i in ((CLR.TypeSystem.ILType)type).GetConstructors())
                {
                    methods.Add(i);
                }

                if (((CLR.TypeSystem.ILType)type).GetStaticConstroctor() != null)
                {
                    methods.Add(((CLR.TypeSystem.ILType)type).GetStaticConstroctor());
                }

                foreach (var j in methods)
                {
                    var method = j as ILMethod;
                    if (method != null)
                    {
                        if (method.GenericParameterCount > 0 && !method.IsGenericInstance)
                        {
                            continue;
                        }

                        var body = method.Body;
                    }
                }
            }
        }

        private static string GenerateDelegateCode(Type type, ref DelegateNode actionTypeTree, ref DelegateNode funcTypeTree)
        {
            var sb = new StringBuilder();

            var method = type.GetMethod("Invoke");
            var parameterTypes = method.GetParameters().Select(p => p.ParameterType).ToArray();
            var returnType = method.ReturnType;
            if (returnType == typeof(void))
            {
                if (parameterTypes.Length > 0 && !actionTypeTree.Find(parameterTypes))
                {
                    sb.Append("            typeof(global::ILRuntime.Runtime.Enviorment.ActionAdaptor<");
                    sb.Append(GenerateGenericTypes(parameterTypes));
                    sb.Append(">),");
                }
            }
            else if (!funcTypeTree.Find(parameterTypes.Concat(new[] { returnType }).ToArray()))
            {
                sb.Append("            typeof(global::ILRuntime.Runtime.Enviorment.FuncAdaptor<");
                sb.Append(GenerateGenericTypes(parameterTypes, returnType));
                sb.Append(">),");
            }

            return sb.ToString();
        }

        private static string GenerateGenericTypes(Type[] parameterTypes, Type returnType = null)
        {
            var p = new List<string>();
            for (var i = 0; i < parameterTypes.Length; ++i)
            {
                p.Add(FormatType(parameterTypes[i]));
            }

            if (returnType != null)
            {
                p.Add(FormatType(returnType));
            }

            return string.Join(", ", p.ToArray());
        }

        private static string FormatType(Type type)
        {
            if (type.IsPrimitive)
            {
                if (type == typeof(bool))
                {
                    return "bool";
                }
                else if (type == typeof(byte))
                {
                    return "byte";
                }
                else if (type == typeof(short))
                {
                    return "short";
                }
                else if (type == typeof(int))
                {
                    return "int";
                }
                else if (type == typeof(long))
                {
                    return "long";
                }
                else if (type == typeof(ushort))
                {
                    return "ushort";
                }
                else if (type == typeof(uint))
                {
                    return "uint";
                }
                else if (type == typeof(ulong))
                {
                    return "ulong";
                }
                else if (type == typeof(float))
                {
                    return "float";
                }
                else if (type == typeof(double))
                {
                    return "double";
                }
            }
            else if (type == typeof(string))
            {
                return "string";
            }
            else if (type == typeof(object))
            {
                return "object";
            }
            else if (type.IsArray)
            {
                return $"{FormatType(type.GetElementType())}[]";
            }

            return ParseTypeName(type);
        }

        private static string ParseTypeName(Type type)
        {
            if (type.IsGenericType)
            {
                var args = type.GetGenericArguments();
                var typeName = type.FullName.Substring(0, type.FullName.IndexOf("`"));
                return $"global::{typeName}<{GenerateGenericTypes(args)}>";
            }

            return $"global::{type}".Replace("+", ".");
        }
    }
}