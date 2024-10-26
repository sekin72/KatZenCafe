using System.Collections.Generic;
using System.Threading;
using CFGameClient;
using GameClient.Core.Board.Components;
using UnityEngine;

namespace GameClient.Core.Board
{
    public class DeckBoard : Board
    {
        public override void Initialize(CancellationToken cancellationToken)
        {
            base.Initialize(cancellationToken);

            Data.BoardType = BoardType.Deck;

            View.name = "Deck";

            var sheetData = new SheetData(Data.BoardID, Data.Elevation - 1, Data.Width, Data.Height);
            var sheetView = _viewSpawnerSystem.Spawn<SheetView>(GamePoolKeys.Sheet);
            var sheet = new Sheet();
            _objectResolver.Inject(sheet);
            sheet.SetDataAndView(sheetData, sheetView);
            sheet.Initialize(cancellationToken);
            sheet.Activate();

            Data.Sheets[0] = sheet;

            sheetView.SetParent(View.transform, 0, Vector3.zero);
        }

        public Container GetNextEmptyContainer()
        {
            for (var i = 0; i < Data.Width; i++)
            {
                Container container = Data.Sheets[0].GetContainer(Vector3Int.right * i);
                if (container.IsContainerEmpty())
                {
                    return container;
                }
            }

            return null;
        }

        public override Container GetRandomContainer()
        {
            return null;
        }
    }
}
