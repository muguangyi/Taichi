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
    public static class Log
    {
        private static readonly LogConfigure configure = new LogConfigure();

        public static bool Enabled { get; set; } = true;
        public static ILogConfigure Configure => configure;

        public static void Verbose(object verbose, object sender = null)
        {
            if (Enabled)
            {
                Output(verbose, LogLevel.Verbose, sender);
            }
        }

        public static void Debug(object debug, object sender = null)
        {
            if (Enabled)
            {
                Output(debug, LogLevel.Debug, sender);
            }
        }

        public static void Info(object info, object sender = null)
        {
            if (Enabled)
            {
                Output(info, LogLevel.Info, sender);
            }
        }

        public static void Warn(object warn, object sender = null)
        {
            if (Enabled)
            {
                Output(warn, LogLevel.Warn, sender);
            }
        }

        public static void Error(object error, object sender = null)
        {
            Output(error, LogLevel.Error, sender);
        }

        public static void Fatal(object fatal, object sender = null)
        {
            Output(fatal, LogLevel.Fatal, sender);
        }

        private static void Output(object message, LogLevel level, object sender)
        {
            var logEvent = new LogEvent(message, level, sender);

            for (var i = 0; i < configure.Sinks.Count; ++i)
            {
                var s = configure.Sinks[i];
                if (s.MinimumLevel <= level)
                {
                    s.Emit(logEvent);
                }
            }
        }
    }
}
