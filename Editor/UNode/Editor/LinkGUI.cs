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
    public sealed class LinkGUI : ElementGUI
    {
        private static readonly Color NORMAL_COLOR = new Color(1f, 0.79f, 0.05f);
        private static readonly Color ACTIVE_COLOR = new Color(0.5f, 1f, 0f);

        private readonly NodeLayout.Layout from = null;
        private readonly NodeLayout.Layout to = null;

        public LinkGUI(Link link, NodeLayout manager)
            : base(manager, new Color(1f, 0.4f, 0.4f))
        {
            this.Link = link;
            this.from = manager.Find(SpotType.Out == link.S0.Type ? link.S0.ID : link.S1.ID);
            this.to = manager.Find(SpotType.In == link.S0.Type ? link.S0.ID : link.S1.ID);

            this.Link.Dispatcher.AddListener(Message.DESTROY, NotifyHandler);
            this.Link.GetOutSpot().Owner.Dispatcher.AddListener(Node.NodeMessage.SIGNAL, NotifyHandler);
            this.Dispatcher = new Dispatcher(this);
        }

        public override void Dispose()
        {
            this.Dispatcher.Notify(Message.Destroy);
            this.Dispatcher.Dispose();
            this.Link.Dispatcher.RemoveListener(Message.DESTROY, NotifyHandler);
            this.Link.GetOutSpot().Owner.Dispatcher.RemoveListener(Node.NodeMessage.SIGNAL, NotifyHandler);

            this.Link = null;
            this.Dispatcher = null;

            base.Dispose();
        }

        public override void OnGUI()
        {
            base.OnGUI();

            var startPoint = GetSpotPosition(this.from);
            var endPoint = GetSpotPosition(this.to);
            DrawLinkGUI(startPoint, endPoint);

            var evt = Event.current;
            if (!Application.isPlaying && evt.type == EventType.ContextClick)
            {
                var centerPoint = startPoint + (endPoint - startPoint) * 0.5f;
                var mousePos = evt.mousePosition;
                if (new Rect(centerPoint.x - 50f, centerPoint.y - 30f, 100f, 60f).Contains(mousePos))
                {
                    var menu = new GenericMenu();
                    menu.AddItem(
                        new GUIContent("Delete"),
                        false,
                        () =>
                        {
                            this.Link.Dispose();
                        }
                    );
                    menu.ShowAsContext();
                    evt.Use();
                }
            }
        }

        public Link Link { get; private set; } = null;

        public Dispatcher Dispatcher { get; private set; } = null;

        private void DrawLinkGUI(Vector3 startPoint, Vector3 endPoint)
        {
            var offset = (Mathf.Abs(startPoint.x - endPoint.x) + Mathf.Abs(startPoint.y - endPoint.y)) * 0.25f;
            var originColor = Handles.color;
            {
                var drawColor = this.IsHighlighted ? this.HighlightColor : (IsLinkActived() ? ACTIVE_COLOR : NORMAL_COLOR);
                Handles.color = drawColor;
                Handles.DrawBezier(
                    startPoint,
                    endPoint,
                    new Vector3(startPoint.x + offset, startPoint.y),
                    new Vector3(endPoint.x - offset, endPoint.y),
                    drawColor,
                    null,
                    2f
                );
                Handles.DrawSolidDisc(startPoint, Vector3.back, 8f);
                Handles.DrawSolidDisc(endPoint, Vector3.back, 8f);
            }
            Handles.color = originColor;
        }

        private Vector3 GetSpotPosition(NodeLayout.Layout layout)
        {
            return new Vector3(layout.GlobalX + layout.Width * 0.5f, layout.GlobalY + layout.Height * 0.5f, 0f);
        }

        private void NotifyHandler(object target, Message message)
        {
            if (Message.DESTROY == message.Type)
            {
                Dispose();
            }
            else if (Node.NodeMessage.SIGNAL == message.Type)
            {
                var spot = (message as Node.NodeMessage).Spot;
                if (spot == this.Link.S0 || spot == this.Link.S1)
                {
                    EnableHighlight();
                }
            }
        }

        private bool IsLinkActived()
        {
            var obj = ActionHandler.ActiveObject;
            if (null != obj)
            {
                if (obj is Node)
                {
                    var node = obj as Node;
                    return (this.Link.S0.Owner == node || this.Link.S1.Owner == node);
                }
                else if (obj is Link)
                {
                    return obj == this.Link;
                }
            }

            return false;
        }
    }
}