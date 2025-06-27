using UnityEngine;
using UnityEngine.UIElements;
using R3;
using PinballBenki.Scene;

namespace PinballBenki.Game
{
    public class GameGUIManager : MonoBehaviour
    {
        [SerializeField] private UIDocument _uiDocument;

        public void Init()
        {
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
