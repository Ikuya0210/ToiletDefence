using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;

namespace GGGameOver.Toilet.Game
{
    public class CharacterModel : IDisposable
    {
        public int Id => (int)_id;
        public Character.State State { get; private set; }
        public Observable<uint> Move => _move;// 移動先のidを通知する
        public Observable<int> Attack => _attack; // 攻撃力を通知する
        public Observable<Unit> Dead => _dead; // 死亡を通知する

        private readonly Subject<uint> _move = new();
        private readonly Subject<int> _attack = new();
        private readonly Subject<Unit> _dead = new();

        private readonly CancellationTokenSource _cts = new();

        private readonly uint _id;
        private readonly int _maxHealth;
        private readonly int _attackPower; // Assuming a default attack power
        private readonly float _restTime;
        private int _health;


        public CharacterModel(CharacterEntity entity, uint id)
        {
            _id = id;
            _health = _maxHealth = entity.Health;
            _attackPower = entity.AttackPower;
            _restTime = Mathf.Max(entity.RestTime, Character.MinRestTime);
            State = Character.State.Idle;
        }

        public void Dispose()
        {
            _move.Dispose();
            _attack.Dispose();
            _cts.Cancel();
            _cts.Dispose();
        }

        public void TakeDamage(int damage)
        {
            _health -= damage;
            if (_health <= 0)
            {
                _health = 0;
                SetState(Character.State.Dead);
                _dead.OnNext(Unit.Default);
            }
        }

        public void SetTarget(uint targetId)
        {
            SetState(Character.State.Moving);
            _move.OnNext(targetId);
        }

        private void SetState(Character.State newState)
        {
            if (State == Character.State.Dead)
            {
                return;
            }
            State = newState;
        }

        public void OnMoveCompleted(bool isArrived)
        {
            if (State != Character.State.Moving)
            {
                Debug.LogError("動けない状態で移動完了を受け取りました: " + State);
                return;
            }

            if (isArrived)
            {
                SetState(Character.State.Attacking);
                _attack.OnNext(_attackPower); // Notify that the character is ready to attack
            }
            else
            {
                SetState(Character.State.Idle);
            }
        }

        public void OnAttackCompleted(bool isHit)
        {
            if (State != Character.State.Attacking)
            {
                Debug.LogError("攻撃中でない状態で攻撃完了を受け取りました: " + State);
                return;
            }

            if (isHit)
            {
                SetState(Character.State.Rest);
                UniTask.Delay((int)(_restTime * 1000)).ContinueWith(() =>
                {
                    SetState(Character.State.Idle);
                    // 死亡確認用の仮
                    TakeDamage(30); // Example damage, replace with actual logic
                }).Forget();
            }
            else
            {
                UniTask.Delay((int)(_restTime * 300)).ContinueWith(() =>
                {
                    SetState(Character.State.Idle);
                    // 死亡確認用の仮
                    TakeDamage(30); // Example damage, replace with actual logic
                }).Forget();
            }
        }
    }
}
