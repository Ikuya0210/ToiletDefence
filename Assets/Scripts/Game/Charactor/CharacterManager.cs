using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using R3;
using System.Threading;

namespace GGGameOver.Toilet.Game
{
    public sealed class CharacterManager : MonoBehaviour
    {
        [SerializeField] private CharacterCreater _creater;
        private readonly List<CharacterModel> _characters = new();
        private readonly Queue<CharacterModel> _addCharacterQueue = new();

        public void Init()
        {
            _characters.Clear();
            _addCharacterQueue.Clear();

            _creater.Init();

            UpdateProcess(destroyCancellationToken).Forget();
        }

        public void AddRequest(CharacterEntity entity)
        {
            var c = _creater.Create(entity, this.transform, false);
            _addCharacterQueue.Enqueue(c);
        }

        private async UniTask UpdateProcess(CancellationToken ct)
        {
            Queue<CharacterModel> _disposeQueue = new();

            while (!ct.IsCancellationRequested)
            {
                await UniTask.Yield(ct);

                // キューに追加されたキャラクターをリストに追加
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
                    switch (character.State)
                    {
                        // 待機中のものにはターゲットを設定する
                        case Character.State.Idle:
                            character.SetTarget(TargetJudge.GetTargetId(true));
                            break;
                        // 死んでいたら廃棄
                        case Character.State.Dead:
                            _disposeQueue.Enqueue(character);
                            continue;
                        default:
                            break;
                    }
                }

                // 廃棄キューに入っているものを廃棄
                while (_disposeQueue.Count > 0)
                {
                    var character = _disposeQueue.Dequeue();
                    if (character != null)
                    {
                        _characters.Remove(character);
                        character.Dispose();
                    }
                }
            }
        }

        void OnDestroy()
        {
            for (int i = 0; i < _characters.Count; i++)
            {
                _characters[i].Dispose();
            }
            _characters.Clear();

            while (_addCharacterQueue.Count > 0)
            {
                var character = _addCharacterQueue.Dequeue();
                if (character != null)
                {
                    character.Dispose();
                }
            }
        }
    }
}
