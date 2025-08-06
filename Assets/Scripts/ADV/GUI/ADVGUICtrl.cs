using UnityEngine;

namespace GGGameOver.Toilet.ADV
{
    public class ADVGUICtrl : MonoBehaviour, IADVGUI
    {
        [SerializeField] private ADVDialogue _dialogue;

        public IADVDialogue Dialogue => _dialogue;
        public void Init()
        {
            _dialogue.Init();
        }
    }
}
