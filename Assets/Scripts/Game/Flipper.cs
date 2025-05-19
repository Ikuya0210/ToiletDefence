using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace PinballBenki.Game
{
    public class Flipper : MonoBehaviour
    {
        [SerializeField] private HingeJoint2D _hingeJoint;
        [SerializeField] private float _flipSpeed = 1000f; // 回転速度
        [SerializeField] private float _releaseSpeed = -1000f; // 戻るときの速度
        bool _isFlipping = false;

        internal void Init()
        {
            _hingeJoint.useMotor = true;
        }

        public void Flip()
        {
            if (_isFlipping)
            {
                return;
            }
            _isFlipping = true;
            FlipAsync(destroyCancellationToken)
                .ContinueWith(() => _isFlipping = false)
                .Forget();
        }

        private async UniTask FlipAsync(CancellationToken ct)
        {
            var motor = _hingeJoint.motor;
            motor.motorSpeed = _flipSpeed;
            _hingeJoint.motor = motor;
            await UniTask.WaitForSeconds(0.1f, cancellationToken: ct);
            motor.motorSpeed = _releaseSpeed;
            _hingeJoint.motor = motor;
            await UniTask.WaitForSeconds(0.4f, cancellationToken: ct);
            motor.motorSpeed = 0;
            _hingeJoint.motor = motor;
        }
    }
}
