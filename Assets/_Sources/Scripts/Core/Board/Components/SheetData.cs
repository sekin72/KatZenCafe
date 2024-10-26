using CerberusFramework.Core.MVC;

namespace GameClient.Core.Board.Components
{
    public class SheetData : Data
    {
        public int BoardID { get; private set; }
        public int ZIndex { get; private set; }

        public int Width { get; private set; }
        public int Height { get; private set; }
        public Container[,] Containers { get; private set; }

        public SheetData(int boardID, int zIndex, int width, int height) : base()
        {
            BoardID = boardID;
            ZIndex = zIndex;
            Width = width;
            Height = height;

            Containers = new Container[Width, Height];
        }
    }
}
