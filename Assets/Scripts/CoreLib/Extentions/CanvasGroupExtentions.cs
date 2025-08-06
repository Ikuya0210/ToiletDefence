using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace GGGameOver
{
    public static class CanvasGroupExtentions
    {
        public static void SetEnable(this CanvasGroup canvasGroup, bool enable)
        {
            if (canvasGroup == null) return;

            canvasGroup.alpha = enable ? 1f : 0f;
            canvasGroup.interactable = enable;
            canvasGroup.blocksRaycasts = enable;
        }

        public static async UniTask SetEnableAsync(this CanvasGroup canvasGroup, bool enable, float duration, CancellationToken ct)
        {
            float targetAlpha = enable ? 1f : 0f;
            if (canvasGroup == null || Mathf.Approximately(canvasGroup.alpha, targetAlpha))
                return;

            await DOTween.To(() => canvasGroup.alpha, x => canvasGroup.alpha = x, targetAlpha, duration)
                .SetEase(Ease.InOutQuad)
                .ToUniTask(cancellationToken: ct);

            canvasGroup.interactable = enable;
            canvasGroup.blocksRaycasts = enable;
        }
    }
}
