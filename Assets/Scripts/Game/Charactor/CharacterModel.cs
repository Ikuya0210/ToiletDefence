using System;
using UnityEngine;

namespace GGGameOver.Toilet.Game
{
    public class CharacterModel : IDisposable
    {
        private string _name;
        private int _health;


        public CharacterModel(CharacterEntity entity, uint id)
        {
            _name = entity.Name;
            _health = 100;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public void TakeDamage(int damage)
        {
            _health -= damage;
            if (_health <= 0)
            {
                _health = 0;
                SetState(State.Dead);
            }
        }

        private void SetState(State newState)
        {

        }


        private enum State
        {
            None = 0,
            Idle,
            Moving,
            Attacking,
            Rest,
            Dead
        }
    }
}
