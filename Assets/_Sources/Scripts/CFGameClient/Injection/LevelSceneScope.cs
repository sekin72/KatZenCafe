using CFGameClient.Core;
using CFGameClient.Core.Scenes;
using CFGameClient.UI.Popups.FailPopupVariant;
using CFGameClient.UI.Popups.PausePopup;
using CFGameClient.UI.Popups.WinPopupVariant;
using VContainer;
using VContainer.Unity;

namespace CFGameClient.Injection
{
    public class LevelSceneScope : LifetimeScope
    {
        public LevelSceneController LevelSceneController;
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponent(LevelSceneController);
            builder.Register<GameSession>(Lifetime.Scoped).AsImplementedInterfaces().AsSelf();
            RegisterUI(builder);
            base.Configure(builder);
        }

        private static void RegisterUI(IContainerBuilder builder)
        {
            builder.Register<WinPopupVariant>(Lifetime.Scoped).AsImplementedInterfaces().AsSelf();
            builder.Register<FailPopupVariant>(Lifetime.Scoped).AsImplementedInterfaces().AsSelf();
            builder.Register<PausePopup>(Lifetime.Scoped).AsImplementedInterfaces().AsSelf();
        }
    }
}
