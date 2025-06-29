using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace GGGameOver
{
    public static class SceneChanger
    {
        internal static bool IsSceneChanging = false;
        internal static Action<SceneNames> _onSceneChanged;

        public static void ChangeScene(SceneNames nextName)
        {
            _onSceneChanged?.Invoke(nextName);
        }

        /// <summary>
        /// シーンを変更する
        /// </summary>
        internal static async UniTask ChangeSceneInternal(SceneRoot prevScene, SceneNames nextName, CancellationToken ct)
        {
            if (IsSceneChanging)
            {
                UnityEngine.Debug.LogWarning("シーン遷移中なので、ChangeSceneは無視されました");
                return;
            }

            IsSceneChanging = true;
            _onSceneChanged = null;

            // ローディングエフェクトを表示
            if (Shareables.TryGet<IloadingEffect>(out var loadingEffect))
            {
                await loadingEffect.Show(ct);
            }

            prevScene.gameObject.SetActive(false);
            await prevScene.Release(ct);

            var prevSceneInstance = UnityEngine.SceneManagement.SceneManager.GetActiveScene();

            await UniTask.Yield(cancellationToken: ct);

            await UniTask.WhenAll(
                Shareables.ExecuteTransitionTasks(nextName, ct),
                UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(nextName.ToSceneNameString(), UnityEngine.SceneManagement.LoadSceneMode.Additive)
                    .ToUniTask(cancellationToken: ct)
                    .ContinueWith(() =>
                    {
                        // 新しいシーンをアクティブにする
                        var nextScene = UnityEngine.SceneManagement.SceneManager.GetSceneByName(nextName.ToSceneNameString());
                        UnityEngine.SceneManagement.SceneManager.SetActiveScene(nextScene);
                    })
            );
            await UniTask.Yield(cancellationToken: ct);

            // 新しいシーンのルートを取得
            var nextSceneRoot = GameObject.FindFirstObjectByType<SceneRoot>(FindObjectsInactive.Exclude);
            await UniTask.Yield(cancellationToken: ct);

            if (nextSceneRoot == null)
            {
                UnityEngine.Debug.LogError($"シーン {nextName} に SceneRoot が見つかりませんでした。");
                // TODO: エラーハンドリング
                return;
            }

            // 新しいシーンのルートを初期化
            await nextSceneRoot.InitBeforeShow(ct);
            if (Shareables.TryGet<IloadingEffect>(out loadingEffect))
            {
                await loadingEffect.Hide(ct);
            }
            nextSceneRoot.gameObject.SetActive(true);
            await nextSceneRoot.Init(ct);
            IsSceneChanging = false;
            _onSceneChanged = nextSceneRoot.ChangeScene;

            // 非同期でシーンをアンロード
            UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(prevSceneInstance)
               .ToUniTask(cancellationToken: ct)
               .Forget();
        }
    }
}
