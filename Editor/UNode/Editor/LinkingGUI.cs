/*
 * This file is part of Taichi project.
 *
 * (c) MuGuangyi <muguangyi@hotmail.com>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 */

using UnityEditor;
using UnityEngine;

namespace Taichi.UNode.Editor
{
    public sealed class LinkingGUI : ElementGUI
    {
        private Spot spot = null;

        public LinkingGUI(Spot spot, NodeLayout manager)
            : base(manager)
        {
            this.spot = spot;
            this.layout = manager.Find(this.spot.ID);
            this.Dispatcher = new Dispatcher(this);
        }

        public override void Dispose()
        {
            this.Dispatcher.Notify(Message.Destroy);
            this.Dispatcher.Dispose();

            this.spot = null;
            this.Dispatcher = null;

            base.Dispose();
        }

        public override void OnGUI()
        {
            base.OnGUI();

            var mousePos = Event.current.mousePosition;
            var p0 = GetSpotPosition();
            var p1 = new Vector3(mousePos.x, mousePos.y, 0);
            DrawLinkingGUI(SpotType.Out == this.spot.Type ? p0 : p1, SpotType.In == this.spot.Type ? p0 : p1);
        }

        public Dispatcher Dispatcher { get; private set; } = null;

        private void DrawLinkingGUI(Vector3 startPoint, Vector3 endPoint)
        {
            var offset = (Mathf.Abs(startPoint.x - endPoint.x) + Mathf.Abs(startPoint.y - endPoint.y)) * 0.25f;
            Handles.DrawBezier(
                startPoint,
                endPoint,
                new Vector3(startPoint.x + offset, startPoint.y),
                new Vector3(endPoint.x - offset, endPoint.y),
                Color.yellow,
                null,
                2f
            );
        }

        private Vector3 GetSpotPosition()
        {
            return new Vector3(this.layout.GlobalX + this.layout.Width * 0.5f, this.layout.GlobalY + this.layout.Height * 0.5f, 0f);
        }
    }
}