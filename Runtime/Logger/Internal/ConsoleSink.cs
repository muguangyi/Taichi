/*
 * This file is part of Taichi project.
 *
 * (c) MuGuangyi <muguangyi@hotmail.com>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 */

namespace Taichi.Logger.Internal
{
    internal sealed class ConsoleSink : ILogSink
    {
        public ConsoleSink(LogLevel minimunLevel = LogLevel.Fatal)
        {
            this.MinimumLevel = minimunLevel;
        }

        public LogLevel MinimumLevel { get; private set; } = LogLevel.Fatal;

        public void Emit(ILogEvent logEvent)
        {
            System.Console.WriteLine(logEvent.Message);
        }
    }
}
