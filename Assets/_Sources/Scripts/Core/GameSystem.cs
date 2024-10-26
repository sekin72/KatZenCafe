using System.Threading;
using CerberusFramework.Core;
using CerberusFramework.Core.Systems;
using Cysharp.Threading.Tasks;

namespace CFGameClient.Core
{
    public abstract class GameSystem : SystemBase
    {
        protected GameSession _session;

        public override UniTask Initialize(GameSessionBase gameSession, CancellationToken cancellationToken)
        {
            _session = (GameSession)gameSession;
            return base.Initialize(gameSession, cancellationToken);
        }
    }
}
