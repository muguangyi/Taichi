/*
 * This file is part of Taichi project.
 *
 * (c) MuGuangyi <muguangyi@hotmail.com>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 */

using System;
using UnityEngine;

namespace Taichi.UNode.Editor
{
    public abstract class ElementGUI : IDisposable
    {
        private const float HIGHLIGHT_DURATION = 0.1f;

        protected NodeLayout.Layout layout = null;
        private Color? highlightColor = null;
        private float highlightTimer = 0;

        public ElementGUI(NodeLayout manager, Color? highlightColor = null)
        {
            this.Manager = manager;
            this.highlightColor = highlightColor ?? new Color(1f, 1f, 0);
        }

        public virtual void Dispose()
        { }

        public virtual void OnGUI()
        {
            if (this.highlightTimer > 0)
            {
                this.highlightTimer -= Time.deltaTime;
                if (this.highlightTimer <= 0)
                {
                    this.highlightTimer = 0;
                }
            }
        }

        public NodeLayout Manager { get; } = null;

        public void EnableHighlight()
        {
            this.highlightTimer = HIGHLIGHT_DURATION;
        }

        protected Color HighlightColor
        {
            get
            {
                return this.highlightColor.Value;
            }
        }

        protected bool IsHighlighted
        {
            get
            {
                return (this.highlightTimer > 0);
            }
        }
    }
}