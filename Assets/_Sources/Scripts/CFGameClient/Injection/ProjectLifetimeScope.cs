using CerberusFramework.Injection;
using CerberusFramework.Managers.Pool;
using CerberusFramework.Managers.Sound;
using CerberusFramework.UI.Popups;
using CFGameClient.Core.Events;
using CFGameClient.Managers.Data;
using GameClient.Core.Events;
using GameClient.Managers.LevelManager;
using MessagePipe;
using VContainer;

namespace CFGameClient.Injection
{
    public class ProjectLifetimeScope : RootLifetimeScope
    {
        protected override void ProjectConfiguration(IContainerBuilder builder, MessagePipeOptions messagePipeOptions)
        {
            base.ProjectConfiguration(builder, messagePipeOptions);

            CFPoolKeys.CombineTypes(GamePoolKeys.GetEnumerable());
            CFSoundTypes.CombineTypes(GameSoundKeys.GetEnumerable());
            CFPopupTypes.CombineTypes(GamePopupKeys.GetEnumerable());

            RegisterMessagePipe(builder, messagePipeOptions);
            RegisterManagers(builder);
        }

        private static void RegisterMessagePipe(IContainerBuilder builder, MessagePipeOptions options)
        {
            builder.RegisterMessageBroker<AttachParticleEvent>(options);
            builder.RegisterMessageBroker<DetachParticleEvent>(options);
            builder.RegisterMessageBroker<InputTakenEvent>(options); 
            builder.RegisterMessageBroker<MoveTileEvent>(options);
            builder.RegisterMessageBroker<ContainerCreatedEvent>(options);
        }

        private static void RegisterManagers(IContainerBuilder builder)
        {
            builder.Register<ProjectDataManager>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
            builder.Register<LevelManager>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
        }
    }
}
