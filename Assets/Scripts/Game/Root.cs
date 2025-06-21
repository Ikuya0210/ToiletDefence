using System.Threading;
using Cysharp.Threading.Tasks;
using PinballBenki.Communicate;
using UnityEngine;
using R3;

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
