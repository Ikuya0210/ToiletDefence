using System.Threading;
using Cysharp.Threading.Tasks;
using PinballBenki.Scene;
using UnityEngine;

namespace PinballBenki.ADV
{
    public class ADVRoot : SceneRoot
    {
        protected override SceneNames SceneName => SceneNames.ADV;

        [SerializeField] private PlayerCtrl _playerCtrl;

        protected override UniTask Init(CancellationToken ct)
        {
            _playerCtrl.Init();
            return base.Init(ct);
        }
    }
}
