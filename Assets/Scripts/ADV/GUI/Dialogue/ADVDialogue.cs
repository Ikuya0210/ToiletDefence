using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace PinballBenki.ADV
{
    public class ADVDialogue : MonoBehaviour, IADVDialogue
    {
        public bool IsVisible { get; private set; }
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private TMP_Text _npcNameText;
        [SerializeField] private TMP_Text _dialogueText;
        private CancellationTokenSource _skipCts = new();

        public void Skip()
        {
            if (_skipCts != null && !_skipCts.IsCancellationRequested)
            {
                _skipCts.Cancel();
            }
        }

        public async UniTask SetTextAsync(string npcName, string text, CancellationToken ct)
        {
            _skipCts = new();

            if (_npcNameText != null)
            {
                _npcNameText.text = npcName;
            }
            else
            {
                _npcNameText.text = string.Empty;
            }

            var linkCt = CancellationTokenSource.CreateLinkedTokenSource(ct, _skipCts.Token).Token;
            bool isCancel = await _dialogueText.SetTextAsync(text, linkCt)
                .SuppressCancellationThrow();

            // skippによるキャンセル以外は失敗
            if (isCancel && !_skipCts.IsCancellationRequested)
            {
                ct.ThrowIfCancellationRequested();
            }
        }

        public void Show()
            => _canvasGroup.SetEnable(true);

        public void Hide()
            => _canvasGroup.SetEnable(false);

        public UniTask ShowAsync(CancellationToken ct)
            => _canvasGroup.SetEnableAsync(true, 0.3f, ct);

        public UniTask HideAsync(CancellationToken ct)
            => _canvasGroup.SetEnableAsync(false, 0.3f, ct);
    }
}
