using CerberusFramework.Core.MVC;
using UnityEngine;

namespace GameClient.Core.Board.Components
{
    public class TileData : Data
    {
        public Container Container { get; set; }
        public TileState TileState { get; private set; }

        public Sprite Sprite;

        public bool Despawned { get; set; }
        public bool Decked { get; set; }

        public TileData(Sprite sprite) : base()
        {
            Container = null;
            TileState = TileState.Shown;
            Sprite = sprite;

            Despawned = false;
            Decked = false;
        }

        public void SetTileState(TileState tileState)
        {
            TileState = tileState;
        }
    }
}
