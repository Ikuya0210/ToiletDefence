using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace GGGameOver.Toilet.Game
{
    public class ScheduleExecuter : MonoBehaviour
    {
        [SerializeField] private ScheduleData _data;
        private Action<CharacterEntity> _requestInitCharacter;

        public void Init(Action<CharacterEntity> requestInitCharacter)
        {
            _requestInitCharacter = requestInitCharacter;
            ExecuteScheduleAsync(destroyCancellationToken).Forget();
        }

        private async UniTask ExecuteScheduleAsync(CancellationToken ct)
        {
            foreach (var entry in _data.GetScheduleEntries())
            {
                await UniTask.Delay(TimeSpan.FromSeconds(entry.Time), cancellationToken: ct);
                _requestInitCharacter?.Invoke(entry.CharacterEntity);
            }
        }
    }
}
