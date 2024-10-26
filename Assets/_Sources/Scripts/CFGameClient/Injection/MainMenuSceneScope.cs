using CFGameClient.Core.Scenes;
using VContainer;
using VContainer.Unity;

namespace CFGameClient.Injection
{
    public sealed class MainMenuSceneScope : LifetimeScope
    {
        public MainSceneController MainSceneController;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponent(MainSceneController);
        }
    }
}
