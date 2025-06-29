using System;
using Cysharp.Threading.Tasks;
using PinballBenki.Scene;
using UnityEngine;
using UnityEngine.UIElements;
using R3;
using System.Threading;

namespace PinballBenki.GUI
{
    public sealed class Popup : ShareableInitializer, IShareable
    {
        public Type ShareType => typeof(Popup);
        public bool IsActive { get; private set; }

        [SerializeField] private UIDocument _uiDocument;
        private VisualElement _root;
        private Button _yesButton;
        private Button _noButton;
        private Label _messageLabel;

        private CompositeDisposable _disposables;


        public override IShareable Init()
        {
            _uiDocument.enabled = true;
            UniTask.WaitUntil(() => _uiDocument.rootVisualElement != null)
                .ContinueWith(() =>
                {
                    _root = _uiDocument.rootVisualElement;
                    _yesButton = _root.Q<Button>("YesButton");
                    _noButton = _root.Q<Button>("NoButton");
                    _messageLabel = _root.Q<Label>("MessageLabel");
                    _root.SetEnable(false);
                });
            return this;
        }

        public void Show(string message, Action onYes, Action onNo)
        {
            IsActive = true;
            _messageLabel.text = message;
            BindButtons(onYes, onNo, () => Hide());
            _root.SetEnable(true);
        }

        public void Hide()
        {
            if (_disposables != null)
            {
                _disposables.Dispose();
                _disposables = null;
            }
            _root.SetEnable(false);
            _messageLabel.text = string.Empty;
            IsActive = false;
        }

        public async UniTask ShowAsync(string message, Action onYes, Action onNo, CancellationToken ct)
        {
            IsActive = true;
            _messageLabel.text = message;
            await _root.SetEnableAsync(true, 0.3f, ct);
            bool isShown = true;
            BindButtons(onYes, onNo, () => isShown = false);
            await UniTask.WaitUntil(() => !isShown, cancellationToken: ct);
            await HideAsync(ct);
        }

        public async UniTask HideAsync(CancellationToken ct)
        {
            if (_disposables != null)
            {
                _disposables.Dispose();
                _disposables = null;
            }

            await _root.SetEnableAsync(false, 0.3f, ct);
            _messageLabel.text = string.Empty;
            IsActive = false;
        }

        private void BindButtons(Action onYes, Action onNo, Action hide)
        {
            if (_disposables != null)
            {
                _disposables.Dispose();
            }
            _disposables = new();
            _yesButton.OnClickAsObservable()
                .Subscribe(_ =>
                {
                    onYes?.Invoke();
                    hide?.Invoke();
                })
                .AddTo(_disposables);
            _noButton.OnClickAsObservable()
                .Subscribe(_ =>
                {
                    onNo?.Invoke();
                    hide?.Invoke();
                })
                .AddTo(_disposables);
        }
    }
}
