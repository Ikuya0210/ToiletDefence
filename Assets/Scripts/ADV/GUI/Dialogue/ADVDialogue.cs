using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace PinballBenki.ADV
{
    public class ADVDialogue : MonoBehaviour, IADVDialogue
    {
        public UniTask HideAsync(CancellationToken ct)
        {
            throw new System.NotImplementedException();
        }

        public UniTask SetTextAsync(string texts, CancellationToken ct)
        {
            throw new System.NotImplementedException();
        }

        public UniTask ShowAsync(CancellationToken ct)
        {
            throw new System.NotImplementedException();
        }
    }
}
