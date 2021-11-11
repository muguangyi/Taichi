/*
 * This file is part of Taichi project.
 *
 * (c) MuGuangyi <muguangyi@hotmail.com>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 */

using UnityEngine;

namespace Taichi.Gameplay.Internal
{
    internal class GObjectNode : ViewableNode
    {
        private readonly Transform parent = null;
        private Transform transform = null;

        public GObjectNode(Viewable container, Transform parent) : base(container)
        {
            this.parent = parent;
        }

        public override object View => this.transform?.gameObject;

        public override IViewableNode Retrieve(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return this;
            }

            var name = string.Empty;
            var index = path.IndexOf("/");
            if (index == 0)
            {
                // Ignore the root '/'.
                return Retrieve(path.Substring(1));
            }
            else if (index > 0)
            {
                name = path.Substring(0, index);
                path = path.Substring(index + 1);
            }
            else
            {
                name = path;
                path = string.Empty;
            }

            if (!this.children.TryGetValue(name, out ViewableNode view))
            {
                this.children.Add(name, view = new GObjectNode(this.container, this.transform));
            }

            return view.Retrieve(path);
        }

        internal Transform Transform
        {
            set
            {
                // Destroy the old one if it's exist.
                if (this.transform != null)
                {
                    Object.Destroy(this.transform.gameObject);
                    this.transform = null;
                }

                this.transform = value;

                if (this.parent != null)
                {
                    this.transform.SetParent(this.parent, false);
                }

                NotifyCompleted();
            }
        }
    }
}

