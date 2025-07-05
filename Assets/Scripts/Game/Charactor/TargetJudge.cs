using System;
using System.Collections.Generic;
using UnityEngine;

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

        public static void Register(Transform target, uint id, bool isAlly)
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
            if (_entries.Exists(entry => entry.Id == id))
            {
                _entries.RemoveAll(entry => entry.Id == id);
                Debug.LogWarning($"Target with ID {id} already exists. Replacing it.");
                return;
            }
            _entries.Add(new Target(target, id, isAlly));
        }

        public static uint GetTargetId(bool isAlly)
        {
            foreach (var entry in _entries)
            {
                if (entry.IsAlly == isAlly)
                {
                    return entry.Id;
                }
            }
            Debug.LogWarning("No target found for the specified team");
            return 0; // Return 0 if no target is found
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
            return null; // Return null if no target is found
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
