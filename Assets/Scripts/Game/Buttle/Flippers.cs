using PinballBenki.Input;
using UnityEngine;
using R3;

namespace PinballBenki.Game
{
    internal sealed class Flippers : MonoBehaviour
    {
        [SerializeField] private Flipper _l;
        [SerializeField] private Flipper _r;
        private PartialInput _input;

        internal void Init()
        {
            _input = new();
            _input.AddTo(this);
            _l.Init();
            _r.Init();
            _input.OnFlip_L.Subscribe(_ => _l.Flip()).AddTo(this);
            _input.OnFlip_R.Subscribe(_ => _r.Flip()).AddTo(this);
        }
    }
}
