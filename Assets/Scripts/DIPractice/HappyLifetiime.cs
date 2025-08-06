using VContainer;
using VContainer.Unity;

namespace DIPractice
{
    public class HappyLifetiime : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<HelloWorldService>(Lifetime.Singleton);
        }
    }
}
