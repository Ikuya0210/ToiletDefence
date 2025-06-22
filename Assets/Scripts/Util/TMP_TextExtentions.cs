using Cysharp.Threading.Tasks;
using UnityEngine;
using TMPro;
using System.Threading;

namespace PinballBenki
{
    public static class TMP_TextExtentions
    {
        /// <summary>
        /// 一文字ずつ表示する拡張 (遅延時間0.05秒)　キャンセルで全文表示
        /// </summary>
        public static UniTask SetTextAsync(this TMP_Text textComponent, string text, CancellationToken ct)
            => textComponent.SetTextAsync(text, 0.05f, ct);


        /// <summary>
        /// 一文字ずつ表示する拡張 (遅延時間指定) キャンセルで全文表示
        /// </summary>
        public static async UniTask SetTextAsync(this TMP_Text textComponent, string text, float delay, CancellationToken ct)
        {
            textComponent.text = string.Empty;

            await SetTextInternal(text, (int)(delay * 1000), ct)
                .SuppressCancellationThrow()
                .ContinueWith(isCancel =>
                {
                    if (isCancel)
                    {
                        textComponent.text = text;
                    }
                });

            async UniTask SetTextInternal(string text, int delayMS, CancellationToken ct)
            {
                foreach (char c in text)
                {
                    textComponent.text += c;
                    await UniTask.Delay(delayMS, cancellationToken: ct);
                }
            }
        }
    }
}
