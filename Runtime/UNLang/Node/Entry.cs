/*
 * This file is part of Taichi project.
 *
 * (c) MuGuangyi <muguangyi@hotmail.com>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 */

using Taichi.UNode;

namespace Taichi.UNLang
{
    /// <summary>
    /// Entry module defines the script start entrance.
    /// </summary>
    [NodeInterface("Entry", "UNLang/")]
    public sealed class Entry : LangNode
    {
        public override void Init()
        {
            Add(new LangSpot("", LangType.Category.Any, this, 1, SpotType.Out));
        }

        protected override void OnBegin()
        {
            GetAt(0).Signal(this.Instance);
        }
    }
}