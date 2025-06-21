using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace PinballBenki.Scene
{
    internal static class SceneChanger
    {
        internal static bool IsSceneChanging = false;

        /// <summary>
        /// シーンを変更する
        /// </summary>
        internal static async UniTask ChangeScene(SceneRoot prevScene, SceneNames nextName, CancellationToken ct)
        {
            if (IsSceneChanging)
            {
                UnityEngine.Debug.LogWarning("シーン遷移中なので、ChangeSceneは無視されました");
                return;
            }

            IsSceneChanging = true;

            // ローディングエフェクトを表示
            if (Shareables.TryGet<IloadingEffect>(out var loadingEffect))
            {
                await loadingEffect.Show(ct);
            }

            prevScene.gameObject.SetActive(false);
            await prevScene.ReleaseInternal(ct);
            // 非同期でシーンをアンロード
            var prevSceneInstance = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
            UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(prevSceneInstance)
               .ToUniTask(cancellationToken: ct)
               .Forget();
            await UniTask.Yield(cancellationToken: ct);

            await UniTask.WhenAll(
                Shareables.ExecuteTransitionTasks(ct),
                UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(nextName.ToString(), UnityEngine.SceneManagement.LoadSceneMode.Additive)
                    .ToUniTask(cancellationToken: ct)
                    .ContinueWith(() =>
                    {
                        // 新しいシーンをアクティブにする
                        var nextScene = UnityEngine.SceneManagement.SceneManager.GetSceneByName(nextName.ToString());
                        UnityEngine.SceneManagement.SceneManager.SetActiveScene(nextScene);
                    })
            );
            await UniTask.Yield(cancellationToken: ct);

            // 新しいシーンのルートを取得
            var nextSceneRoot = GameObject.FindFirstObjectByType<SceneRoot>();
            await UniTask.Yield(cancellationToken: ct);

            if (nextSceneRoot == null)
            {
                UnityEngine.Debug.LogError($"シーン {nextName} に SceneRoot が見つかりませんでした。");
                // TODO: エラーハンドリング
                return;
            }

            // 新しいシーンのルートを初期化
            await nextSceneRoot.InitBeforeShowInternal(ct);
            if (Shareables.TryGet<IloadingEffect>(out loadingEffect))
            {
                await loadingEffect.Hide(ct);
            }
            await nextSceneRoot.InitInternal(ct);
            nextSceneRoot.gameObject.SetActive(true);
            IsSceneChanging = false;
        }
    }
}
