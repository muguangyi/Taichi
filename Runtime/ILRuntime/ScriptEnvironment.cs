/*
 * This file is part of Taichi project.
 *
 * (c) MuGuangyi <muguangyi@hotmail.com>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 */

using System.Linq;
using ILRuntime.Runtime.Enviorment;

namespace Taichi.ILRuntime
{
    public static class ScriptEnvironment
    {
        public static void Setup(AppDomain domain)
        {
            // Register adapters.
            var assemblies = System.AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                var types = assembly.GetTypes().Where(a => a.GetCustomAttributes(typeof(ScriptAdaptorAttribute), false).Length > 0);
                foreach (var t in types)
                {
                    domain.RegisterCrossBindingAdaptor(new ScriptBindingAdaptor(t));
                }
            }
        }
    }
}
