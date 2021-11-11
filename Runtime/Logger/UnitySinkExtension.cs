/*
 * This file is part of Taichi project.
 *
 * (c) MuGuangyi <muguangyi@hotmail.com>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 */

using Taichi.Logger.Internal;

namespace Taichi.Logger
{
    public static class UnitySinkExtension
    {
        public static ILogConfigure Unity(this ILogConfigure configure, LogLevel minimumLevel = LogLevel.Fatal)
        {
            return configure.Sink(new UnitySink(minimumLevel));
        }
    }
}
