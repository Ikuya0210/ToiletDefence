using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

namespace GGGameOver
{
    public class LoadingPanel : ShareableInitializer, IShareable, IloadingEffect
    {
        Type IShareable.ShareType => typeof(IloadingEffect);
        [SerializeField] private UIDocument _uiDocument;
        private VisualElement _root;

        public async UniTask Hide(CancellationToken ct)
        {
            await _root.SetEnableAsync(false, 0.5f, ct);
        }

        public override IShareable Init()
        {
            _uiDocument.enabled = true;
            _root = _uiDocument.rootVisualElement;
            _root.SetEnable(false);
            return this;
        }

        public async UniTask Show(CancellationToken ct)
        {
            await _root.SetEnableAsync(true, 0.5f, ct);
        }
    }
}
