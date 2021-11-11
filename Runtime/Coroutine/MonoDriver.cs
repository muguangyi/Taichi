/*
 * This file is part of Taichi project.
 *
 * (c) MuGuangyi <muguangyi@hotmail.com>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 */

using System.Collections;
using UnityEngine;

namespace Taichi.Coroutine
{
    public sealed class MonoDriver : IMonoDriver
    {
        private sealed class Driver : MonoBehaviour
        { }

        private GameObject container = null;
        private Driver driver = null;

        public UnityEngine.Coroutine StartCoroutine(IEnumerator routine)
        {
            return this.driver.StartCoroutine(routine);
        }

        private void OnResolve()
        {
            this.container = new GameObject("MonoDriver");
            this.driver = this.container.AddComponent<Driver>();
            Object.DontDestroyOnLoad(this.container);
        }
    }
}
