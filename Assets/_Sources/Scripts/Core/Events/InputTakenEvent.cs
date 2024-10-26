using GameClient.Core.Board.Components;

namespace CFGameClient.Core.Events
{
    public struct InputTakenEvent
    {
        public Tile Tile;
        public Container Container;

        public InputTakenEvent(Tile tile, Container container)
        {
            Tile = tile;
            Container = container;
        }
    }
}