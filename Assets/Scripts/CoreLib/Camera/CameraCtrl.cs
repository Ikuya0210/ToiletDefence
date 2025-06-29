using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace GGGameOver
{
    public sealed class CameraCtrl : ShareableInitializer, IShareable
    {
        Type IShareable.ShareType => typeof(CameraCtrl);

        [SerializeField] private Camera _advCamera;
        [SerializeField] private Camera _gameCamera;

        public override IShareable Init()
        {
            return this;
        }

        public override Func<SceneNames, CancellationToken, UniTask> TransitionTask =>
             (sceneName, token) =>
        {
            switch (sceneName)
            {
                case SceneNames.Game:
                    _advCamera.gameObject.SetActive(false);
                    _gameCamera.gameObject.SetActive(true);
                    break;
                default:
                    _advCamera.gameObject.SetActive(true);
                    _gameCamera.gameObject.SetActive(false);
                    break;
            }
            return UniTask.CompletedTask;
        };

        internal void SetCameraEditor(SceneNames sceneName)
        => TransitionTask?.Invoke(sceneName, CancellationToken.None).Forget();
    }
}
