using CerberusFramework.Core.MVC;
using CerberusFramework.Core.Systems;

namespace CFGameClient.Core.Systems.ViewSpawnerSystem
{
    public interface IViewSpawnerSystem : IGameSystem
    {
        public T Spawn<T>(GamePoolKeys poolKey) where T : View;
        public void Despawn<T>(GamePoolKeys poolKey, T t) where T : View;
    }
}
