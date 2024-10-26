using System.Collections.Generic;
using System.Threading;
using CFGameClient;
using GameClient.Core.Board.Components;
using UnityEngine;

namespace GameClient.Core.Board
{
    public class StackBoard : Board
    {
        public override void Initialize(CancellationToken cancellationToken)
        {
            base.Initialize(cancellationToken);

            Data.BoardType = BoardType.Stack;

            View.name = "Stack";

            for (var i = 0; i < Data.Elevation; i++)
            {
                var sheetData = new SheetData(Data.BoardID, i, Data.Width, Data.Height);
                var sheetView = _viewSpawnerSystem.Spawn<SheetView>(GamePoolKeys.Sheet);
                var sheet = new Sheet();
                _objectResolver.Inject(sheet);
                sheet.SetDataAndView(sheetData, sheetView);
                sheet.Initialize(cancellationToken);
                sheet.Activate();

                Data.Sheets[i] = sheet;

                Vector3 localPosition = (Data.BoardDesignData.StackDirection * Data.BoardDesignData.StackDistanceBetweenTiles * sheetData.ZIndex) + (Vector3.forward * (Data.Elevation - i - 1));
                sheetView.SetParent(View.transform, i, localPosition);
            }
        }

        public override Container GetRandomContainer()
        {
            Container container;
            do
            {
                var x = Random.Range(0, Data.Width);
                var y = Random.Range(0, Data.Height);
                var elevation = Random.Range(0, Data.Elevation);

                container = GetContainer(new Vector3Int(x, y, elevation));
            }
            while (!container.IsContainerEmpty());

            return container;
        }
    }
}
