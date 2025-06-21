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
            SceneChanger.IsSceneChanging = true;

            await SceneManager.LoadSceneAsync(PERMANENT_SCENE_NAME, LoadSceneMode.Additive)
                .ToUniTask(cancellationToken: ct);
            await UniTask.Yield(cancellationToken: ct);

            // 後で今のシーンをActiveにするため保存
            var currentScene = SceneManager.GetActiveScene();

            // PermanentシーンからBaseShareableInitializerを探して初期化する
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(PERMANENT_SCENE_NAME));
            var initializers = GameObject.FindObjectsByType<BaseShareableInitializer>(FindObjectsSortMode.None);
            await UniTask.Yield(cancellationToken: ct);

            foreach (var initializer in initializers)
            {
                var shareable = initializer.Init();
                if (shareable != null)
                {
                    Shareables.Register(shareable);
                }
            }

            // 戻す
            SceneManager.SetActiveScene(currentScene);

            // 初回はここで初期化。フェードは挟まない。
            await root.InitBeforeShowInternal(ct);
            await root.InitInternal(ct);

            // フラグを立てる
            IsBooted = true;
            SceneChanger.IsSceneChanging = false;
        }
    }
}
