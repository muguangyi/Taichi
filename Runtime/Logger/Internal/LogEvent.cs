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
    internal struct LogEvent : ILogEvent
    {
        public LogEvent(object message, LogLevel level, object sender)
        {
            this.Message = message;
            this.Level = level;
            this.Sender = sender;
        }

        public object Message { get; private set; }
        public LogLevel Level { get; private set; }
        public object Sender { get; private set; }
    }
}
