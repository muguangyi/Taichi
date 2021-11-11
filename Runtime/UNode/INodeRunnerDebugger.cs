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

namespace Taichi.UNode
{
    public interface INodeRunnerDebugger : IDisposable
    {
        IEnumerator<NodeRunner> Runners { get; }
        void Attach(NodeRunner runner);
        void Detach(NodeRunner runner);
    }
}
