using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using GGGameOver;

namespace PinballBenki.Game
{
    internal sealed class Root : SceneRoot
    {
        protected override SceneNames SceneName => SceneNames.Game;
        [SerializeField] private GameGUIManager _guiManager;

        protected override UniTask InitBeforeShow(CancellationToken ct)
        {
            _guiManager.Init();
            return UniTask.CompletedTask;
        }
    }
}
