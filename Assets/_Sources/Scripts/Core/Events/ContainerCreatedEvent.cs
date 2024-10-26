using GameClient.Core.Board.Components;

namespace GameClient.Core.Events
{
    public struct ContainerCreatedEvent
    {
        public Container Container;

        public ContainerCreatedEvent(Container container)
        {
            Container = container;
        }
    }
}
