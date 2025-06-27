using UnityEngine;
using UnityEngine.UIElements;
using R3;
using PinballBenki.Scene;
using Cysharp.Threading.Tasks;

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
            escapeButton.OnClickAsObservable()
                .Subscribe(_ =>
                {
                    SceneChanger.ChangeScene(SceneNames.ADV);
                })
                .AddTo(this);
        }
    }
}
