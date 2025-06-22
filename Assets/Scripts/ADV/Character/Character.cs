using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace PinballBenki.ADV
{
    public class Character : MonoBehaviour
    {
        [SerializeField] private float _moveSpeed = 1.0f;
        private const float TALKABLE_DISTANCE = 1.1f;
        private States _state = States.Idle;

        private int _characterLayerMask;

        public void Init()
        {
            _characterLayerMask = LayerMask.GetMask("Character");// ここで入れないと怒られる
        }

        public void Move(Vector2 dir)
        {
            if (_state != States.Idle || dir.IsZero())
            {
                return;
            }
            MoveAsync(dir, destroyCancellationToken).Forget();

            async UniTask MoveAsync(Vector2 dir, CancellationToken ct)
            {
                _state = States.Walk;
                Vector3 dir3d = new Vector3(dir.x, 0, dir.y);
                Vector3 dir3dNorm = dir3d.normalized;
                var targetPos = transform.position + dir3d * _moveSpeed;
                // 目的方向を向いていたら移動する.
                // 目的方向を向いていなかったら、目的方向を向く。
                if (Vector3.Dot(transform.forward, dir3dNorm) < 0.9f)
                {
                    var targetRotation = Quaternion.LookRotation(dir3dNorm, Vector3.up);
                    await transform.DORotateQuaternion(targetRotation, 0.2f)
                        .SetEase(Ease.Linear)
                        .ToUniTask(cancellationToken: ct);
                }
                else
                {
                    Ray ray = new(transform.position, transform.forward);
                    if (Physics.Raycast(ray, out var _, TALKABLE_DISTANCE, _characterLayerMask))
                    {
                        await UniTask.Delay(100, cancellationToken: ct);
                        _state = States.Idle;
                        return; // キャラクターに当たった場合は移動しない
                    }
                    await transform.DOMove(targetPos, 0.5f)
                        .SetEase(Ease.Linear)
                        .ToUniTask(cancellationToken: ct);
                }
                _state = States.Idle;
            }
        }

        public void Talk()
        {
            if (_state != States.Idle)
            {
                return;
            }
            _state = States.Talk;
            // 前にRayを飛ばして、キャラクターレイヤーに当たるか確認
            Ray ray = new(transform.position, transform.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, TALKABLE_DISTANCE, _characterLayerMask))
            {
                Debug.Log($"Hit character: {hit.collider.gameObject.name}");
                if (hit.collider.TryGetComponent<ITalkable>(out var talkable))
                {
                    talkable.TalkAsync(this, destroyCancellationToken)
                        .SuppressCancellationThrow()
                        .ContinueWith(success => _state = States.Idle)
                        .Forget();
                    return;
                }
            }
            else
            {
                // キャラクターに当たらなかった場合の処理
                Debug.Log("No character to talk to.");
            }
            _state = States.Idle;
        }


        public enum States
        {
            Idle = 0,
            Walk,
            Talk,
        }
    }
}
