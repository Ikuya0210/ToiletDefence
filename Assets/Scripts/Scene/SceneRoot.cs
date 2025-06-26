using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace PinballBenki.Scene
{
    public abstract class SceneRoot : MonoBehaviour
    {
        protected internal abstract SceneNames SceneName { get; }

        /// <summary>
        /// SceneRootの初期化処理
        /// <para>シーンがロードされる前に呼び出される</para>
        protected internal virtual UniTask InitBeforeShow(CancellationToken ct) => UniTask.CompletedTask;

        /// <summary>
        /// SceneRootの初期化処理
        /// <para>シーンがロードされた後に呼び出される</para>
        protected internal virtual UniTask Init(CancellationToken ct) => UniTask.CompletedTask;

        /// <summary>
        /// SceneRootの解放処理
        /// <para>フェード処理後に呼び出される</para>
        protected internal virtual UniTask Release(CancellationToken ct) => UniTask.CompletedTask;

        // Unityが呼びます
        private void Awake()
        {
            gameObject.SetActive(false);
            BootTask.Boot(this, destroyCancellationToken).Forget();
        }

        protected internal void ChangeScene(SceneNames sceneName)
        {
            SceneChanger.ChangeSceneInternal(this, sceneName, destroyCancellationToken).Forget();
        }
    }
}
