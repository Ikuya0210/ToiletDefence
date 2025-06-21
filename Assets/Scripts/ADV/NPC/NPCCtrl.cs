using UnityEngine;

namespace PinballBenki.ADV
{
    public class NPCCtrl : MonoBehaviour
    {
        [SerializeField] private Character _character;

        public void Init()
        {
            _character.Init();
        }
    }
}
