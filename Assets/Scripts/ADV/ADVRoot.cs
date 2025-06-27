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
        [SerializeField] private NPCManager _npcManager;
        [SerializeField] private ADVGUICtrl _guiCtrl;

        protected override UniTask InitBeforeShow(CancellationToken ct)
        {
            _guiCtrl.Init();
            _playerCtrl.Init();
            _npcManager.Init(_guiCtrl);
            return base.Init(ct);
        }
    }
}
