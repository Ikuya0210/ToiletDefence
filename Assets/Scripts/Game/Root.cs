using UnityEngine;

namespace PinballBenki.Game
{
    internal sealed class Root : MonoBehaviour
    {
        [SerializeField] private Flippers _flippers;

        private void Start()
        {
            _flippers.Init();
        }
    }
}
