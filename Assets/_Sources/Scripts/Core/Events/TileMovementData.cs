using GameClient.Core.Board.Components;
namespace GameClient.Core.Events
{
    public struct TileMovementData
    {
        public Tile Tile;
        public Container FromContainer;

        public TileMovementData(Tile tile, Container fromContainer)
        {
            Tile = tile;
            FromContainer = fromContainer;
        }
    }
}
