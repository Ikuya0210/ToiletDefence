using System;
using System.Collections.Generic;
using R3.Triggers;
using UnityEngine;
using R3;

namespace GGGameOver.Toilet.Game
{
    public static class TargetJudge
    {
        private static readonly List<Target> _entries = new();
        private struct Target
        {
            public readonly Transform TargetTransform;
            public readonly uint Id;
            public readonly bool IsAlly;

            public Target(Transform targetTransform, uint id, bool isAlly)
            {
                TargetTransform = targetTransform;
                Id = id;
                IsAlly = isAlly;
            }
        }

        /// <summary>
        /// idリストに登録する。OnDestroyAsObservable()を使って、破棄時にエントリを削除している。Unregister()を使用してもよい。
        /// </summary>
        /// <param name="target">登録したいもの</param>
        /// <param name="id">生成時に取得したもの</param>
        /// <param name="isPlayers">プレイヤーにとって味方かどうか</param>
        public static void Register(Transform target, uint id, bool isPlayers)
        {
            if (target == null)
            {
                Debug.LogError("TargetTransform is null");
                return;
            }
            if (id == 0)
            {
                Debug.LogError("Id is zero");
                return;
            }

            // 重複チェック
            if (_entries.Exists(entry => entry.Id == id))
            {
                _entries.RemoveAll(entry => entry.Id == id);
                Debug.LogWarning($"Target with ID {id} already exists. Replacing it.");
                return;
            }

            // 追加
            _entries.Add(new Target(target, id, isPlayers));

            // 破棄時にエントリを削除
            target.gameObject.OnDestroyAsObservable()
                .Subscribe(_ => _entries.RemoveAll(entry => entry.Id == id))
                .AddTo(target);
        }

        public static void Unregister(uint id)
        {
            _entries.RemoveAll(entry => entry.Id == id);
        }

        /// <summary>
        /// 指定されたチームのターゲットIDを取得する。
        /// </summary>
        /// <param name="isPlayers">プレイヤーにとって味方かどうか</param>     
        public static uint GetTargetId(bool isPlayers)
        {
            foreach (var entry in _entries)
            {
                if (entry.IsAlly == isPlayers)
                {
                    return entry.Id;
                }
            }
            Debug.LogWarning("No target found for the specified team");
            return 0;
        }

        public static Transform GetTargetTransform(uint id)
        {
            foreach (var entry in _entries)
            {
                if (entry.Id == id)
                {
                    return entry.TargetTransform;
                }
            }
            Debug.LogWarning($"No target found with ID: {id}");
            return null;
        }

        public static void ClearList()
        {
            _entries.Clear();
        }

        public static bool IsArrived(Transform t1, Transform t2, float threshold = 0.2f)
        {
            if (t1 == null || t2 == null)
            {
                Debug.LogError("One or both transforms are null");
                return false;
            }
            // x軸のみで判断
            return t1.position.x >= t2.position.x - threshold &&
                   t1.position.x <= t2.position.x + threshold;
        }
    }
}
