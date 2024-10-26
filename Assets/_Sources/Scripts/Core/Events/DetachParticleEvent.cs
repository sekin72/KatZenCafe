using CerberusFramework.Managers.Pool;
using UnityEngine;

namespace CFGameClient.Core.Events
{
    public readonly struct DetachParticleEvent
    {
        public readonly CFPoolKeys PoolKey;
        public readonly GameObject ParticleGameObject;

        public DetachParticleEvent(CFPoolKeys poolKey, GameObject particleObject)
        {
            PoolKey = poolKey;
            ParticleGameObject = particleObject;
        }
    }
}
