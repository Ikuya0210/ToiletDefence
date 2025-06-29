using System.Threading;
using Cysharp.Threading.Tasks;
using GGGameOver;
using UnityEngine;

namespace PinballBenki.Title
{
    public class TitleRoot : SceneRoot
    {
        protected override SceneNames SceneName => SceneNames.Title;

        [SerializeField] private TitleGUI _gui;

        protected override async UniTask InitBeforeShow(CancellationToken ct)
        {
            await UniTask.Yield(cancellationToken: ct);
            _gui.Init();
            await UniTask.CompletedTask;
        }
    }
}