/*
 * This file is part of Taichi project.
 *
 * (c) MuGuangyi <muguangyi@hotmail.com>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 */

using System.Collections.Generic;

namespace Taichi.Logger.Internal
{
    internal sealed class LogConfigure : ILogConfigure
    {
        public List<ILogSink> Sinks { get; } = new List<ILogSink>();

        public ILogConfigure Sink(ILogSink sink)
        {
            this.Sinks.Add(sink);
            return this;
        }
    }
}
