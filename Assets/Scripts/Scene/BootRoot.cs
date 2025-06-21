using System.Threading;
using Cysharp.Threading.Tasks;

namespace PinballBenki.Scene
{
    internal sealed class BootRoot : SceneRoot
    {
        protected internal override SceneNames SceneName => SceneNames.None;

        protected internal override UniTask Init(CancellationToken ct)
        {
            ChangeScene(SceneNames.Title);
            return UniTask.CompletedTask;
        }
    }
}
