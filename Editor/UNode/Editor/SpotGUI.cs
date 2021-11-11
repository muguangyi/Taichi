/*
 * This file is part of Taichi project.
 *
 * (c) MuGuangyi <muguangyi@hotmail.com>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 */

using UnityEngine;

namespace Taichi.UNode.Editor
{
    public sealed class SpotGUI : SquareGUI
    {
        private const float FONTSIZE = 8f;

        private readonly Texture2D normalTexture = null;
        private readonly Texture2D connectTexture = null;

        public SpotGUI(Spot spot, NodeLayout manager) : base(manager, new Color(1f, 0.4f, 0.4f))
        {
            this.Spot = spot;
            this.layout = manager.Find(spot.ID);

            this.Spot.Owner.Dispatcher.AddListener(Node.NodeMessage.SIGNAL, NotifyHandler);

            this.normalTexture = new Texture2D(1, 1);
            this.normalTexture.SetPixel(0, 0, Color.gray);
            this.normalTexture.Apply();

            this.connectTexture = new Texture2D(1, 1);
            this.connectTexture.SetPixel(0, 0, spot.Type == SpotType.In ? new Color(1f, 0.5f, 0f) : new Color(0.13f, 0.69f, 0.3f));
            this.connectTexture.Apply();
        }

        public override void Dispose()
        {
            this.Spot.Owner.Dispatcher.RemoveListener(Node.NodeMessage.SIGNAL, NotifyHandler);
            this.Spot = null;

            base.Dispose();
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
                GUI.backgroundColor = new Color(1f, 1f, 1f, 0.7f);
                GUI.Button(new Rect(this.layout.X, this.layout.Y, this.layout.Width, this.layout.Height), string.Empty, new GUIStyle(GUI.skin.box));
            }
            GUI.backgroundColor = originColor;
            var size = new GUIStyle(GUI.skin.label).CalcSize(new GUIContent(this.Spot.Name));
            GUI.DrawTexture(
                new Rect(this.layout.X + 2f, this.layout.Y + 2f, this.layout.Width - 4f, this.layout.Height - 4f),
                this.Spot.LinkCount > 0 ? this.connectTexture : this.normalTexture,
                ScaleMode.StretchToFill
            );
            if (this.Spot.Type == SpotType.In)
            {
                GUI.Label(new Rect(this.layout.X + this.layout.Width, this.layout.Y - 2f, size.x, size.y), this.Spot.Name);
            }
            else
            {
                GUI.Label(new Rect(this.layout.X - size.x, this.layout.Y - 2f, size.x, size.y), this.Spot.Name);
            }
        }

        public Spot Spot { get; private set; } = null;

        public NodeLayout.Layout Layout
        {
            get
            {
                return this.layout;
            }
        }

        public NodeLayout.Layout NodeLayout
        {
            get
            {
                return this.layout.Parent;
            }
        }

        public float MaxWidth
        {
            get
            {
                return this.Spot.Name.Length * FONTSIZE + this.layout.Width;
            }
        }

        private void NotifyHandler(object target, Message message)
        {
            if (Node.NodeMessage.SIGNAL == message.Type)
            {
                var spot = (message as Node.NodeMessage).Spot;
                if (spot == this.Spot)
                {
                    EnableHighlight();
                }
            }
        }
    }
}