using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace GGGameOver.Toilet.Game
{
    internal sealed class Root : SceneRoot
    {
        protected override SceneNames SceneName => SceneNames.Game;
        [SerializeField] private GameGUIManager _guiManager;
        [SerializeField] private CharacterManager _characterManager;
        [SerializeField] private ScheduleExecuter _scheduleExecuter;

        protected override UniTask InitBeforeShow(CancellationToken ct)
        {
            _characterManager.Init();
            _guiManager.Init();
            return UniTask.CompletedTask;
        }

        protected override UniTask Init(CancellationToken ct)
        {
            // デュエル開始ィィィ
            _scheduleExecuter.Init(_characterManager.AddRequest);
            return base.Init(ct);
        }
    }
}
