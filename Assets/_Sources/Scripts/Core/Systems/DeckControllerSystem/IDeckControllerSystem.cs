using CerberusFramework.Core.Systems;

namespace GameClient.Core.Systems.DeckControllerSystem
{
    public interface IDeckControllerSystem : IGameSystem
    {
        void Undo();
        void OpenUpAdditionalSpace();
    }
}
