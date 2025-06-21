using System.Threading;
using Cysharp.Threading.Tasks;

namespace PinballBenki.Scene
{
    internal sealed class BootRoot : SceneRoot
    {
        protected override SceneNames SceneName => SceneNames.None;

        protected override UniTask Init(CancellationToken ct)
        {
            ChangeScene(SceneNames.Title);
            return UniTask.CompletedTask;
        }
    }
}
