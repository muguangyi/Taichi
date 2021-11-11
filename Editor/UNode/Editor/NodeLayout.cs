/*
 * This file is part of Taichi project.
 *
 * (c) MuGuangyi <muguangyi@hotmail.com>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 */

using Taichi.Stream;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Taichi.UNode.Editor
{
    public sealed class NodeLayout
    {
        public class Layout
        {
            private Rect data = new Rect();

            public Layout()
            { }

            public Layout(float x, float y, float width, float height)
            {
                this.data.Set(x, y, width, height);
            }

            public float X
            {
                get => this.data.x;
                set => this.data.x = value;
            }

            public float Y
            {
                get => this.data.y;
                set => this.data.y = value;
            }

            public float Width
            {
                get => this.data.width;
                set => this.data.width = value;
            }

            public float Height
            {
                get => this.data.height;
                set => this.data.height = value;
            }

            public Vector2 Position
            {
                get
                {
                    return this.data.position;
                }
            }

            public Vector2 Center
            {
                get
                {
                    return this.data.center;
                }
            }

            public Rect Region => this.data;

            public float GlobalX
            {
                get
                {
                    return this.X + (null != this.Parent ? this.Parent.GlobalX : 0);
                }
            }

            public float GlobalY
            {
                get
                {
                    return this.Y + (null != this.Parent ? this.Parent.GlobalY : 0);
                }
            }

            public Vector2 GlobalPosition
            {
                get
                {
                    return this.Position + (null != this.Parent ? this.Parent.Position : Vector2.zero);
                }
            }

            public Layout Parent { get; set; } = null;
        }

        private Dictionary<string, Layout> table = new Dictionary<string, Layout>();

        public Layout Find(string id)
        {
            Layout layout = null;
            if (!this.table.TryGetValue(id, out layout))
            {
                this.table.Add(id, layout = new Layout());
            }

            return layout;
        }

        public void Import(byte[] data)
        {
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
            this.table.Clear();
        }

        private void Write(DictStreamWriter writer)
        {
            writer.Blob("Layout", () =>
            {
                writer.WriteKeyInteger("Count", this.table.Count);
                foreach (var i in table)
                {
                    writer.WriteKeyString("ID", i.Key);
                    writer.WriteKeyFloat("X", i.Value.X);
                    writer.WriteKeyFloat("Y", i.Value.Y);
                }
            });
        }

        private void Read(DictStreamReader reader)
        {
            reader.Blob("Layout", () =>
            {
                var count = reader.ReadKeyInteger("Count");
                for (var i = 0; i < count; ++i)
                {
                    var key = reader.ReadKeyString("ID");
                    var layout = new Layout();
                    layout.X = (float)reader.ReadKeyFloat("X");
                    layout.Y = (float)reader.ReadKeyFloat("Y");
                    this.table.Add(key, layout);
                }
            });
        }
    }
}