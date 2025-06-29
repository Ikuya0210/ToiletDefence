using System.Threading;
using Cysharp.Threading.Tasks;

namespace GGGameOver.Toilet.ADV
{
    public interface ITalkable
    {
        UniTask TalkAsync(Character character, CancellationToken ct);
    }
}
