using System.Threading;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
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
        [SerializeField] private ADVDialogueSelectPanel _selectPanel;
        private CancellationTokenSource _skipCts = new();

        public void Init()
        {
            _selectPanel.Init();
            Hide();
        }

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
        {
            _canvasGroup.SetEnable(true);
            IsVisible = true;
        }


        public void Hide()
        {
            _canvasGroup.SetEnable(false);
            IsVisible = false;
            _npcNameText.text = string.Empty;
            _dialogueText.text = string.Empty;

            if (_skipCts != null && !_skipCts.IsCancellationRequested)
            {
                _skipCts.Cancel();
                _skipCts.Dispose();
                _skipCts = null;
            }
        }

        public async UniTask ShowAsync(CancellationToken ct)
        {
            if (IsVisible) return;
            await _canvasGroup.SetEnableAsync(true, 0.3f, ct);
            IsVisible = true;
        }

        public async UniTask HideAsync(CancellationToken ct)
        {
            if (!IsVisible) return;
            await _canvasGroup.SetEnableAsync(false, 0.3f, ct);
            IsVisible = false;
            _npcNameText.text = string.Empty;
            _dialogueText.text = string.Empty;

            if (_skipCts != null && !_skipCts.IsCancellationRequested)
            {
                _skipCts.Cancel();
                _skipCts.Dispose();
                _skipCts = null;
            }
        }

        public UniTask<int> SelectAsync(string[] texts, CancellationToken ct)
        {
            return _selectPanel.ShowAsync(texts, ct);
        }
    }
}
