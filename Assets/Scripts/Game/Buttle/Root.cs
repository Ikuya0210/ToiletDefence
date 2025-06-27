using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using PinballBenki.Scene;

namespace PinballBenki.Game
{
    internal sealed class Root : SceneRoot
    {
        protected override SceneNames SceneName => SceneNames.Game;
        [SerializeField] private Flippers _flippers;
        [SerializeField] private GameGUIManager _guiManager;

        protected override UniTask InitBeforeShow(CancellationToken ct)
        {
            _flippers.Init();
            _guiManager.Init();
            return UniTask.CompletedTask;
        }
    }
}
