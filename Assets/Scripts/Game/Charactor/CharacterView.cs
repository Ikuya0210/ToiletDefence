using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;

namespace GGGameOver.Toilet.Game
{
    public class CharacterView : PooledObject<CharacterView>, ITakeDamage
    {
        /// <summary>
        /// 到着できていない場合はfalseを返す
        /// </summary>
        public Observable<bool> OnMoveCompleted => _moveCompleted.Where(_ => !_isDead);
        public Observable<bool> OnAttackCompleted => _attackCompleted.Where(_ => !_isDead);
        public Observable<int> OnDamageReceived => _damageReceived.Where(_ => !_isDead);

        [SerializeField] private SpriteRenderer _renderer;
        [SerializeField] private CapsuleCollider2D _capsuleCollider;

        private Subject<bool> _moveCompleted = new();
        private Subject<bool> _attackCompleted = new();
        private Subject<int> _damageReceived = new();

        private float _moveSpeed = 0.1f;
        private uint _id;
        private bool _isDead = false;

        private bool _isPlayers;
        private int _attackableLayer; //攻撃先のレイヤー
        private bool _isTakingDamage = false;


        public void Init(CharacterEntity entity, uint id, bool isPlayers)
        {
            _id = id;
            _isPlayers = isPlayers;
            _renderer.sprite = entity.Sprite;
            _renderer.color = Color.white;
            _renderer.transform.localScale = entity.SpriteScale;
            _capsuleCollider.size = new Vector2(entity.SpriteScale.x, entity.SpriteScale.y);

            _moveSpeed = entity.MoveSpeed;

            _attackableLayer = isPlayers ? Character.EnemyCharacterLayer : Character.PlayerCharacterLayer;

            _moveCompleted.AddTo(this);
            _attackCompleted.AddTo(this);
            _isDead = false;
            _isTakingDamage = false;
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
                await UniTask.Yield(PlayerLoopTiming.FixedUpdate, ct);
                if (target == null)
                {
                    return false;
                }
                int dir = target.position.x > transform.position.x ? 1 : -1;
                transform.position += new Vector3(dir * _moveSpeed / 50, 0, 0);

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
                _renderer.color = Color.blue; // 攻撃中の色に変更
                // 円形のRayを前方に飛ばして
                Vector2 direction = _isPlayers ? Vector2.right : Vector2.left;
                float radius = _capsuleCollider.size.x / 2f;
                Vector2 origin = (Vector2)transform.position + direction * (radius + 0.01f);
                var hit = Physics2D.CircleCast(origin, radius, Vector2.zero, 0f, 1 << _attackableLayer);
                // ITakeDamageのインターフェースを実装しているものにダメージを与える
                if (hit.collider != null)
                {
                    if (hit.collider.TryGetComponent<ITakeDamage>(out var target))
                    {
                        target.TakeDamage(attackPower);
                        Debug.Log($"{gameObject.name}の攻撃がヒット: {hit.collider.name}");
                    }
                    else
                    {
                        Debug.LogWarning($"{gameObject.name}の攻撃対象がITakeDamageを実装していない: {hit.collider.name}");
                    }
                }
                else
                {
                    Debug.Log($"{gameObject.name} 攻撃ミス");
                }

                await UniTask.Delay(1500, cancellationToken: ct);
                _renderer.color = Color.white; // 攻撃後の色を元に戻す
                return true;
            }
        }

        public void TakeDamage(int damage)
        {
            if (_isDead || damage <= 0 || _isTakingDamage)
            {
                return;
            }

            TakeDamageProcess(damage, releaseCancellationToken).Forget();

            async UniTask TakeDamageProcess(int damage, CancellationToken ct)
            {
                _isTakingDamage = true;
                _renderer.color = Color.red;
                _damageReceived.OnNext(damage);
                await UniTask.Delay(300, cancellationToken: ct);
                _renderer.color = Color.white;
                _isTakingDamage = false;
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
