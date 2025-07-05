using System;
using R3;
using UnityEngine;

namespace GGGameOver.Toilet.Game
{
    public class CharacterModel : IDisposable
    {
        public Character.State State { get; private set; }
        private readonly Subject<uint> _move = new();
        public Observable<uint> Move => _move;// 移動先のidを通知する
        public int Id => (int)_id;
        private readonly uint _id;
        private readonly int _maxHealth;
        private readonly int _attackPower; // Assuming a default attack power
        private int _health;


        public CharacterModel(CharacterEntity entity, uint id)
        {
            _id = id;
            _health = _maxHealth = entity.Health;
            _attackPower = entity.AttackPower;
            State = Character.State.Idle;
        }

        public void Dispose()
        {
            _move.Dispose();
        }

        public void TakeDamage(int damage)
        {
            _health -= damage;
            if (_health <= 0)
            {
                _health = 0;
                SetState(Character.State.Dead);
            }
        }

        public void SetTarget(uint targetId)
        {
            // Notify the move subject with the target ID
            _move.OnNext(targetId);
            SetState(Character.State.Moving);
        }

        public void SetState(Character.State newState)
        {
            State = newState;
        }
    }
}
