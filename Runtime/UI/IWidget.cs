﻿/*
 * This file is part of Taichi project.
 *
 * (c) MuGuangyi <muguangyi@hotmail.com>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 */

using System;

namespace Taichi.UI
{
    public interface IWidget
    {
        void OpenWindow(string window, Type windowView = null);
        void CloseWindow(string window);
    }
}
