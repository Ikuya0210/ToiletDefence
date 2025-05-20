using System.Threading;
using Cysharp.Threading.Tasks;
using PinballBenki.Communicate;
using UnityEngine;
using R3;

namespace PinballBenki.Game
{
    internal sealed class Root : MonoBehaviour
    {
        [SerializeField] private Flippers _flippers;
        private TestTCP _tcp;

        private void Start()
        {
            _flippers.Init();
            _tcp = new();
            _tcp.AddTo(this);

            if (!_tcp.IsConnect)
            {
                return;
            }
            _tcp.OnMessageReceived += message =>
            {
                Debug.Log("サーバーから受信: " + message);
            };

            _tcp.StartReceiveLoop(destroyCancellationToken).Forget();
            LoopSendAsync(destroyCancellationToken).Forget();
        }

        private async UniTask LoopSendAsync(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                await UniTask.Delay(3000, cancellationToken: ct);
                await _tcp.SendAsync("こんにちわん！", ct);
            }
        }
    }
}
