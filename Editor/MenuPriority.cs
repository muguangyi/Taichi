/*
 * This file is part of Taichi project.
 *
 * (c) MuGuangyi <muguangyi@hotmail.com>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 */

namespace Taichi.Editor
{
    public static class MenuPriority
    {
        private const int MenuRange = 10;

        public const int Asset = MenuRange;
        public const int ILRuntime = 2 * MenuRange;
        public const int UNLang = 3 * MenuRange;
        public const int Pipeline = 4 * MenuRange;
        public const int Analysis = 5 * MenuRange;
        public const int Preferences = 6 * MenuRange; 

        public const int Custom = 10 * MenuRange;
    }
}
