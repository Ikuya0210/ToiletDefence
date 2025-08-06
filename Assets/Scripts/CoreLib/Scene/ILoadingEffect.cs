using System.Threading;
using Cysharp.Threading.Tasks;
using System;

namespace GGGameOver
{
    public interface IloadingEffect : IShareable
    {
        Type IShareable.ShareType => typeof(IloadingEffect);
        UniTask Show(CancellationToken ct);
        UniTask Hide(CancellationToken ct);
    }
}
