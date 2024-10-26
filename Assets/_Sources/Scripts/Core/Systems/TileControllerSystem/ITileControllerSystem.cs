using System.Collections.Generic;
using CerberusFramework.Core.Systems;
using GameClient.Core.Board.Components;

namespace GameClient.Core.Systems.TileControllerSystem
{
    public interface ITileControllerSystem : IGameSystem
    {
        void DespawnTile(Tile tile);
        List<Tile> AllTiles { get; }
    }
}
