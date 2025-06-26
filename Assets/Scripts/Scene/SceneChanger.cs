using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace PinballBenki.Scene
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
            Debug.Log("ChangeScnene_1");

            // ローディングエフェクトを表示
            if (Shareables.TryGet<IloadingEffect>(out var loadingEffect))
            {
                await loadingEffect.Show(ct);
                Debug.Log("ChangeScnene_2");
            }

            prevScene.gameObject.SetActive(false);
            await prevScene.Release(ct);

            var prevSceneInstance = UnityEngine.SceneManagement.SceneManager.GetActiveScene();

            await UniTask.Yield(cancellationToken: ct);
            Debug.Log("ChangeScnene_3");

            await UniTask.WhenAll(
                Shareables.ExecuteTransitionTasks(ct),
                UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(nextName.ToSceneNameString(), UnityEngine.SceneManagement.LoadSceneMode.Additive)
                    .ToUniTask(cancellationToken: ct)
                    .ContinueWith(() =>
                    {
                        // 新しいシーンをアクティブにする
                        var nextScene = UnityEngine.SceneManagement.SceneManager.GetSceneByName(nextName.ToSceneNameString());
                        UnityEngine.SceneManagement.SceneManager.SetActiveScene(nextScene);
                    })
            );
            Debug.Log("ChangeScnene_4");
            await UniTask.Yield(cancellationToken: ct);

            // 新しいシーンのルートを取得
            var nextSceneRoot = GameObject.FindFirstObjectByType<SceneRoot>(FindObjectsInactive.Include);
            await UniTask.Yield(cancellationToken: ct);

            if (nextSceneRoot == null)
            {
                UnityEngine.Debug.LogError($"シーン {nextName} に SceneRoot が見つかりませんでした。");
                // TODO: エラーハンドリング
                return;
            }
            Debug.Log($"ChangeScnene_5: {nextSceneRoot.SceneName}");

            // 新しいシーンのルートを初期化
            await nextSceneRoot.InitBeforeShow(ct);
            if (Shareables.TryGet<IloadingEffect>(out loadingEffect))
            {
                await loadingEffect.Hide(ct);
            }
            Debug.Log("ChangeScnene_6");
            await nextSceneRoot.Init(ct);
            nextSceneRoot.gameObject.SetActive(true);
            IsSceneChanging = false;
            _onSceneChanged = nextSceneRoot.ChangeScene;
            Debug.Log("ChangeScnene_7");

            // 非同期でシーンをアンロード
            UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(prevSceneInstance)
               .ToUniTask(cancellationToken: ct)
               .Forget();
        }
    }
}
