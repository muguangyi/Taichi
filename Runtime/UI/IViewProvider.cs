/*
 * This file is part of Taichi project.
 *
 * (c) MuGuangyi <muguangyi@hotmail.com>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 */

using Taichi.Async;
using UnityEngine;

namespace Taichi.UI
{
    public interface IViewProvider
    {
        bool EditorBundleMode { set; }

        IAsync<GameObject> LoadAsync(string view);
    }
}

