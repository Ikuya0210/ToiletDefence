using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using R3;
using VContainer;

namespace GGGameOver.Toilet.Game
{
    internal sealed class Root : SceneRoot
    {
        protected override SceneNames SceneName => SceneNames.Game;
        [SerializeField] private GameLifetimeScope _lifetimeScope;
        [Inject] private GameGUIManager _guiManager;
        [Inject] private Tower _tower;
        [Inject] private CharacterManager _characterManager;
        [Inject] private ScheduleExecuter _scheduleExecuter;
        [Inject] private PointManager _pointManager;


        protected override UniTask InitBeforeShow(CancellationToken ct)
        {
            _lifetimeScope.Build();
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
    }
}
