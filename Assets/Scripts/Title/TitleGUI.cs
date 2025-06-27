using UnityEngine;
using UnityEngine.UIElements;
using R3;
using PinballBenki.Scene;
using Cysharp.Threading.Tasks;

namespace PinballBenki.Title
{
    public class TitleGUI : MonoBehaviour
    {
        [SerializeField] private UIDocument _uiDocument;

        public async void Init()
        {
            // 待たないと壊れる
            await UniTask.WaitUntil(() => _uiDocument.rootVisualElement != null, cancellationToken: destroyCancellationToken);
            var root = _uiDocument.rootVisualElement;
            var startButton = root.Q<Button>("StartButton");
            startButton.OnClickAsObservable()
                .Subscribe(_ =>
                {
                    SceneChanger.ChangeScene(SceneNames.ADV);
                })
                .AddTo(this);
        }
    }
}