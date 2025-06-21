using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace PinballBenki.ADV
{
    public class Character : MonoBehaviour
    {
        [SerializeField] private float _moveSpeed = 1.0f;
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
                await transform.DOMove(transform.position + dir3d * _moveSpeed, 0.5f)
                    .SetEase(Ease.Linear)
                    .ToUniTask(cancellationToken: ct);
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
            Ray ray = new(transform.position, Vector3.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, 1.0f, _characterLayerMask))
            {
                // キャラクターに当たった場合の処理
                Debug.Log($"Talking to character: {hit.collider.name}");
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
