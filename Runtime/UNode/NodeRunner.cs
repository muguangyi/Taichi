/*
 * This file is part of Taichi project.
 *
 * (c) MuGuangyi <muguangyi@hotmail.com>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 */

using Taichi.Stream;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Taichi.UNode
{
    public sealed class NodeRunner
    {
        private readonly List<Node> nodes = new List<Node>();
        private readonly List<Link> links = new List<Link>();

        public void Recover()
        {
            for (var i = this.nodes.Count - 1; i >= 0; --i)
            {
                this.nodes[i].Recover();
            }
        }

        public void Reclaim()
        {
            for (var i = this.nodes.Count - 1; i >= 0; --i)
            {
                this.nodes[i].Reclaim();
            }
        }

        public void Import(byte[] data)
        {
            Clear();
            using (var reader = new DictStreamReader(new MemoryStream(data), true))
            {
                Read(reader);
            }
        }

        public byte[] Export()
        {
            using (var stream = new MemoryStream())
            {
                using (var writer = new DictStreamWriter(stream))
                {
                    Write(writer);
                }

                return stream.ToArray();
            }
        }

        public void Clear()
        {
            for (var i = this.nodes.Count - 1; i >= 0; --i)
            {
                this.nodes[i].Dispose();
            }
            this.nodes.Clear();
            this.links.Clear();
        }

        public void Add(Node o)
        {
            o.Dispatcher.AddListener(Message.DESTROY, NotifyHandler);
            this.nodes.Add(o as Node);
        }

        public void Add(Link o)
        {
            o.Dispatcher.AddListener(Message.DESTROY, NotifyHandler);
            this.links.Add(o);
        }

        public void Remove(Node o)
        {
            o.Dispatcher.RemoveListener(Message.DESTROY, NotifyHandler);
            this.nodes.Remove(o);
        }

        public void Remove(Link o)
        {
            o.Dispatcher.RemoveListener(Message.DESTROY, NotifyHandler);
            this.links.Remove(o);
        }

        public Node[] Nodes
        {
            get
            {
                return this.nodes.ToArray();
            }
        }

        public Link[] Links
        {
            get
            {
                return this.links.ToArray();
            }
        }

        internal void NotifyHandler(object target, Message message)
        {
            if (Message.DESTROY == message.Type)
            {
                if (target is Node)
                {
                    Remove(target as Node);
                }
                else if (target is Link)
                {
                    Remove(target as Link);
                }
            }
            else if (NodeVM.NodeRunnerDebugMessage.START == message.Type)
            {
                var debugger = (message as NodeVM.NodeRunnerDebugMessage).Debugger;
                debugger.Attach(this);
            }
        }

        private void Write(DictStreamWriter writer)
        {
            writer.Blob("Root", () =>
            {
                writer.WriteKeyInteger("NodeCount", this.nodes.Count);
                for (var i = 0; i < this.nodes.Count; ++i)
                {
                    var node = this.nodes[i];
                    writer.WriteKeyString("FullName", node.GetType().FullName);
                    node.Write(writer);
                }

                writer.WriteKeyInteger("LinkCount", this.links.Count);
                for (var i = 0; i < this.links.Count; ++i)
                {
                    var link = this.links[i];
                    writer.WriteKeyString("S0", link.S0.ID);
                    writer.WriteKeyString("S1", link.S1.ID);
                }
            });
        }

        private void Read(DictStreamReader reader)
        {
            reader.Blob("Root", () =>
            {
                var count = reader.ReadKeyInteger("NodeCount");
                for (var i = 0; i < count; ++i)
                {
                    var fullName = reader.ReadKeyString("FullName");
                    var node = CreateInstance(fullName);
                    node.Read(reader);
                    Add(node);
                }

                count = reader.ReadKeyInteger("LinkCount");
                for (var i = 0; i < count; ++i)
                {
                    var s0 = FindSpotById(reader.ReadKeyString("S0"));
                    var s1 = FindSpotById(reader.ReadKeyString("S1"));
                    if (null != s0 && null != s1)
                    {
                        Add(new Link(s0, s1));
                    }
                }
            });
        }

        private Spot FindSpotById(string id)
        {
            for (var i = 0; i < this.nodes.Count; ++i)
            {
                var spot = this.nodes[i].GetById(id);
                if (null != spot)
                {
                    return spot;
                }
            }

            return null;
        }

        private Node CreateInstance(string fullName)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var a in assemblies)
            {
                var t = a.GetType(fullName);
                if (null != t)
                {
                    return ScriptableObject.CreateInstance(t) as Node;
                }
            }

            return new InvalidNode();
        }
    }
}