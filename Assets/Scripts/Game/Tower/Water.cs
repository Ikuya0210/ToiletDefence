using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace GGGameOver.Toilet.Game
{
    public class Water : PooledObject<Water>
    {
        [SerializeField] private float _lifetime = 5f; // 水の寿命
        [SerializeField] private float _speed = 20f; // 水の飛行速度
        private const float _gravity = -9.8f; // 重力加速度
        public bool Launched { get; private set; }

        public void Launch(Vector2 targetPoint)
        {
            if (Launched)
                return;
            Vector2 startPoint = transform.position;
            transform.position = startPoint;
            Launched = true;
            float flightTime = Vector2.Distance(startPoint, targetPoint) / _speed;
            var velocity = CalculateInitialVelocity(startPoint, targetPoint, _gravity, flightTime);
            ProcessFlight(startPoint, velocity, releaseCancellationToken)
                .Forget();
        }

        private async UniTask ProcessFlight(Vector2 startPoint, Vector2 velocity, CancellationToken ct)
        {
            var prevPosition = startPoint;
            float elapsed = 0f;

            while (Launched && !ct.IsCancellationRequested)
            {
                elapsed += Time.fixedDeltaTime;

                // 座標計算
                float x = startPoint.x + velocity.x * elapsed;
                float y = startPoint.y + velocity.y * elapsed + 0.5f * _gravity * elapsed * elapsed;

                Vector2 newPosition = new Vector2(x, y);

                // 進行方向に回転
                Vector2 moveDir = newPosition - prevPosition;
                if (moveDir.sqrMagnitude > 0.0001f)
                {
                    float angle = Mathf.Atan2(moveDir.y, moveDir.x) * Mathf.Rad2Deg;
                    transform.rotation = Quaternion.Euler(0, 0, angle);
                }

                transform.position = newPosition;
                prevPosition = newPosition;

                // 目標点を超えた後も同じ速度・加速度で進み続けます

                // 寿命チェック
                if (elapsed >= _lifetime)
                {
                    Launched = false;
                    ReleaseToPool();
                    return;
                }

                await UniTask.Yield(PlayerLoopTiming.FixedUpdate, cancellationToken: ct);
            }
        }

        Vector2 CalculateInitialVelocity(Vector2 start, Vector2 target, float gravity, float time)
        {
            float vx = (target.x - start.x) / time;
            float vy = (target.y - start.y - 0.5f * gravity * time * time) / time;
            return new Vector2(vx, vy);
        }
    }
}
