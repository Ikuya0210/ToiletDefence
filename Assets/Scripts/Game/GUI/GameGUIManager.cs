using UnityEngine;
using UnityEngine.UIElements;
using R3;
using Cysharp.Threading.Tasks;
using GGGameOver.GUI;

namespace GGGameOver.Toilet.Game
{
    public class GameGUIManager : MonoBehaviour
    {
        [SerializeField] private UIDocument _uiDocument;
        private Label _pointLabel;
        private Label _timerLabel;

        public async void Init()
        {
            await UniTask.WaitUntil(() => _uiDocument.rootVisualElement != null, cancellationToken: destroyCancellationToken);
            var root = _uiDocument.rootVisualElement;
            var escapeButton = root.Q<Button>("EscapeButton");
            _pointLabel = root.Q<Label>("PointLabel");
            _timerLabel = root.Q<Label>("TimerLabel");
            var popup = Shareables.Get<Popup>();
            escapeButton.OnClickAsObservable()
                .Subscribe(_ =>
                {
                    if (popup.IsActive)
                    {
                        return;
                    }
                    popup.ShowAsync(
                        "ゲームを終了しますか？",
                        () => SceneChanger.ChangeScene(SceneNames.ADV),
                        null,
                        destroyCancellationToken)
                        .Forget();
                })
                .AddTo(this);
        }

        public async void UpdatePoint(int point)
        {
            await UniTask.WaitUntil(() => _pointLabel != null, cancellationToken: destroyCancellationToken);
            _pointLabel.text = $"ポイント: {point}";
        }

        public void ShowGameOver()
        {
            Debug.Log("Game Over");
        }
    }
}
