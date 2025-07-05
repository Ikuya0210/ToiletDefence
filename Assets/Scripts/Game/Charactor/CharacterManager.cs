using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using R3;

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
        }

        public void AddRequest(CharacterEntity entity)
        {
            var c = Character.Create(entity, this.transform);
            c.AddTo(this);
            _addCharacterQueue.Enqueue(c);
        }

        private void UpdateProcess()
        {
            while (_addCharacterQueue.Count > 0)
            {
                var character = _addCharacterQueue.Dequeue();
                if (character != null)
                {
                    _characters.Add(character);
                }
            }
        }
    }
}
