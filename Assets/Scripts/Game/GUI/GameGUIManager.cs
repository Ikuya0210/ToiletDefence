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

        public async void Init()
        {
            await UniTask.WaitUntil(() => _uiDocument.rootVisualElement != null, cancellationToken: destroyCancellationToken);
            var root = _uiDocument.rootVisualElement;
            var escapeButton = root.Q<Button>("EscapeButton");
            var pointLabel = root.Q<Label>("PointLabel");
            var timerLabel = root.Q<Label>("TimerLabel");
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
    }
}
