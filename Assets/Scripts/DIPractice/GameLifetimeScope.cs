using System;
using VContainer;
using VContainer.Unity;
using UnityEngine;

namespace DIPractice
{
    public class GameLifetimeScope : LifetimeScope
    {
        [SerializeField] private HelloCanvas helloCanvas;
        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<HelloWorldService>(Lifetime.Singleton);
            builder.RegisterEntryPoint<GamePresenter>();
            builder.RegisterComponent(helloCanvas).AsSelf();
        }
    }
}
