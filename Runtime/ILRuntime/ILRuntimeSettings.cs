/*
 * This file is part of Taichi project.
 *
 * (c) MuGuangyi <muguangyi@hotmail.com>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 */

#if UNITY_EDITOR
using Taichi.Configure;

namespace Taichi.ILRuntime
{
    public sealed class ILRuntimeSettings : EditorSettings<ILRuntimeSettings>
    {
        public string BindingFolder = "Assets/ILBinding";
        public string OutputFolder = "Assets/Loadable/IL";
        public string[] Assemblies = new string[0];
    }
}
#endif
