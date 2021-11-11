/*
 * This file is part of Taichi project.
 *
 * (c) MuGuangyi <muguangyi@hotmail.com>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 */

using ILRuntime.Runtime.CLRBinding;
using ILRuntime.Runtime.Enviorment;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;

namespace Taichi.ILRuntime.Editor
{
    [InitializeOnLoad]
    public static class ScriptEditor
    {
        private static readonly string ProjectPath = Path.GetFullPath(Path.Combine(Application.dataPath, "../")).Replace("\\", "/");
        private static readonly string ProjectAssemblyPath = Path.Combine(ProjectPath, "Library/ScriptAssemblies");
        private static readonly string PackageAssemblyPath = Path.Combine(ProjectPath, "Temp");

        private static bool _dirty = false;

        static ScriptEditor()
        {
            CompilationPipeline.assemblyCompilationFinished -= OnAssemblyCompilationFinished;
            CompilationPipeline.compilationFinished -= OnCompilationFinished;

            CompilationPipeline.assemblyCompilationFinished += OnAssemblyCompilationFinished;
            CompilationPipeline.compilationFinished += OnCompilationFinished;
        }

        public static void Generate()
        {
            try
            {
                ForceClean();
                GenerateObjectBindingCode();
                BuildScriptAssemblies();
                GenerateScriptAssets(PackageAssemblyPath);

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }

        public static void ForceClean()
        {
            var settings = ILRuntimeSettings.Fetch();
            ResetFolder(settings.BindingFolder);
        }

        private static void GenerateObjectBindingCode()
        {
            var settings = ILRuntimeSettings.Fetch();
            var files = settings.Assemblies.Select(file =>
            {
                var filePath = Path.Combine(ProjectAssemblyPath, $"{file}.dll");
                if (!File.Exists(filePath))
                {
                    throw new FileNotFoundException(filePath);
                }

                return new FileStream(filePath, FileMode.Open, FileAccess.Read);
            });

            try
            {
                var domain = new global::ILRuntime.Runtime.Enviorment.AppDomain();
                foreach (var f in files)
                {
                    domain.LoadAssembly(f);
                }

                ScriptEnvironment.Setup(domain);

                var targetPath = Path.Combine(ProjectPath, settings.BindingFolder);
                BindingCodeGenerator.GenerateBindingCode(domain, targetPath);
                AOTDelegateGenerator.Generate(domain, targetPath);
            }
            finally
            {
                foreach (var f in files)
                {
                    f.Dispose();
                }
            }
        }

        private static void GenerateScriptAssets(string assemblyPath)
        {
            var settings = ILRuntimeSettings.Fetch();
            if (!Directory.Exists(settings.OutputFolder))
            {
                Directory.CreateDirectory(settings.OutputFolder);
            }

            foreach (var asm in settings.Assemblies)
            {
                File.Copy(Path.Combine(assemblyPath, $"{asm}.dll"), Path.Combine(settings.OutputFolder, $"{asm}.il.bytes"), true);
                File.Copy(Path.Combine(assemblyPath, $"{asm}.pdb"), Path.Combine(settings.OutputFolder, $"{asm}.il.pdb.bytes"), true);
            }
        }

        private static void ResetFolder(string folder)
        {
            if (Directory.Exists(folder))
            {
                var files = Directory.GetFiles(folder, "*.cs", SearchOption.AllDirectories);
                foreach (var f in files)
                {
                    try
                    {
                        File.Delete(f);
                    }
                    catch (Exception ex)
                    {
                        Debug.LogException(ex);
                    }
                }
            }
            else
            {
                Directory.CreateDirectory(folder);
            }
        }

        private static void OnAssemblyCompilationFinished(string asm, CompilerMessage[] msgs)
        {
            // Ignore the assemblies that are not defined in the project.
            var path = CompilationPipeline.GetAssemblyDefinitionFilePathFromAssemblyReference(asm);
            if (string.IsNullOrEmpty(path) || !path.StartsWith("Assets"))
            {
                return;
            }

            var asmName = Path.GetFileNameWithoutExtension(asm);

            var settings = ILRuntimeSettings.Fetch();
            if (settings.Assemblies.Any(a => a == asmName))
            {
                _dirty = true;
            }
        }

        private static void OnCompilationFinished(object obj)
        {
            var settings = ILRuntimeSettings.Fetch();
            if (Directory.Exists(Path.Combine(ProjectPath, settings.BindingFolder)) && !_dirty)
            {
                return;
            }

            Debug.Log("Start generating IL related files...");
            _dirty = false;
            try
            {
                // NOTE: DO NOT generate object binding code.
                GenerateScriptAssets(ProjectAssemblyPath);

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
            finally
            {
                Debug.Log("Finish generating IL related files.");
            }
        }

        public static void BuildScriptAssemblies()
        {
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);

            // Collect all assemblies which needs to be compiled.
            var assemblies = CompilationPipeline.GetAssemblies().Where(a => IsScriptAssembly(a)).ToArray();

            // Sort assemblies by dependency.
            Array.Sort(assemblies, (a, b) =>
            {
                return a.allReferences.Contains(b.outputPath) ? 1 : -1;
            });

            // Start to compile.
            foreach (var a in assemblies)
            {
                BuildReleaseAssembly(a);
            }
        }

        private static bool IsScriptAssembly(Assembly assembly)
        {
            var settings = ILRuntimeSettings.Fetch();
            return settings.Assemblies.Contains(assembly.name);
        }

        private static void BuildReleaseAssembly(Assembly assembly)
        {
            var outputPath = Path.Combine(PackageAssemblyPath, $"{assembly.name}.dll");
            var builder = new AssemblyBuilder(outputPath, assembly.sourceFiles);
            builder.compilerOptions = assembly.compilerOptions;
            builder.compilerOptions.CodeOptimization = CodeOptimization.Release;
            builder.referencesOptions = ReferencesOptions.UseEngineModules;
            builder.excludeReferences = new[] { outputPath };

            // Called on main thread
            builder.buildStarted += assemblyPath =>
            {
                Debug.Log($"Start build assembly: {assemblyPath}");
            };

            // Called on main thread
            builder.buildFinished += (assemblyPath, compilerMessages) =>
            {
                Debug.Log($"Finish build assembly: {assemblyPath}");

                var errors = compilerMessages.Where(m => m.type == CompilerMessageType.Error).ToArray();
                if (errors.Length == 0)
                {
                    AssetDatabase.ImportAsset(assemblyPath);
                }
                else
                {
                    foreach (var e in errors)
                    {
                        Debug.Log($"Error message: {e.message}");
                    }
                }
            };

            builder.Build();

            while (builder.status != AssemblyBuilderStatus.Finished)
            {
                Thread.Sleep(10);
            }
        }
    }
}
