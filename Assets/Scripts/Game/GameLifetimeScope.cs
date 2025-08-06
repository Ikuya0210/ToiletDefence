using VContainer;
using VContainer.Unity;

namespace GGGameOver.Toilet.Game
{
    internal sealed class GameLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<IDProvider>(Lifetime.Singleton);
            builder.Register<TargetJudge>(Lifetime.Singleton);
            builder.RegisterComponentInHierarchy<Root>();
            builder.RegisterComponentInHierarchy<CharacterCreater>();
            builder.RegisterComponentInHierarchy<CharacterManager>();
            builder.RegisterComponentInHierarchy<GameGUIManager>();
            builder.RegisterComponentInHierarchy<Tower>();
            builder.RegisterComponentInHierarchy<ScheduleExecuter>();
            builder.Register<PointManager>(Lifetime.Singleton);
            builder.Register<CharacterView>(Lifetime.Transient);
        }
    }
}
