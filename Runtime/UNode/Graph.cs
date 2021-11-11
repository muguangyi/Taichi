/*
 * This file is part of Taichi project.
 *
 * (c) MuGuangyi <muguangyi@hotmail.com>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 */

using System.Text;
using System.Linq;
using UnityEngine;
using System.Collections.Generic;
using System;
using Taichi.Stream;

namespace Taichi.UNode
{
    [NodeInterface("Graph")]
    public sealed class Graph : Node
    {
        private string target = null;
        private NodeRunner runner = null;
        private Dictionary<string, string> spotIds = new Dictionary<string, string>();

        public Graph()
        {
            this.target = null;
            this.runner = new NodeRunner();
        }

        public override void Dispose()
        {
            if (null != this.runner)
            {
                var nodes = this.runner.Nodes;
                for (var i = 0; i < nodes.Length; ++i)
                {
                    nodes[i].Dispatcher.RemoveListener(NodeMessage.SIGNAL, NotifyHandler);
                }

                this.runner.Clear();
                this.runner = null;
            }

            base.Dispose();
        }

        public override void Recover()
        {
            this.runner = NodeVM.Pick(this.target);

            var nodes = this.runner.Nodes;
            for (var i = 0; i < nodes.Length; ++i)
            {
                nodes[i].Dispatcher.AddListener(NodeMessage.SIGNAL, NotifyHandler);
            }
        }

        public override void Reclaim()
        {
            var nodes = this.runner.Nodes;
            for (var i = 0; i < nodes.Length; ++i)
            {
                nodes[i].Dispatcher.RemoveListener(NodeMessage.SIGNAL, NotifyHandler);
            }

            NodeVM.Drop(this.target, this.runner);
        }

        public override void Init()
        { }

        public override byte[] Export()
        {
            return Encoding.UTF8.GetBytes(this.target);
        }

        public override void Import(byte[] data)
        {
            this.target = Encoding.UTF8.GetString(data);
            Load();
        }

        public override void Loaded()
        { }

        public string Target
        {
            get
            {
                return this.target;
            }
            set
            {
                this.target = FormatTargetPath(value);
                Load();
            }
        }

        internal override void Write(DictStreamWriter writer)
        {
            writer.Blob("Graph", () =>
            {
                writer.WriteKeyString("ID", this.ID);
                var keys = this.spotIds.Keys.ToArray();
                writer.WriteKeyInteger("MapCount", keys.Length);
                for (var i = 0; i < keys.Length; ++i)
                {
                    writer.Blob("Map", () =>
                    {
                        var key = keys[i];
                        writer.WriteKeyString("ID", key);
                        writer.WriteKeyString("MapID", this.spotIds[key]);
                    });
                }
                writer.WriteKeyBytes("Data", Export());
            });
        }

        internal override void Read(DictStreamReader reader)
        {
            reader.Blob("Graph", () =>
            {
                this.ID = reader.ReadKeyString("ID");
                var mapCount = reader.ReadKeyInteger("MapCount");
                if (mapCount > 0)
                {
                    for (var i = 0; i < mapCount; ++i)
                    {
                        reader.Blob("Map", () =>
                        {
                            var key = reader.ReadKeyString("ID");
                            var value = reader.ReadKeyString("MapID");
                            this.spotIds.Add(key, value);
                        });
                    }
                }
                Import(reader.ReadKeyBytes("Data"));
            });
        }

        private void Load()
        {
            this.Tag = "Sub: " + this.target.Substring(this.target.LastIndexOf("/") + 1);

            var script = NodeLoader.Load(this.target);
            this.runner.Import(script);

            var nodes = this.runner.Nodes;
            for (var i = 0; i < nodes.Length; ++i)
            {
                var node = nodes[i];
                node.Dispatcher.AddListener(Node.NodeMessage.SIGNAL, NotifyHandler);

                var attrs = node.GetType().GetCustomAttributes(typeof(GraphInterfaceAttribute), false).Cast<GraphInterfaceAttribute>().ToArray();
                if (attrs.Length > 0)
                {
                    var spots = node.Find(s => { return attrs[0].Type == s.Type; });
                    for (var j = 0; j < spots.Length; ++j)
                    {
                        var spot = spots[j];
                        spot.ID = GetSpotMapId(spot.ID);
                    }
                    AddRange(spots);
                }
            }
        }

        private string FormatTargetPath(string path)
        {
            return path.Replace(Application.dataPath, "");
        }

        private string GetSpotMapId(string spotId)
        {
            string spotMapId = null;
            if (!this.spotIds.TryGetValue(spotId, out spotMapId))
            {
                this.spotIds.Add(spotId, spotMapId = Guid.NewGuid().ToString());
            }

            return spotMapId;
        }

        private void NotifyHandler(object target, Message message)
        {
            this.Dispatcher.Notify(message);
        }
    }
}