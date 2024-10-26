using System.Collections.Generic;
using CerberusFramework.Managers.Pool;
using UnityEngine;

namespace CFGameClient.Core.Systems.ExternalParticlesSystem
{
    public class ExternalParticlesSystemView : MonoBehaviour
    {
        private Dictionary<CFPoolKeys, List<GameObject>> _particles = new();
        public void Initialize()
        {
        }

        public void Dispose()
        {
        }

        public void AttachGameObject(CFPoolKeys key, GameObject particleGameObject)
        {
            if (!_particles.ContainsKey(key))
            {
                _particles.Add(key, new List<GameObject>());
            }

            _particles[key].Add(particleGameObject);
            particleGameObject.transform.SetParent(transform);
        }
    }
}
