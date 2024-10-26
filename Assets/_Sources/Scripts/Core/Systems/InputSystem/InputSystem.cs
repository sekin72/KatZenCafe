using System;
using System.Threading;
using CerberusFramework.Core;
using CFGameClient.Core;
using CFGameClient.Core.Events;
using Cysharp.Threading.Tasks;
using GameClient.Core.Events;
using MessagePipe;
using UnityEngine;

namespace GameClient.Core.Systems.InputSystem
{
    [CreateAssetMenu(fileName = "InputSystem", menuName = "GameClient/Systems/InputSystem", order = 4)]
    public class InputSystem : GameSystem, IInputSystem
    {
        public override Type RegisterType => typeof(IInputSystem);
        private IDisposable _messageSubscription;
        private IPublisher<MoveTileEvent> _moveTileContainerEventPublisher;

        public override async UniTask Initialize(GameSessionBase gameSessionBase, CancellationToken cancellationToken)
        {
            await base.Initialize(gameSessionBase, cancellationToken);

            var bagBuilder = DisposableBag.CreateBuilder();
            GlobalMessagePipe.GetSubscriber<InputTakenEvent>().Subscribe(OnInputTakenEvent).AddTo(bagBuilder);
            _messageSubscription = bagBuilder.Build();

            _moveTileContainerEventPublisher = GlobalMessagePipe.GetPublisher<MoveTileEvent>();
        }

        public override void Activate()
        {
        }

        public override void Deactivate()
        {
            _messageSubscription?.Dispose();
        }

        public override void Dispose()
        {
        }

        private void OnInputTakenEvent(InputTakenEvent evt)
        {
            _moveTileContainerEventPublisher.Publish(new MoveTileEvent(evt.Container, evt.Tile));

        }
    }
}
