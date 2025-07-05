using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;

namespace GGGameOver.Toilet.Game
{
    public class CharacterView : MonoBehaviour
    {
        /// <summary>
        /// 到着できていない場合はfalseを返す
        /// </summary>
        public Subject<bool> OnMoveCompleted = new();
        private float _moveSpeed = 0.1f;
        public void Init(CharacterEntity entity, uint id)
        {
            var renderer = gameObject.AddComponent<SpriteRenderer>();
            renderer.sprite = entity.Sprite;
            renderer.transform.localScale = entity.SpriteScale;
            _moveSpeed = entity.MoveSpeed;

            OnMoveCompleted.AddTo(this);
        }

        public void SetTarget(uint targetId)
        {
            // Move the character to the target position
            MoveToTarget(targetId, destroyCancellationToken)
                .ContinueWith(result => OnMoveCompleted?.OnNext(result))
                .Forget();
        }

        private async UniTask<bool> MoveToTarget(uint targetId, CancellationToken ct)
        {
            var target = TargetJudge.GetTargetTransform(targetId);
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
    }
}
