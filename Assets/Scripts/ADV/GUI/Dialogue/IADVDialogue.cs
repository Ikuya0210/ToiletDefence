using System.Threading;
using Cysharp.Threading.Tasks;

namespace GGGameOver.Toilet.ADV
{
    public interface IADVDialogue
    {
        bool IsVisible { get; }
        void Skip();
        void Show();
        void Hide();
        UniTask<int> SelectAsync(string[] texts, CancellationToken ct);
        UniTask ShowAsync(CancellationToken ct);
        UniTask HideAsync(CancellationToken ct);
        UniTask SetTextAsync(string talkerName, string text, CancellationToken ct);
    }
}
