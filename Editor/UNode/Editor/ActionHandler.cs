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
    static class ActionHandler
    {
        private static INodeEditor editor = null;
        private static object activeObject = null;

        public static void Bind(INodeEditor editor)
        {
            ActionHandler.editor = editor;
        }

        public static void Handle(ActionEvent evt)
        {
            switch (evt.Type)
            {
            case ActionEvent.ActionType.NodeBeginClick:
                break;
            case ActionEvent.ActionType.NodeEndClick:
                ActionEvent.Clear();
                break;
            case ActionEvent.ActionType.NodeDrag:
                {
                    var node = evt.Target<NodeGUI>();
                    if (null != node)
                    {
                        var offset = evt.MousePosition - evt.Prev.MousePosition;
                        node.Layout.X += offset.x;
                        node.Layout.Y += offset.y;
                        if (node.Layout.X < 0) { node.Layout.X = 0; }
                        if (node.Layout.Y < 0) { node.Layout.Y = 0; }
                    }
                }
                break;
            case ActionEvent.ActionType.SpotBeginConnect:
                var spot = evt.Target<SpotGUI>();
                editor.DrawLinking(new LinkingGUI(spot.Spot, spot.Manager));
                break;
            case ActionEvent.ActionType.SpotEndConnect:
                {
                    editor.DrawLinking(null);
                    if (null != evt.Prev && ActionEvent.ActionType.SpotBeginConnect == evt.Prev.Type)
                    {
                        if (Application.isPlaying)
                        {
                            editor.ShowNotification("You can't modify during playing!");
                        }
                        else
                        {
                            var from = evt.Target<SpotGUI>();
                            var globalPos = evt.MousePosition + from.NodeLayout.Position;
                            var to = editor.HittestSpot(globalPos);
                            if (null != to && from.Spot.TryConnect(to.Spot))
                            {
                                editor.Add(new LinkGUI(new Link(from.Spot, to.Spot), from.Manager));
                            }
                        }
                    }
                    ActionEvent.Clear();
                }
                break;
            case ActionEvent.ActionType.GraphOpen:
                editor.Load(Application.dataPath + evt.Target<string>());
                break;
            default:
                break;
            }
        }

        public static object ActiveObject
        {
            get
            {
                return activeObject;
            }
            set
            {
                activeObject = value;
                if (activeObject is Node) { Selection.activeObject = (Object)activeObject; }
            }
        }
    }
}