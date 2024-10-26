using System;
using System.Threading;
using CerberusFramework.Core;
using CerberusFramework.Managers.Pool;
using CFGameClient.Core.Events;
using Cysharp.Threading.Tasks;
using MessagePipe;
using UnityEngine;
using VContainer;

namespace CFGameClient.Core.Systems.ExternalParticlesSystem
{
    [CreateAssetMenu(fileName = "ExternalParticlesSystem", menuName = "CerberusFramework/Systems/ExternalParticlesSystem", order = 3)]
    public class ExternalParticlesSystem : GameSystem, IExternalParticlesSystem
    {
        private IDisposable _messageSubscription;
        private ExternalParticlesSystemView _view;
        public override Type RegisterType => typeof(IExternalParticlesSystem);

        private PoolManager _poolManager;

        [Inject]
        private void Inject(PoolManager poolManager)
        {
            _poolManager = poolManager;
        }

        public override async UniTask Initialize(GameSessionBase gameSessionBase, CancellationToken cancellationToken)
        {
            await base.Initialize(gameSessionBase, cancellationToken);

            var bagBuilder = DisposableBag.CreateBuilder();
            GlobalMessagePipe.GetSubscriber<AttachParticleEvent>().Subscribe(OnAttachParticleEvent).AddTo(bagBuilder);
            _messageSubscription = bagBuilder.Build();

            _view = _poolManager.GetGameObject(CFPoolKeys.FromIdOrName(nameof(ExternalParticlesSystemView))).GetComponent<ExternalParticlesSystemView>();
            _view.Initialize();
        }

        public override void Activate()
        {
        }

        public override void Deactivate()
        {
        }

        public override void Dispose()
        {
            _messageSubscription?.Dispose();
            _view.Dispose();
            _poolManager.SafeReleaseObject(CFPoolKeys.FromIdOrName(nameof(ExternalParticlesSystemView)), _view.gameObject);
        }

        private void OnAttachParticleEvent(AttachParticleEvent evt)
        {
            _view.AttachGameObject(evt.PoolKey, evt.ParticleGameObject);
        }
    }
}
