using System;
using System.Collections.Concurrent;
using UnityEditor;
using UnityEngine;

namespace PrometheanAI.Modules.Utils
{
    /// <summary>
    /// Class which is used to execute actions in the Main thread of the UnityEngine
    /// </summary>
    public static class MainThreadDispatcher
    {
        private const float k_maxExecutionTime = 0.005f;
        private static ConcurrentQueue<Action> s_queue = new ConcurrentQueue<Action>();

        static MainThreadDispatcher() {
            EditorApplication.update += Update;
        }

        public static void Enqueue(Action action) {
            s_queue.Enqueue(action);
        }

        public static void Reset() {
            s_queue = new ConcurrentQueue<Action>();
        }

        private static void Update() {
            var time = Time.realtimeSinceStartup;
            while (s_queue.TryDequeue(out var action)) {
                action?.Invoke();
                if (Time.realtimeSinceStartup - time > k_maxExecutionTime) {
                    break;
                }
            }
        }
    }
}