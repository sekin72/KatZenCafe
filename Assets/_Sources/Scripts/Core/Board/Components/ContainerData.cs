using CerberusFramework.Core.MVC;
using UnityEngine;

namespace GameClient.Core.Board.Components
{
    public class ContainerData : Data
    {
        public int BoardID { get; private set; }
        public Vector3Int Index { get; private set; }

        public Tile Tile { get; set; }

        public ContainerData(int boardID, Vector3Int index) : base()
        {
            BoardID = boardID;
            Index = index;
            Tile = null;
        }
    }
}
