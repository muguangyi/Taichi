/*
 * This file is part of Taichi project.
 *
 * (c) MuGuangyi <muguangyi@hotmail.com>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 */

using Taichi.Binding;

namespace Taichi.UI
{
    public sealed class ViewModelNode : ModifiableNode
    {
        public ViewModelNode(ViewModel viewModel, string name) : base(viewModel, name)
        { }
    }
}
