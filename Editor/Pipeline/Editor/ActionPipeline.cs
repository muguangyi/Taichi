/*
 * This file is part of Taichi project.
 *
 * (c) MuGuangyi <muguangyi@hotmail.com>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 */

using Taichi.Pipeline.Editor.Internal;
using System;
using UnityEngine;
using System.Diagnostics;

namespace Taichi.Pipeline.Editor
{
    public static class ActionPipeline
    {
        public static bool IsBuilding { get; private set; } = false;

        public static void Run(string name, bool selectOnly = false)
        {
            var settings = ActionPipelineSettings.Fetch();
            ExecutePipeline(settings.FindPipeline(name), selectOnly);
        }

        public static void RunCommandLine()
        {
            var args = Environment.GetCommandLineArgs();
            var index = Array.IndexOf(args, typeof(ActionPipeline).FullName);
            if (index >= 0 && index + 1 < args.Length)
            {
                Run(args[index + 1]);
            }
        }

        public static void ShellScript(TextAsset asset)
        {
#if UNITY_EDITOR_WIN
            var file = "cmd.exe";
#else
            var file = "/bin/bash";
#endif

            var p = new Process();
            p.StartInfo.FileName = file;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.CreateNoWindow = true;
            p.Start();

            p.StandardInput.WriteLine(asset.text + " &exit");
            p.StandardInput.AutoFlush = true;
            p.WaitForExit();
            p.Close();
        }

        internal static bool ExecuteStep(IStep step)
        {
            try
            {
                IsBuilding = true;
                step.Method?.Invoke();
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                IsBuilding = false;
            }

            return true;
        }

        private static void ExecutePipeline(IPipeline pipeline, bool selectOnly = false)
        {
            if (pipeline == null)
            {
                // TODO: Show error.
                return;
            }

            var settings = ActionPipelineSettings.Fetch();
            settings.ExcutingSteps.Clear();
            foreach (var s in pipeline.Steps)
            {
                settings.ExcutingSteps.Enqueue(s);
            }
            ExecuteContinued();
        }

        private static void ExecuteContinued()
        {
            var settings = ActionPipelineSettings.Fetch();
            while (settings.ExcutingSteps.Count > 0)
            {
                var step = settings.ExcutingSteps.Dequeue();
                if (!ExecuteStep(step))
                {
                    break;
                }
            }
        }
    }
}
