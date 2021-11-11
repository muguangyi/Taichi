/*
 * This file is part of Taichi project.
 *
 * (c) MuGuangyi <muguangyi@hotmail.com>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 */

using System;
using System.Reflection;

namespace Taichi.Foundation
{
    public interface ITypeResolver
    {
        object CreateInstance(string module, params object[] payload);
        Delegate CreateDelegate(Type type, object target, MethodInfo method);
    }
}
