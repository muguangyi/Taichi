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
    public abstract class SquareGUI : ElementGUI
    {
        private Texture2D highlightTexture = null;

        public SquareGUI(NodeLayout manager, Color? highlightColor = null) : base(manager, highlightColor)
        {
            this.highlightTexture = new Texture2D(1, 1);
            this.highlightTexture.SetPixel(0, 0, this.HighlightColor);
            this.highlightTexture.Apply();
        }

        public override void Dispose()
        {
            if (null != this.highlightTexture)
            {
                Object.DestroyImmediate(this.highlightTexture);
                this.highlightTexture = null;
            }

            base.Dispose();
        }

        protected void DrawHighlight()
        {
            if (null != this.layout)
            {
                GUI.DrawTexture(
                        this.layout.Region,
                        this.highlightTexture,
                        ScaleMode.StretchToFill
                );
                GUI.DrawTexture(
                    new Rect(this.layout.X - 2f, this.layout.Y - 2f, this.layout.Width + 4f, this.layout.Height + 4),
                    this.highlightTexture,
                    ScaleMode.StretchToFill,
                    true,
                    0
#if UNITY_2017_2_OR_NEWER
                    ,
                    Color.gray,
                    1.5f,
                    2f
#endif
                );
            }
        }
    }
}