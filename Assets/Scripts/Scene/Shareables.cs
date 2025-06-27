using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace PinballBenki.Scene
{
    public static class Shareables
    {
        private static readonly Dictionary<Type, IShareable> _shareables = new();
        private static readonly List<Func<SceneNames, CancellationToken, UniTask>> _transitionTasks = new();

        internal static void Register<T>(T shareable) where T : IShareable
        {
            if (shareable == null)
            {
                Debug.LogError("Shareable cannot be null.");
                return;
            }

            var type = shareable.ShareType;
            if (_shareables.ContainsKey(type))
            {
                Debug.LogWarning($"Shareable of type {type} is already registered. Overwriting.");
            }

            _shareables[type] = shareable;
        }

        /// <summary>
        /// 指定された型のShareableを取得します。
        /// </summary>
        public static T Get<T>()
        {
            var type = typeof(T);
            if (_shareables.TryGetValue(type, out var shareable))
            {
                return (T)shareable;
            }

            Debug.LogError($"Shareable of type {type} is not registered.");
            return default;
        }

        public static bool TryGet<T>(out T shareable) where T : IShareable
        {
            var type = typeof(T);
            if (_shareables.TryGetValue(type, out var s))
            {
                shareable = (T)s;
                return true;
            }

            Debug.LogError($"Shareable of type {type} is not registered.");
            shareable = default;
            return false;
        }

        internal static void Unregister<T>() where T : IShareable
        {
            var type = typeof(T);
            if (_shareables.Remove(type))
            {
                Debug.Log($"Shareable of type {type} has been unregistered.");
            }
            else
            {
                Debug.LogWarning($"Shareable of type {type} was not found.");
            }
        }

        internal static void RegisterTransitionTask(Func<SceneNames, CancellationToken, UniTask> task)
        {
            if (task == null)
            {
                Debug.LogError("Transition task cannot be null.");
                return;
            }

            _transitionTasks.Add(task);
        }

        internal static async UniTask ExecuteTransitionTasks(SceneNames nextSceneName, CancellationToken ct)
        {
            var task = _transitionTasks.Where(t => t != null).Select(t => t(nextSceneName, ct));
            await UniTask.WhenAll(task);
        }
    }
}
