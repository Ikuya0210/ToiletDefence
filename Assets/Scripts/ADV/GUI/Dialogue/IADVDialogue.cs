using System.Threading;
using Cysharp.Threading.Tasks;

namespace PinballBenki.ADV
{
    public interface IADVDialogue
    {
        bool IsVisible { get; }
        void Skip();
        void Show();
        void Hide();
        UniTask ShowAsync(CancellationToken ct);
        UniTask HideAsync(CancellationToken ct);
        UniTask SetTextAsync(string talkerName, string text, CancellationToken ct);
    }
}
