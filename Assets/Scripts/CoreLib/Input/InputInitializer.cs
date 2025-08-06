using R3;

namespace GGGameOver
{
    public sealed class InputInitializer : ShareableInitializer
    {
        public override IShareable Init()
        {
            var inputProvider = new InputProvider();
            inputProvider.AddTo(this);
            return inputProvider;
        }
    }
}
