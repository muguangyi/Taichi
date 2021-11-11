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
using System.Linq;
using UnityEngine;

namespace Taichi.UNode
{
    public abstract class Node : ScriptableObject, IDisposable
    {
        public sealed class NodeMessage : Message
        {
            public const string ADD = "node.add";
            public const string SIGNAL = "node.signal";

            public NodeMessage(string type, Spot spot) : base(type)
            {
                this.Spot = spot;
            }

            public Spot Spot { get; } = null;
        }

        private List<Spot> spots = new List<Spot>();
        private SpotHideAttribute[] hideSpots = null;

        public Node()
        {
            var attr = GetType().GetCustomAttributes(typeof(NodeInterfaceAttribute), false)[0] as NodeInterfaceAttribute;
            this.Tag = attr.Tag;
            this.ID = Guid.NewGuid().ToString();
            this.hideSpots = GetType().GetCustomAttributes(typeof(SpotHideAttribute), true)
                                      .Cast<SpotHideAttribute>()
                                      .ToArray();
            this.Dispatcher = new Dispatcher(this);

            Init();
        }

        public abstract void Init();
        public abstract void Import(byte[] data);
        public abstract byte[] Export();
        public abstract void Loaded();

        public virtual void Dispose()
        {
            if (null != this.spots)
            {
                for (var i = 0; i < this.spots.Count; ++i)
                {
                    this.spots[i].Dispose();
                }
                this.spots = null;
            }

            if (null != this.Dispatcher)
            {
                this.Dispatcher.Notify(Message.Destroy);
                this.Dispatcher.Dispose();
                this.Dispatcher = null;
            }
        }

        public virtual void Recover()
        { }

        public virtual void Reclaim()
        { }

        public virtual void OnSignal(Spot spot, params object[] args)
        { }

        public void Add(Spot spot)
        {
            if (this.spots.IndexOf(spot) < 0)
            {
                spot.Index = this.spots.Count;
                this.spots.Add(spot);
                this.Dispatcher.Notify(new NodeMessage(NodeMessage.ADD, spot));
            }
        }

        public void AddRange(IEnumerable<Spot> collection)
        {
            foreach (var s in collection)
            {
                Add(s);
            }
        }

        public Spot GetAt(int index)
        {
            if (index >= 0 && index < this.spots.Count)
            {
                return this.spots[index];
            }
            else
            {
                return null;
            }
        }

        public Spot GetById(string id)
        {
            for (var i = 0; i < this.spots.Count; ++i)
            {
                var spot = this.spots[i];
                if (id == spot.ID)
                {
                    return spot;
                }
            }

            return null;
        }

        public Spot GetByName(string name)
        {
            for (var i = 0; i < this.spots.Count; ++i)
            {
                var spot = this.spots[i];
                if (name == spot.Name)
                {
                    return spot;
                }
            }

            return null;
        }

        public Spot[] Find(Func<Spot, bool> predicate)
        {
            return this.spots.Where(predicate).ToArray();
        }

        public string ID { get; set; } = null;

        public string Tag { get; set; } = null;

        public int SpotCount
        {
            get
            {
                return (null != this.spots ? this.spots.Count : 0);
            }
        }

        public Dispatcher Dispatcher { get; private set; } = null;

        internal virtual void Write(DictStreamWriter writer)
        {
            writer.Blob("Node", () =>
            {
                writer.WriteKeyString("ID", this.ID);
                writer.WriteKeyString("Tag", this.Tag);

                writer.WriteKeyInteger("SpotCount", this.spots.Count);
                for (var i = 0; i < this.spots.Count; ++i)
                {
                    writer.Blob("Spot", () =>
                    {
                        var spot = this.spots[i];
                        writer.WriteKeyString("ID", spot.ID);
                        writer.WriteKeyString("Name", spot.Name);
                    });
                }

                writer.WriteKeyBytes("Data", Export());
            });
        }

        internal virtual void Read(DictStreamReader reader)
        {
            reader.Blob("Node", () =>
            {
                this.ID = reader.ReadKeyString("ID");
                var tag = reader.ReadKeyString("Tag");
                if (!string.IsNullOrEmpty(tag)) { this.Tag = tag; }

                var count = reader.ReadKeyInteger("SpotCount");
                for (var i = 0; i < count; ++i)
                {
                    reader.Blob("Spot", () =>
                    {
                        var id = reader.ReadKeyString("ID");
                        if (i < this.spots.Count)
                        {
                            this.spots[i].ID = id;
                        }
                        var name = reader.ReadKeyString("Name");
                        if (!string.IsNullOrEmpty(name) && i < this.spots.Count)
                        {
                            this.spots[i].Name = name;
                        }
                    });
                }

                Import(reader.ReadKeyBytes("Data"));
                Loaded();
            });
        }

        internal bool IsSpotHidden(Spot spot)
        {
            return (Array.FindIndex(this.hideSpots, s => s.Name == spot.Name) >= 0);
        }
    }
}