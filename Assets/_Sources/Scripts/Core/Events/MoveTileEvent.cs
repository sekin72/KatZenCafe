using GameClient.Core.Board.Components;

namespace GameClient.Core.Events
{
    public struct MoveTileEvent
    {
        public Container FromContainer;
        public Tile Tile;

        public MoveTileEvent(Container fromContainer, Tile tile)
        {
            FromContainer = fromContainer;
            Tile = tile;
        }
    }
}
