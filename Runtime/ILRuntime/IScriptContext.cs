/*
 * This file is part of Taichi project.
 *
 * (c) MuGuangyi <muguangyi@hotmail.com>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 */

using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;

namespace Taichi.ILRuntime
{
    public interface IScriptContext
    {
        AppDomain Domain { get; }
        ILTypeInstance Instance { get; }

        IScriptMethod GetMethod(string name, int paramCount = 0, bool declaredOnly = false);

        string ToString();
    }
}
