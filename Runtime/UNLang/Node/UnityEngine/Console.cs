/*
 * This file is part of Taichi project.
 *
 * (c) MuGuangyi <muguangyi@hotmail.com>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 */

using UnityEngine;
using Taichi.UNode;

namespace Taichi.UNLang
{
    /// <summary>
    /// Unity Debug output module.
    /// </summary>
    [NodeInterface("Console", "UNLang/UnityEngine/")]
    public sealed class Console : LangNode
    {
        public override void Init()
        {
            Add(new LangSpot("", LangType.Category.Any, this, -1, SpotType.In));
            Add(new LangSpot("", LangType.Category.Any, this, 1, SpotType.Out));
        }

        public override void OnSignal(Spot spot, params object[] args)
        {
            if (SpotType.In == spot.Type)
            {
                Debug.Log(args[1]);
                GetAt(1).Signal(args);
            }
        }
    }
}