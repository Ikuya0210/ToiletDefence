using PinballBenki.Scene;
using R3;

namespace PinballBenki.Input
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
