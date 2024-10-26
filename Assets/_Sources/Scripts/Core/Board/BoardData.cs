using CerberusFramework.Core.MVC;
using GameClient.Core.Board.Components;
using GameClient.GameData;

namespace GameClient.Core.Board
{
    public class BoardData : Data
    {
        public BoardDesignData BoardDesignData { get; private set; }
        public BoardType BoardType { get; set; }
        public int BoardID { get; private set; }
        public int Elevation { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }
        public Sheet[] Sheets { get; private set; }

        public BoardData(int boardID, int elevation, int width, int height, BoardDesignData boardDesignData) : base()
        {
            BoardID = boardID;
            Elevation = elevation;
            Width = width;
            Height = height;

            Sheets = new Sheet[Elevation];
            BoardDesignData = boardDesignData;
        }
    }
}
