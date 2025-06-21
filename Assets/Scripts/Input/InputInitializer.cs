using PinballBenki.Scene;

namespace PinballBenki.Input
{
    public sealed class InputInitializer : BaseShareableInitializer
    {
        public override IShareable Init()
        {
            var inputProvider = new InputProvider();
            return inputProvider;
        }
    }
}
