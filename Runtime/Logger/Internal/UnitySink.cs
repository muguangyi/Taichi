/*
 * This file is part of Taichi project.
 *
 * (c) MuGuangyi <muguangyi@hotmail.com>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 */

using UnityEngine;

namespace Taichi.Logger.Internal
{
    internal sealed class UnitySink : ILogSink
    {
        public UnitySink(LogLevel minimumLevel = LogLevel.Fatal)
        {
            this.MinimumLevel = minimumLevel;
        }

        public LogLevel MinimumLevel { get; private set; } = LogLevel.Fatal;

        public void Emit(ILogEvent logEvent)
        {
            var category = Category(logEvent.Sender);
            category = FormatCategory(category);
            switch (logEvent.Level)
            {
            case LogLevel.Verbose:
            case LogLevel.Debug:
            case LogLevel.Info:
#if UNITY_EDITOR
                    Debug.Log($"<color=#fefefe>{category} {logEvent.Message}</color>");
#else
                    Debug.Log($"{category} {logEvent.Message}");
#endif
                    break;
            case LogLevel.Warn:
#if UNITY_EDITOR
                    Debug.LogWarning($"<color=#fc9d00>{category} {logEvent.Message}</color>");
#else
                    Debug.LogWarning($"{category} {logEvent.Message}");
#endif
                    break;
            case LogLevel.Error:
            case LogLevel.Fatal:
#if UNITY_EDITOR
                    Debug.LogError($"<color=#ff2020>{category} {logEvent.Message}</color>");
#else
                    Debug.LogError($"{category} {logEvent.Message}");
#endif
                    break;
            }
        }

        private static string Category(object sender)
        {
            return sender == null ? string.Empty : sender.GetType().Name;
        }

        private static string FormatCategory(string category)
        {
            return string.IsNullOrEmpty(category) ? string.Empty : $"<{category}>";
        }
    }
}
