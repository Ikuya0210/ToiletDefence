using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

namespace GGGameOver.Toilet.ADV
{
    public class ADVDialogue : MonoBehaviour, IADVDialogue
    {
        public bool IsVisible { get; private set; }
        [SerializeField] private UIDocument _uIDocument;
        private CancellationTokenSource _skipCts = new();
        private VisualElement _root;
        private VisualElement _selectPanel;
        private Button _selectYesButton;
        private Button _selectNoButton;
        private Label _npcNameText;
        private Label _mainText;

        public async void Init()
        {
            await UniTask.WaitUntil(() => _uIDocument.rootVisualElement != null, cancellationToken: this.GetCancellationTokenOnDestroy());
            _root = _uIDocument.rootVisualElement;
            _selectPanel = _uIDocument.rootVisualElement.Q<VisualElement>("SelectBox");
            _selectYesButton = _uIDocument.rootVisualElement.Q<Button>("SelectYesButton");
            _selectNoButton = _uIDocument.rootVisualElement.Q<Button>("SelectNoButton");
            _npcNameText = _uIDocument.rootVisualElement.Q<Label>("NameText");
            _mainText = _uIDocument.rootVisualElement.Q<Label>("MainText");
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
            bool isCancel = await _mainText.SetTextAsync(text, linkCt)
                .SuppressCancellationThrow();

            // skippによるキャンセル以外は失敗
            if (isCancel && !_skipCts.IsCancellationRequested)
            {
                ct.ThrowIfCancellationRequested();
            }
        }

        public void Show()
        {
            _root.SetEnable(true);
            IsVisible = true;
        }


        public void Hide()
        {
            _root.SetEnable(false);
            _selectPanel.SetEnable(false);
            _selectYesButton.text = string.Empty;
            _selectNoButton.text = string.Empty;
            IsVisible = false;
            _npcNameText.text = string.Empty;
            _mainText.text = string.Empty;

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
            await _root.SetEnableAsync(true, 0.3f, ct);
            IsVisible = true;
        }

        public async UniTask HideAsync(CancellationToken ct)
        {
            if (!IsVisible) return;
            await _root.SetEnableAsync(false, 0.3f, ct);
            IsVisible = false;
            _npcNameText.text = string.Empty;
            _mainText.text = string.Empty;

            if (_skipCts != null && !_skipCts.IsCancellationRequested)
            {
                _skipCts.Cancel();
                _skipCts.Dispose();
                _skipCts = null;
            }
        }

        public async UniTask<int> SelectAsync(string[] texts, CancellationToken ct)
        {
            _selectYesButton.text = texts[0];
            _selectNoButton.text = texts[1];

            int selectNum = -1;
            void ChangeNum(int num)
            {
                if (selectNum < 0)
                {
                    selectNum = num;
                }
            }
            await _selectPanel.SetEnableAsync(true, 0.2f, ct);
            await UniTask.Delay(300, cancellationToken: ct);
            _selectYesButton.clicked += () => ChangeNum(0);
            _selectNoButton.clicked += () => ChangeNum(1);
            await UniTask.WaitUntil(() => selectNum != -1);
            _selectYesButton.clicked -= () => ChangeNum(0);
            _selectNoButton.clicked -= () => ChangeNum(1);
            await _selectPanel.SetEnableAsync(false, 0.2f, ct);
            await UniTask.Yield(cancellationToken: ct);
            return selectNum;
        }
    }
}
