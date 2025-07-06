using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using R3;

namespace GGGameOver.Toilet.Game
{
    internal sealed class Root : SceneRoot
    {
        protected override SceneNames SceneName => SceneNames.Game;
        [SerializeField] private GameGUIManager _guiManager;
        [SerializeField] private Tower _tower;
        [SerializeField] private CharacterManager _characterManager;
        [SerializeField] private ScheduleExecuter _scheduleExecuter;
        private PointManager _pointManager;

        protected override UniTask InitBeforeShow(CancellationToken ct)
        {
            IDProvider.Reset();
            _pointManager = new();

            _tower.Init();
            _characterManager.Init();
            _characterManager.OnGetPoint += _pointManager.AddPoint;
            _guiManager.Init();

            _tower.OnDead += () =>
            {
                _characterManager.StopLoop();
                _guiManager.ShowGameOver();
            };

            _pointManager.Point
                .Subscribe(point => _guiManager.UpdatePoint(point))
                .AddTo(this);
            return UniTask.CompletedTask;
        }

        protected override UniTask Init(CancellationToken ct)
        {
            // デュエル開始ィィィ
            _scheduleExecuter.Init(_characterManager.AddRequest);
            return base.Init(ct);
        }

        void OnDestroy()
        {
            IDProvider.Reset();
            TargetJudge.ClearList();
        }
    }
}
