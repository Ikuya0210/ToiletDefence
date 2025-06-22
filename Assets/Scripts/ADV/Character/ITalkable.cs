using System.Threading;
using Cysharp.Threading.Tasks;

namespace PinballBenki.ADV
{
    public interface ITalkable
    {
        UniTask TalkAsync(Character character, CancellationToken ct);
    }
}
