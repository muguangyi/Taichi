/*
 * This file is part of Taichi project.
 *
 * (c) MuGuangyi <muguangyi@hotmail.com>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 */

using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Taichi.Asset.Editor.Internal
{
    internal abstract class Node<T> where T : Node<T>
    {
        private string uniqueRefer = null;

        public HashSet<T> Depends { get; } = new HashSet<T>();
        public HashSet<T> Refers { get; } = new HashSet<T>();

        public void Depend(T node)
        {
            if (this == node)
            {
                return;
            }

            this.Depends.Add(node);
            if (node.Refers.Add((T)this))
            {
                node.uniqueRefer = null;
            }
        }

        public void Break(T node)
        {
            this.Depends.Remove(node);
            if (node.Refers.Remove((T)this))
            {
                node.uniqueRefer = null;
            }
        }

        public string ReferrerHash()
        {
            if (this.uniqueRefer != null)
            {
                return this.uniqueRefer;
            }

            var hashs = (from n in this.Refers select n.GetHashCode()).ToList();
            hashs.Sort();

            var sb = new StringBuilder();
            foreach (var v in hashs)
            {
                sb.Append(v.ToString());
            }

            this.uniqueRefer = sb.ToString();

            return this.uniqueRefer;
        }
    }
}
