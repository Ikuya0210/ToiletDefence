using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GGGameOver.Toilet.Game
{
    [CreateAssetMenu(fileName = "ScheduleData", menuName = "Game/ScheduleData")]
    public class ScheduleData : ScriptableObject
    {
        [SerializeField] private ScheduleEntry[] _entries;
        [Serializable]
        public class ScheduleEntry
        {
            [SerializeField] private float _time;
            [SerializeField] private Vector2 _position;
            [SerializeField] private CharacterEntity _characterEntity;

            public float Time => _time;
            public Vector2 Position => _position;
            public CharacterEntity CharacterEntity => _characterEntity;
        }

        public IEnumerable<ScheduleEntry> GetScheduleEntries()
        {
            var es = _entries.Where(e => e != null)
                .OrderBy(e => e.Time)
                .ThenBy(e => e.Position.x);
            return es;
        }
    }
}
