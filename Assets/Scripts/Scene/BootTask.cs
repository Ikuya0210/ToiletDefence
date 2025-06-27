using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PinballBenki.Scene
{
    internal static class BootTask
    {
        private static bool IsBooted = false;
        private const string PERMANENT_SCENE_NAME = "Permanent";

        internal static async UniTask Boot(SceneRoot root, CancellationToken ct)
        {
            if (IsBooted)
            {
                return;
            }
            root.gameObject.SetActive(false);
            SceneChanger.IsSceneChanging = true;

            var permanent = SceneManager.GetSceneByName(PERMANENT_SCENE_NAME);
            await UniTask.Yield(cancellationToken: ct);
            if (!permanent.isLoaded)
            {
                await SceneManager.LoadSceneAsync(PERMANENT_SCENE_NAME, LoadSceneMode.Additive)
                    .ToUniTask(cancellationToken: ct);
                await UniTask.Yield(cancellationToken: ct);
            }

            // 後で今のシーンをActiveにするため保存
            var currentScene = SceneManager.GetActiveScene();

            // PermanentシーンからBaseShareableInitializerを探して初期化する
            permanent = SceneManager.GetSceneByName(PERMANENT_SCENE_NAME);
            await UniTask.Yield(cancellationToken: ct);
            SceneManager.SetActiveScene(permanent);
            await UniTask.Yield(cancellationToken: ct);

            var initializers = GameObject.FindObjectsByType<ShareableInitializer>(FindObjectsSortMode.None);
            await UniTask.Yield(cancellationToken: ct);

            foreach (var initializer in initializers)
            {
                var shareable = initializer.Init();
                if (shareable != null)
                {
                    Shareables.Register(shareable);
                    if (initializer.TransitionTask != null)
                    {
                        Shareables.RegisterTransitionTask(initializer.TransitionTask);
                    }
                }
            }
            await Shareables.ExecuteTransitionTasks(root.SceneName, ct);

            // 戻す
            SceneManager.SetActiveScene(currentScene);

            // 初回はここで初期化。フェードは挟まない。
            await root.InitBeforeShow(ct);
            root.gameObject.SetActive(true);
            await root.Init(ct);

            // フラグを立てる
            IsBooted = true;
            SceneChanger.IsSceneChanging = false;
            SceneChanger._onSceneChanged = root.ChangeScene;
        }
    }
}
