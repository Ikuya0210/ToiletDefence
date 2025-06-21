using PinballBenki.Input;
using UnityEngine;
using R3;

namespace PinballBenki.ADV
{
    public class PlayerCtrl : MonoBehaviour
    {
        [SerializeField] private Character _character;
        private PartialInput _input;

        public void Init()
        {
            _character.Init();
            _input = new();
            _input.AddTo(this);

            _input.OnDecide.Subscribe(_ => _character.Talk()).AddTo(this);
            _input.OnMove4.Subscribe(dir => _character.Move(dir)).AddTo(this);
        }
    }
}
