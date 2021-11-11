/*
 * This file is part of Taichi project.
 *
 * (c) MuGuangyi <muguangyi@hotmail.com>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 */

namespace Taichi.ILRuntime
{
    public interface IScriptMethod
    {
        object Invoke(params object[] args);
    }
}
