/*
 * This file is part of Taichi project.
 *
 * (c) MuGuangyi <muguangyi@hotmail.com>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 */

using System;
using System.IO;
using UnityEngine;
using Taichi.UNode;

namespace Taichi.UNLang
{
    /// <summary>
    /// Cast module to cast any value to target type.
    /// </summary>
    [NodeInterface("Cast", "UNLang/Value/")]
    public sealed class Cast : LangNode
    {
        public override void Init()
        {
            this.From = new LangType();
            this.To = new LangType();

            Add(new LangSpot("From", LangType.Category.Object, this, 1, SpotType.In));
            Add(new LangSpot("To", LangType.Category.Object, this, 1, SpotType.Out));
        }

        public override byte[] Export()
        {
            using (var stream = new MemoryStream())
            {
                using (var writer = new BinaryWriter(stream))
                {
                    var fbytes = this.From.Export();
                    writer.Write(fbytes.Length);
                    writer.Write(fbytes);
                    var tbytes = this.To.Export();
                    writer.Write(tbytes.Length);
                    writer.Write(tbytes);
                }

                return stream.ToArray();
            }
        }

        public override void Import(byte[] data)
        {
            using (var stream = new MemoryStream(data))
            {
                using (var reader = new BinaryReader(stream))
                {
                    this.From.Import(reader.ReadBytes(reader.ReadInt32()));
                    this.From.Import(reader.ReadBytes(reader.ReadInt32()));
                }
            }
        }

        public override void OnSignal(Spot spot, params object[] args)
        {
            if (SpotType.In == spot.Type)
            {
                object result = null;
                try
                {
                    switch (this.To.Type)
                    {
                    case LangType.Category.Boolean:
                        {
                            result = Convert.ChangeType(args[1], typeof(bool));
                        }
                        break;
                    case LangType.Category.Integer:
                        {
                            result = Convert.ChangeType(args[1], typeof(int));
                        }
                        break;
                    case LangType.Category.Float:
                        {
                            result = Convert.ChangeType(args[1], typeof(float));
                        }
                        break;
                    case LangType.Category.String:
                        {
                            result = Convert.ChangeType(args[1], typeof(string));
                        }
                        break;
                    case LangType.Category.Vector2:
                        {
                            result = Convert.ChangeType(args[1], typeof(Vector2));
                        }
                        break;
                    case LangType.Category.Vector3:
                        {
                            result = Convert.ChangeType(args[1], typeof(Vector3));
                        }
                        break;
                    case LangType.Category.Vector4:
                        {
                            result = Convert.ChangeType(args[1], typeof(Vector4));
                        }
                        break;
                    }
                }
                finally
                {
                    GetAt(2).Signal(args[0], result);
                }
            }
        }

        public LangType From { get; private set; } = null;

        public LangType To { get; private set; } = null;
    }
}