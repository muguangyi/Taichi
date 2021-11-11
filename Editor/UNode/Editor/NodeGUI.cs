/*
 * This file is part of Taichi project.
 *
 * (c) MuGuangyi <muguangyi@hotmail.com>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Taichi.UNode.Editor
{
    public sealed class NodeGUI : SquareGUI
    {
        private const float NODEWIDTH = 80f;
        private const float TITLEHEIGHT = 15f;
        private const float SPOTSIZE = 12f;
        private const float SPOTMARGIN = 10f;
        private const float FONTSIZE = 8f;

        private readonly List<SpotGUI> spots = new List<SpotGUI>();
        private Texture2D texture = null;
        private readonly GUIStyle titleStyle = null;

        public NodeGUI(Node node, NodeLayout manager)
            : base(manager, new Color(1f, 0.4f, 0.4f))
        {
            this.Node = node;
            this.layout = manager.Find(node.ID);
            this.Node.Dispatcher.AddListener(Message.DESTROY, NotifyHandler);
            this.Node.Dispatcher.AddListener(Node.NodeMessage.ADD, NotifyHandler);
            this.Node.Dispatcher.AddListener(Node.NodeMessage.SIGNAL, NotifyHandler);

            this.Dispatcher = new Dispatcher(this);

            this.texture = new Texture2D(1, 1);
            this.texture.SetPixel(0, 0, (node is Graph ? new Color(128f / 255f, 0f, 0f, 0.8f) : new Color(12f / 255f, 46f / 255f, 190f / 255f, 0.8f)));
            this.texture.Apply();

            this.titleStyle = new GUIStyle();
            this.titleStyle.fontSize = 11;
            this.titleStyle.normal.textColor = Color.white;

            for (var i = 0; i < this.Node.SpotCount; ++i)
            {
                var spot = this.Node.GetAt(i);
                if (!spot.IsHidden)
                {
                    AddSpot(spot);
                }
            }
            RefreshGUI();
        }

        public override void Dispose()
        {
            this.Dispatcher.Notify(Message.Destroy);
            this.Dispatcher.Dispose();
            this.Node.Dispatcher.RemoveListener(Message.DESTROY, NotifyHandler);
            this.Node.Dispatcher.RemoveListener(Node.NodeMessage.ADD, NotifyHandler);
            this.Node.Dispatcher.RemoveListener(Node.NodeMessage.SIGNAL, NotifyHandler);

            if (null != this.texture)
            {
                UnityEngine.Object.DestroyImmediate(this.texture);
                this.texture = null;
            }

            this.Node = null;
            this.Dispatcher = null;

            base.Dispose();
        }

        public SpotGUI HittestSpot(Vector2 globalPos)
        {
            var localPos = globalPos - this.layout.Position;
            return HittestSpotInternal(localPos);
        }

        public override void OnGUI()
        {
            base.OnGUI();

            if (this.IsHighlighted)
            {
                DrawHighlight();
            }

            var originColor = GUI.backgroundColor;
            {
                GUI.backgroundColor = (Node is InvalidNode ? new Color(1f, 0, 0, 0.5f) : new Color(1f, 1f, 1f, 0.7f));
                GUI.Window(this.Node.ID.GetHashCode(), this.layout.Region, DrawThisNode, string.Empty, new GUIStyle(GUI.skin.window));
            }
            GUI.backgroundColor = originColor;
        }

        public Node Node { get; private set; } = null;

        public NodeLayout.Layout Layout
        {
            get
            {
                return this.layout;
            }
        }

        public Dispatcher Dispatcher { get; private set; } = null;

        private void DrawThisNode(int id)
        {
            HandleNodeMouseEvent();

            GUI.DrawTexture(new Rect(2f, 2f, this.layout.Width - 3f, 12f), this.texture, ScaleMode.StretchToFill);
            GUI.Label(new Rect(2f, 1f, this.layout.Width - 2f, 28f), this.Node.Tag, this.titleStyle);
            for (var i = 0; i < this.spots.Count; ++i)
            {
                this.spots[i].OnGUI();
            }
        }

        private void HandleNodeMouseEvent()
        {
            var evt = Event.current;
            switch (evt.type)
            {
            case EventType.MouseDown:
                {
                    var spotGUI = HittestSpotInternal(evt.mousePosition);
                    if (null != spotGUI)
                    {
                        ActionHandler.Handle(new ActionEvent(ActionEvent.ActionType.SpotBeginConnect, spotGUI, evt.mousePosition));
                    }
                    else
                    {
                        ActionHandler.ActiveObject = this.Node;
                    }
                }
                break;
            case EventType.MouseUp:
            case EventType.Ignore:
                {
                    ActionHandler.Handle(new ActionEvent(ActionEvent.ActionType.SpotEndConnect, null, evt.mousePosition));
                }
                break;
            case EventType.ContextClick:
                var menu = new GenericMenu();
                if (this.Node is Graph)
                {
                    menu.AddItem(new GUIContent("Edit..."),
                    false,
                    () =>
                    {
                        var graph = this.Node as Graph;
                        ActionHandler.Handle(new ActionEvent(ActionEvent.ActionType.GraphOpen, graph.Target, evt.mousePosition));
                    });
                }
                if (!Application.isPlaying)
                {
                    menu.AddItem(
                        new GUIContent("Delete"),
                        false,
                        () =>
                        {
                            this.Node.Dispose();
                        }
                    );
                }
                menu.ShowAsContext();
                evt.Use();
                break;
            }
        }

        private SpotGUI HittestSpotInternal(Vector2 localPos)
        {
            for (var i = 0; i < this.spots.Count; ++i)
            {
                var spot = this.spots[i];
                if (spot.Layout.Region.Contains(localPos))
                {
                    return spot;
                }
            }

            return null;
        }

        private void NotifyHandler(object target, Message message)
        {
            if (Message.DESTROY == message.Type)
            {
                Dispose();
            }
            else if (Node.NodeMessage.ADD == message.Type)
            {
                var spot = (message as Node.NodeMessage).Spot;
                AddSpot(spot);
                RefreshGUI();
            }
            else if (Node.NodeMessage.SIGNAL == message.Type)
            {
                EnableHighlight();
            }
        }

        private void RefreshGUI()
        {
            this.layout.Width = Math.Max(this.Node.Tag.Length * FONTSIZE, NODEWIDTH);

            var spotMaxWidth = 0f;
            for (var i = 0; i < this.spots.Count; ++i)
            {
                var spot = this.spots[i];
                spotMaxWidth = Mathf.Max(spotMaxWidth, spot.MaxWidth);
            }
            this.layout.Width = Mathf.Max(spotMaxWidth * 2f, this.layout.Width);

            var ins = this.spots.Where(s => s.Spot.Type == SpotType.In).ToArray();
            for (var i = 0; i < ins.Length; ++i)
            {
                ins[i].Layout.Y = TITLEHEIGHT + (i * 2 + 1) * SPOTMARGIN;
            }
            var outs = this.spots.Where(s => s.Spot.Type == SpotType.Out).ToArray();
            for (var i = 0; i < outs.Length; ++i)
            {
                outs[i].Layout.X = this.layout.Width - SPOTSIZE - 1f;
                outs[i].Layout.Y = TITLEHEIGHT + (i * 2 + 1) * SPOTMARGIN;
            }

            this.layout.Height = TITLEHEIGHT + (Math.Max(ins.Length, outs.Length) * 2 + 1.5f) * SPOTMARGIN;
        }

        private void AddSpot(Spot spot)
        {
            var element = new SpotGUI(spot, this.Manager);
            element.Layout.Parent = this.layout;
            element.Layout.Width = element.Layout.Height = SPOTSIZE;
            this.spots.Add(element);
        }
    }
}