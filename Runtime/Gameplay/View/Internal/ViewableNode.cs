/*
 * This file is part of Taichi project.
 *
 * (c) MuGuangyi <muguangyi@hotmail.com>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 */

using System;
using System.Collections.Generic;

namespace Taichi.Gameplay.Internal
{
    internal class ViewableNode : IViewableNode
    {
        protected readonly Viewable container = null;
        protected readonly Dictionary<string, ViewableNode> children = new Dictionary<string, ViewableNode>();
        private string assetReference = string.Empty;

        public ViewableNode(Viewable container)
        {
            this.container = container;
        }

        public event Action<IViewableNode> Completed;

        public string AssetReference
        {
            get => this.assetReference;

            set
            {
                if (this.assetReference != value)
                {
                    this.assetReference = value;
                    this.container.NotifyAssetReferenceChanged(this, value);
                }
            }
        }

        public virtual object View => null;

        public virtual IViewableNode Retrieve(string path)
        {
            throw new NotImplementedException("Retrieve");
        }

        protected void NotifyCompleted()
        {
            this.Completed?.Invoke(this);
        }
    }
}