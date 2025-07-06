using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;

namespace GGGameOver.Toilet.Game
{
    public class CharacterView : PooledObject<CharacterView>
    {
        /// <summary>
        /// 到着できていない場合はfalseを返す
        /// </summary>
        public Observable<bool> OnMoveCompleted => _moveCompleted.Where(_ => !_isDead);
        public Observable<bool> OnAttackCompleted => _attackCompleted.Where(_ => !_isDead);

        private Subject<bool> _moveCompleted = new();
        private Subject<bool> _attackCompleted = new();

        private float _moveSpeed = 0.1f;
        private uint _id;
        private bool _isDead = false;


        public void Init(CharacterEntity entity, uint id)
        {
            _id = id;
            var renderer = gameObject.GetComponent<SpriteRenderer>();
            renderer.sprite = entity.Sprite;
            renderer.transform.localScale = entity.SpriteScale;
            _moveSpeed = entity.MoveSpeed;

            _moveCompleted.AddTo(this);
            _attackCompleted.AddTo(this);
            _isDead = false;
        }

        public void SetTarget(uint targetId)
        {
            // Move the character to the target position
            MoveToTarget(targetId, releaseCancellationToken)
                .ContinueWith(result => _moveCompleted?.OnNext(result))
                .Forget();
        }

        private async UniTask<bool> MoveToTarget(uint targetId, CancellationToken ct)
        {
            // 移動先
            var target = TargetJudge.GetTargetTransform(targetId);
            // もう到着している場合はtrueを返す
            if (TargetJudge.IsArrived(this.transform, target))
            {
                return true;
            }

            // ループで近づく
            while (target != null || !ct.IsCancellationRequested)
            {
                await UniTask.Yield(ct);
                if (target == null)
                {
                    return false;
                }
                int dir = target.position.x > transform.position.x ? 1 : -1;
                transform.position += new Vector3(dir * _moveSpeed, 0, 0);

                if (TargetJudge.IsArrived(this.transform, target))
                {
                    return true;
                }
            }
            return false;
        }

        public void Attack(int attackPower)
        {
            AttackProcess(releaseCancellationToken)
                .ContinueWith(result => _attackCompleted?.OnNext(result))
                .Forget();

            async UniTask<bool> AttackProcess(CancellationToken ct)
            {
                Debug.Log($"{gameObject.name} 攻撃開始: {attackPower}");
                await UniTask.Delay(1500, cancellationToken: ct);
                // 攻撃処理
                Debug.Log($"{gameObject.name} 攻撃完了: {attackPower}");
                return true;
            }
        }

        public void Dead()
        {
            Debug.Log($"{gameObject.name} 死亡");
            _isDead = true;
            TargetJudge.Unregister(_id);
            ReleaseToPool();
        }
    }
}
