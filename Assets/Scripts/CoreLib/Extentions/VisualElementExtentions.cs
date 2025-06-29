using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine.UIElements;
using R3;
using System;

namespace GGGameOver
{
    public static class VisualElementExtentions
    {
        public static void SetEnable(this VisualElement element, bool enable)
        {
            if (element == null)
            {
                throw new System.ArgumentNullException(nameof(element), "VisualElement cannot be null.");
            }

            if (enable)
            {
                element.style.display = DisplayStyle.Flex;
                element.style.opacity = 1f;
            }
            else
            {
                element.style.display = DisplayStyle.None;
                element.style.opacity = 0f;
            }
        }

        public static UniTask SetEnableAsync(this VisualElement element, bool enable, float duration, CancellationToken ct)
             => element.SetEnableAsync(enable, duration, enable ? 1f : 0f, ct);

        public static async UniTask SetEnableAsync(this VisualElement element, bool enable, float duration, float opacity, CancellationToken ct)
        {
            if (enable)
            {
                element.style.opacity = 0;
                element.style.display = DisplayStyle.Flex;
            }

            if (element == null)
            {
                throw new System.ArgumentNullException(nameof(element), "VisualElement cannot be null.");
            }

            await element.DoFade(enable ? opacity : 0f, duration)
                .ToUniTask(cancellationToken: ct);


            if (!enable)
            {
                element.style.display = DisplayStyle.None;
            }
        }

        public static Tween DoFade(this VisualElement element, float endValue, float duration)
        {
            if (element == null)
            {
                throw new System.ArgumentNullException(nameof(element), "VisualElement cannot be null.");
            }

            return DOTween.To(() => element.style.opacity.value, x => element.style.opacity = x, endValue, duration)
                .SetTarget(element);
        }

        /// <summary>
        /// 一文字ずつ表示する拡張 (遅延時間0.05秒)　キャンセルで全文表示
        /// </summary>
        public static UniTask SetTextAsync(this Label label, string text, CancellationToken ct)
            => label.SetTextAsync(text, 0.05f, ct);


        /// <summary>
        /// 一文字ずつ表示する拡張 (遅延時間指定) キャンセルで全文表示
        /// </summary>
        public static async UniTask SetTextAsync(this Label label, string text, float delay, CancellationToken ct)
        {
            label.text = string.Empty;

            await SetTextInternal(text, (int)(delay * 1000), ct)
                .SuppressCancellationThrow()
                .ContinueWith(isCancel =>
                {
                    if (isCancel)
                    {
                        label.text = text;
                    }
                });

            async UniTask SetTextInternal(string text, int delayMS, CancellationToken ct)
            {
                foreach (char c in text)
                {
                    label.text += c;
                    await UniTask.Delay(delayMS, cancellationToken: ct);
                }
            }
        }

        public static Observable<Unit> OnClickAsObservable(this Button button, CancellationToken token = default)
        {
            return Observable.FromEvent(
                e => button.clicked += e,
                e => button.clicked -= e, token);
        }

        public static Observable<T> OnValueChangedAsObservable<T>(this INotifyValueChanged<T> changed,
            CancellationToken token = default)
        {
            if (changed is CallbackEventHandler handler)
            {
                return Observable.FromEvent<EventCallback<ChangeEvent<T>>, T>(
                    convert => evt => convert(evt.newValue),
                    add => handler.RegisterCallback(add),
                    remove => handler.UnregisterCallback(remove),
                    token
                );
            }

            throw new ArgumentException($"{changed} does not implement CallbackEventHandler.");
        }
    }
}
