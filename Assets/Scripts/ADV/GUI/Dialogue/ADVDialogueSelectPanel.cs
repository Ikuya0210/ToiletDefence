using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using R3;
using R3.Triggers;

namespace PinballBenki.ADV
{
    public class ADVDialogueSelectPanel : MonoBehaviour
    {
        [SerializeField] private ObservablePointerDownTrigger _yesButton;
        [SerializeField] private ObservablePointerDownTrigger _noButton;
        [SerializeField] private TMP_Text _yesText;
        [SerializeField] private TMP_Text _noText;
        [SerializeField] private CanvasGroup _canvasGroup;
        private int _selectNum = -1;


        public void Init()
        {
            _yesButton.OnPointerDownAsObservable()
                .Subscribe(_ =>
                {
                    if (_selectNum < 0)
                    {
                        _selectNum = 0;
                    }
                }).AddTo(this);
            _noButton.OnPointerDownAsObservable()
                .Subscribe(_ =>
                {
                    if (_selectNum < 0)
                    {
                        _selectNum = 1;
                    }
                }).AddTo(this);

            _yesText.text = string.Empty;
            _noText.text = string.Empty;
            _canvasGroup.SetEnable(false);
        }

        // TODO：　複数選択肢に対応
        public async UniTask<int> ShowAsync(string[] texts, CancellationToken ct)
        {
            Debug.Log("aaaa");
            _yesText.text = texts[0];
            _noText.text = texts[1];
            _canvasGroup.SetEnable(true);
            _selectNum = -1;
            await UniTask.Delay(300, cancellationToken: ct);
            await UniTask.WaitUntil(() => _selectNum != -1);
            _canvasGroup.SetEnable(false);
            await UniTask.Yield(cancellationToken: ct);
            return _selectNum;
        }
    }
}
