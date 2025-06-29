using UnityEngine;
using UnityEngine.UIElements;
using R3;
using GGGameOver;
using Cysharp.Threading.Tasks;
using GGGameOver.GUI;

namespace PinballBenki.Game
{
    public class GameGUIManager : MonoBehaviour
    {
        [SerializeField] private UIDocument _uiDocument;

        public async void Init()
        {
            await UniTask.WaitUntil(() => _uiDocument.rootVisualElement != null, cancellationToken: destroyCancellationToken);
            var root = _uiDocument.rootVisualElement;
            var escapeButton = root.Q<Button>("EscapeButton");
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
