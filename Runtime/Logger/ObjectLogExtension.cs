/*
 * This file is part of Taichi project.
 *
 * (c) MuGuangyi <muguangyi@hotmail.com>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 */

namespace Taichi.Logger
{
    public static class ObjectLogExtension
    {
        public static void Verbose(this object obj, object verbose)
        {
            Log.Verbose(verbose, obj);
        }

        public static void Debug(this object obj, object debug)
        {
            Log.Debug(debug, obj);
        }

        public static void Info(this object obj, object info)
        {
            Log.Info(info, obj);
        }

        public static void Warn(this object obj, object warn)
        {
            Log.Warn(warn, obj);
        }

        public static void Error(this object obj, object error)
        {
            Log.Error(error, obj);
        }

        public static void Fatal(this object obj, object fatal)
        {
            Log.Fatal(fatal, obj);
        }
    }
}

