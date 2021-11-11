/*
 * This file is part of Taichi project.
 *
 * (c) MuGuangyi <muguangyi@hotmail.com>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 */

using UnityEngine;

namespace Taichi.UNode.Editor
{
    class ActionEvent
    {
        public enum ActionType
        {
            None,
            SpotBeginConnect,
            SpotEndConnect,
            NodeBeginClick,
            NodeEndClick,
            NodeDrag,
            GraphOpen,
        }

        private static ActionEvent lastEvent = null;
        private object target = null;

        public ActionEvent(ActionType type, object target, Vector2 mousePosition)
        {
            this.Type = type;
            this.target = target;
            this.MousePosition = mousePosition;

            this.Prev = lastEvent;
            lastEvent = this;

            if (null == this.target && null != this.Prev) { this.target = this.Prev.target; }
        }

        public ActionType Type { get; } = ActionType.None;

        public T Target<T>()
        {
            return (null != this.target && this.target.GetType() == typeof(T)) ? (T)this.target : default(T);
        }

        public Vector2 MousePosition { get; } = new Vector2();

        public ActionEvent Prev { get; } = null;

        public static void Clear()
        {
            lastEvent = null;
        }
    }
}
