using PinballBenki.Input;
using UnityEngine;
using R3;

namespace PinballBenki.Game
{
    internal sealed class Flippers : MonoBehaviour
    {
        [SerializeField] private Flipper _l;
        [SerializeField] private Flipper _r;
        private InputProvider _input;


        internal void Init()
        {
            _input = new();
            InitFlipper(_l);
            InitFlipper(_r);
            _input.AddTo(this);
        }

        private void InitFlipper(Flipper flipper)
        {
            if (flipper == null)
            {
                return;
            }
            flipper.Init();
            _input.OnAttack += () => flipper.Flip();
        }
    }
}
