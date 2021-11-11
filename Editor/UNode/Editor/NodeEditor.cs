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
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Taichi.UNode.Editor
{
    public interface INodeEditor
    {
        void Add(NodeGUI o);
        void Add(LinkGUI o);
        void Remove(NodeGUI o);
        void Remove(LinkGUI o);
        void DrawLinking(LinkingGUI o);
        SpotGUI HittestSpot(Vector2 globalPos);
        void Load(string filePath);
        void ShowNotification(string message);
    }

    public class NodeEditor<NT> : EditorWindow, INodeEditor where NT : Node
    {
        private static readonly string DEFAULT_FILEPATH = "_untitled";
        private static readonly int DRAGNODESCONTROLID = "NodeEditor.HandleDragNodes".GetHashCode();
        private static readonly float WINDOW_SPAN = 20f;

        private readonly NodeRunner runner = null;
        private readonly NodeLayout layout = null;
        private NodeRunner currentRunner = null;
        private readonly List<NodeGUI> nodes = new List<NodeGUI>();
        private readonly List<LinkGUI> links = new List<LinkGUI>();
        private LinkingGUI linking = null;
        private List<CustomNode> customNodes = null;
        private Rect graphRegion = new Rect();
        private Vector2 scrollPos = new Vector2();
        private Vector2 spaceRightBottom = new Vector2();

        public NodeEditor()
        {
            ActionHandler.Bind(this);
            NodeLoader.Load = LoadTarget;
            this.runner = new NodeRunner();
            this.layout = new NodeLayout();
            this.currentRunner = this.runner;
            this.graphRegion.size = this.position.size;
        }

        public virtual void OnGUI()
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                DrawGrid();

                using (var scrollScope = new EditorGUILayout.ScrollViewScope(this.scrollPos))
                {
                    this.scrollPos = scrollScope.scrollPosition;

                    if (null != this.linking)
                    {
                        this.linking.OnGUI();
                    }

                    for (var i = 0; i < this.links.Count; ++i)
                    {
                        this.links[i].OnGUI();
                    }

                    BeginWindows();
                    for (var i = 0; i < this.nodes.Count; ++i)
                    {
                        this.nodes[i].OnGUI();
                    }
                    HandleDragNodes();
                    EndWindows();

                    HandleGraphGUIEvents();

                    if (this.nodes.Any() && EventType.Layout == Event.current.type)
                    {
                        GUILayoutUtility.GetRect(
                            new GUIContent(string.Empty), GUIStyle.none, GUILayout.Width(this.spaceRightBottom.x), GUILayout.Height(this.spaceRightBottom.y));
                    }
                }

                if (Event.current.type == EventType.Repaint)
                {
                    var newRegion = GUILayoutUtility.GetLastRect();
                    if (newRegion != this.graphRegion)
                    {
                        this.graphRegion = newRegion;
                        Repaint();
                    }
                }
            }

            HandleGUIEvent();
        }

        public void OnInspectorUpdate()
        {
            Repaint();
        }

        public void Add(NodeGUI o)
        {
            this.currentRunner.Add(o.Node);
            AddGUI(o);
        }

        public void Add(LinkGUI o)
        {
            this.currentRunner.Add(o.Link);
            AddGUI(o);
        }

        public void Remove(NodeGUI o)
        {
            this.currentRunner.Remove(o.Node);
            RemoveGUI(o);
        }

        public void Remove(LinkGUI o)
        {
            this.currentRunner.Remove(o.Link);
            RemoveGUI(o);
        }

        public void DrawLinking(LinkingGUI o)
        {
            if (null != o)
            {
                this.linking = o;
                this.linking.Dispatcher.AddListener(Message.DESTROY, NotifyHandler);
            }
            else if (null != this.linking)
            {
                this.linking.Dispatcher.RemoveListener(Message.DESTROY, NotifyHandler);
                this.linking = null;
            }
        }

        public SpotGUI HittestSpot(Vector2 globalPos)
        {
            for (var i = 0; i < this.nodes.Count; ++i)
            {
                var spotGui = this.nodes[i].HittestSpot(globalPos);
                if (null != spotGui)
                {
                    return spotGui;
                }
            }

            return null;
        }

        public void Load(string filePath)
        {
            LoadScript(filePath);
        }

        public void ShowNotification(string message)
        {
            ShowNotification(new GUIContent(message));
        }

        protected string FilePath { get; private set; } = DEFAULT_FILEPATH;

        protected NodeRunner CurrentRunner
        {
            get
            {
                return this.currentRunner;
            }
            set
            {
                if (value == this.currentRunner)
                {
                    return;
                }

                if (null == value)
                {
                    this.currentRunner = this.runner;
                }
                else
                {
                    this.currentRunner = value;
                }
                UpdateGUI();
            }
        }

        protected event Action<string> OnLoadCompleted;

        private byte[] LoadTarget(string target)
        {
            using (var stream = new FileStream(Application.dataPath + target, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var data = new byte[stream.Length];
                stream.Read(data, 0, data.Length);
                return data;
            }
        }

        private void LoadScript(string filePath)
        {
            this.FilePath = filePath;
            this.runner.Clear();
            this.layout.Clear();
            using (var stream = new FileStream(this.FilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var data = new byte[stream.Length];
                stream.Read(data, 0, data.Length);
                this.runner.Import(data);
            }
            using (var stream = new FileStream(this.FilePath + ".layout", FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var data = new byte[stream.Length];
                stream.Read(data, 0, data.Length);
                this.layout.Import(data);
            }
            this.currentRunner = this.runner;
            UpdateGUI();

            if (null != this.OnLoadCompleted)
            {
                this.OnLoadCompleted(this.FilePath);
            }
        }

        private void SaveScript(string filePath)
        {
            this.FilePath = filePath;
            var data = this.runner.Export();
            using (var stream = new FileStream(this.FilePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Write))
            {
                stream.Write(data, 0, data.Length);
                stream.SetLength(data.Length);
            }
            data = this.layout.Export();
            using (var stream = new FileStream(this.FilePath + ".layout", FileMode.OpenOrCreate, FileAccess.Write, FileShare.Write))
            {
                stream.Write(data, 0, data.Length);
                stream.SetLength(data.Length);
            }
        }

        private void AddGraphScript(string filePath, Vector2 pos)
        {
            var graph = ScriptableObject.CreateInstance<Graph>();
            graph.Target = filePath;
            var element = new NodeGUI(graph, this.layout);
            element.Layout.X = pos.x;
            element.Layout.Y = pos.y;
            Add(element);
        }

        private void UpdateGUI()
        {
            for (var i = this.nodes.Count - 1; i >= 0; --i)
            {
                this.nodes[i].Dispose();
            }
            for (var i = this.links.Count - 1; i >= 0; --i)
            {
                this.links[i].Dispose();
            }

            for (var i = 0; i < this.currentRunner.Nodes.Length; ++i)
            {
                AddGUI(new NodeGUI(this.currentRunner.Nodes[i], this.layout));
            }
            for (var i = 0; i < this.currentRunner.Links.Length; ++i)
            {
                AddGUI(new LinkGUI(this.currentRunner.Links[i], this.layout));
            }
            UpdateSpaceRect();
        }

        private void AddGUI(NodeGUI o)
        {
            o.Dispatcher.AddListener(Message.DESTROY, NotifyHandler);
            this.nodes.Add(o as NodeGUI);
        }

        private void AddGUI(LinkGUI o)
        {
            o.Dispatcher.AddListener(Message.DESTROY, NotifyHandler);
            this.links.Add(o);
        }

        private void RemoveGUI(NodeGUI o)
        {
            o.Dispatcher.RemoveListener(Message.DESTROY, NotifyHandler);
            this.nodes.Remove(o);
        }

        private void RemoveGUI(LinkGUI o)
        {
            o.Dispatcher.RemoveListener(Message.DESTROY, NotifyHandler);
            this.links.Remove(o);
        }

        private void DrawGrid()
        {
            if (Event.current.type != EventType.Repaint)
            {
                return;
            }

            UnityEditor.Graphs.Styles.graphBackground.Draw(this.graphRegion, false, false, false, false);

            GL.PushMatrix();
            GL.Begin(GL.LINES);

            DrawGridLines(12f, new Color(0f, 0f, 0f, 0.12f));

            GL.End();
            GL.PopMatrix();
        }

        private void DrawGridLines(float gridSize, Color gridColor)
        {
            GL.Color(gridColor);
            for (float x = this.graphRegion.xMin - (this.graphRegion.xMin % gridSize) - this.scrollPos.x; x < this.graphRegion.xMax; x += gridSize)
            {
                if (x < this.graphRegion.xMin)
                {
                    continue;
                }
                DrawLine(new Vector2(x, this.graphRegion.yMin), new Vector2(x, this.graphRegion.yMax));
            }
            GL.Color(gridColor);
            for (float y = this.graphRegion.yMin - (this.graphRegion.yMin % gridSize) - this.scrollPos.x; y < this.graphRegion.yMax; y += gridSize)
            {
                if (y < this.graphRegion.yMin)
                {
                    continue;
                }
                DrawLine(new Vector2(this.graphRegion.xMin, y), new Vector2(this.graphRegion.xMax, y));
            }
        }

        private void DrawLine(Vector2 p1, Vector2 p2)
        {
            GL.Vertex(p1);
            GL.Vertex(p2);
        }

        private void HandleDragNodes()
        {
            Event evt = Event.current;
            int id = GUIUtility.GetControlID(DRAGNODESCONTROLID, FocusType.Passive);
            switch (evt.GetTypeForControl(id))
            {
            case EventType.MouseDown:
                if (0 == evt.button)
                {
                    for (var i = 0; i < this.nodes.Count; ++i)
                    {
                        if (this.nodes[i].Layout.Region.Contains(evt.mousePosition))
                        {
                            ActionHandler.Handle(new ActionEvent(ActionEvent.ActionType.NodeBeginClick, this.nodes[i], evt.mousePosition));
                            break;
                        }
                    }
                }
                break;
            case EventType.MouseUp:
                UpdateSpaceRect();
                ActionHandler.Handle(new ActionEvent(ActionEvent.ActionType.NodeEndClick, null, evt.mousePosition));
                break;
            case EventType.MouseDrag:
                ActionHandler.Handle(new ActionEvent(ActionEvent.ActionType.NodeDrag, null, evt.mousePosition));
                evt.Use();
                break;
            }
        }

        private void HandleGUIEvent()
        {
            var evt = Event.current;
            switch (evt.type)
            {
            case EventType.ContextClick:
                ShowNodeContextMenu(evt.mousePosition);
                break;
            default:
                break;
            }
        }

        private void HandleGraphGUIEvents()
        { }

        private void UpdateSpaceRect()
        {
            if (this.nodes.Any())
            {
                var rightNode = this.nodes.OrderByDescending(node => node.Layout.Region.xMax).First();
                var bottomNode = this.nodes.OrderByDescending(node => node.Layout.Region.yMax).First();
                this.spaceRightBottom = new Vector2(rightNode.Layout.Region.xMax + WINDOW_SPAN, bottomNode.Layout.Region.yMax + WINDOW_SPAN);
            }
        }

        private void ShowNodeContextMenu(Vector2 pos)
        {
            var menu = new GenericMenu();

            menu.AddItem(new GUIContent("New"), false, () =>
            {
                this.FilePath = DEFAULT_FILEPATH;
                this.runner.Clear();
                this.layout.Clear();
                this.currentRunner = this.runner;
                UpdateGUI();

                this.OnLoadCompleted?.Invoke(this.FilePath);
            });
            menu.AddItem(new GUIContent("Load..."), false, () =>
            {
                var filePath = EditorUtility.OpenFilePanel("Open", null, "bytes");
                if (!string.IsNullOrEmpty(filePath))
                {
                    LoadScript(filePath);
                }
            });
            menu.AddItem(new GUIContent("Save..."), false, () =>
            {
                var filePath = EditorUtility.SaveFilePanel("Save", null, "untitled", "bytes");
                if (!string.IsNullOrEmpty(filePath))
                {
                    SaveScript(filePath);
                }
            });
            menu.AddSeparator("");
            menu.AddItem(new GUIContent("Graph..."), false, () =>
            {
                var filePath = EditorUtility.OpenFilePanel("Open", null, "bytes");
                if (!string.IsNullOrEmpty(filePath))
                {
                    AddGraphScript(filePath, pos);
                }
            });
            menu.AddSeparator("");

            var nodeTypes = this.CustomNodeTypes;
            for (var i = 0; i < nodeTypes.Count; ++i)
            {
                var n = nodeTypes[i];
                menu.AddItem(
                    new GUIContent(n.Tag),
                    false,
                    () =>
                    {
                        if (Application.isPlaying)
                        {
                            ShowNotification("You can't modify during playing!");
                        }
                        else
                        {
                            var node = n.CreateInstance();
                            var element = new NodeGUI(node, this.layout);
                            element.Layout.X = pos.x + this.scrollPos.x;
                            element.Layout.Y = pos.y + this.scrollPos.y;
                            Add(element);
                        }
                    }
                );
            }

            menu.ShowAsContext();
        }

        private List<CustomNode> CustomNodeTypes
        {
            get
            {
                if (null == this.customNodes)
                {
                    this.customNodes = BuildCustomNodeList(typeof(NT));
                }

                return this.customNodes;
            }
        }

        private void NotifyHandler(object target, Message message)
        {
            if (Message.DESTROY == message.Type)
            {
                if (target is NodeGUI)
                {
                    RemoveGUI(target as NodeGUI);
                }
                else if (target is LinkGUI)
                {
                    RemoveGUI(target as LinkGUI);
                }
                Repaint();
            }
        }

        private static List<CustomNode> BuildCustomNodeList(Type baseType)
        {
            var list = new List<CustomNode>();

            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var a in assemblies)
            {
                var nodes = a.GetTypes()
                             .Where(t => t != baseType)
                             .Where(t => !t.IsAbstract &&
                                         baseType.IsAssignableFrom(t) &&
                                         t.GetCustomAttributes(typeof(NodeInterfaceAttribute), false).Length > 0);
                foreach (var n in nodes)
                {
                    list.Add(new CustomNode(n));
                }
            }
            list.Sort((n0, n1) => { return n0.Tag.CompareTo(n1.Tag); });

            return list;
        }
    }
}