using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using R3;
using System.Threading;

namespace GGGameOver.Toilet.Game
{
    public sealed class CharacterManager : MonoBehaviour
    {
        private readonly List<CharacterModel> _characters = new();
        private readonly Queue<CharacterModel> _addCharacterQueue = new();

        public void Init()
        {
            _characters.Clear();
            _addCharacterQueue.Clear();

            UpdateProcess(destroyCancellationToken).Forget();
        }

        public void AddRequest(CharacterEntity entity)
        {
            var c = Character.Create(entity, this.transform, false);
            c.AddTo(this);
            _addCharacterQueue.Enqueue(c);
        }

        private async UniTask UpdateProcess(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                await UniTask.Yield(ct);

                while (_addCharacterQueue.Count > 0)
                {
                    var character = _addCharacterQueue.Dequeue();
                    if (character != null)
                    {
                        _characters.Add(character);
                    }
                }

                foreach (var character in _characters)
                {
                    if (character.State == Character.State.Idle)
                    {
                        character.SetTarget(TargetJudge.GetTargetId(true));
                    }
                }
            }
        }
    }
}
