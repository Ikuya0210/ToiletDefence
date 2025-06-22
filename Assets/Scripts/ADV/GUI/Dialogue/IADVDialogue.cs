using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace PinballBenki.ADV
{
    public interface IADVDialogue
    {
        UniTask ShowAsync(CancellationToken ct);
        UniTask HideAsync(CancellationToken ct);
        UniTask SetTextAsync(string texts, CancellationToken ct);
    }
}
