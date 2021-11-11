/*
 * This file is part of Taichi project.
 *
 * (c) MuGuangyi <muguangyi@hotmail.com>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 */

using System;

namespace Taichi.Foundation.Internal
{
    internal interface IModule
    {
        Type Type { get; }
        object Target { get; }

        void Tick(float deltaTime);
    }
}
