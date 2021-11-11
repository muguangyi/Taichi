/*
 * This file is part of Taichi project.
 *
 * (c) MuGuangyi <muguangyi@hotmail.com>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 */

using System;

namespace Taichi.Asset
{
    public interface IAsset : IDisposable
    {
        event Action<IAsset> OnDestroy;

        bool IsDead { get; }

        T Cast<T>();
    }
}
