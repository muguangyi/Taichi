/*
 * This file is part of Taichi project.
 *
 * (c) MuGuangyi <muguangyi@hotmail.com>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 */

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Taichi.UNode.Editor
{
    [CustomEditor(typeof(Node), true)]
    public class NodeInspector : UnityEditor.Editor
    {
        private readonly Dictionary<string, bool> spots = new Dictionary<string, bool>();

        public void OnEnable()
        {
            this.spots.Clear();

            var node = this.target as Node;
            for (var i = 0; i < node.SpotCount; ++i)
            {
                this.spots.Add(node.GetAt(i).ID, true);
            }
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.Separator();

            var node = this.target as Node;
            for (var i = 0; i < node.SpotCount; ++i)
            {
                var spot = node.GetAt(i);
                this.spots[spot.ID] = EditorGUILayout.Foldout(this.spots[spot.ID], spot.Name);
                if (this.spots[spot.ID])
                {
                    for (var j = 0; j < spot.LinkCount; ++j)
                    {
                        var link = spot.GetLinkAt(j);
                        EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
                        {
                            var linkedSpot = link.GetLinkedSpot(spot);
                            if (GUILayout.Button(linkedSpot.Owner.Tag + " => " + linkedSpot.Name, EditorStyles.label))
                            {
                                ActionHandler.ActiveObject = link;
                            }
                            if (!Application.isPlaying)
                            {
                                if (GUILayout.Button("╳", EditorStyles.helpBox, GUILayout.Width(15)))
                                {
                                    link.Dispose();
                                }
                            }
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                }
            }
        }
    }
}