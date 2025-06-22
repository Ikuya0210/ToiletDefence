using UnityEngine;

namespace PinballBenki.ADV
{
    public class ADVGUICtrl : MonoBehaviour, IADVGUI
    {
        [SerializeField] private ADVDialogue _dialogue;

        public IADVDialogue Dialogue => _dialogue;
        public IADVGUI Init()
        {
            return this;
        }
    }
}
