using System.Collections.Generic;
using System.Threading;
using CFGameClient;
using GameClient.Core.Board.Components;
using UnityEngine;

namespace GameClient.Core.Board
{
    public class PyramidBoard : Board
    {
        public override void Initialize(CancellationToken cancellationToken)
        {
            base.Initialize(cancellationToken);

            Data.BoardType = BoardType.Pyramid;

            View.name = "Pyramid";

            var tileSize = _session.GameSettings.TileSize;

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

                Vector3 localPosition = (Vector3)(tileSize / 2f * (sheetData.ZIndex % 2)) + (Vector3.forward * (Data.Elevation - i - 1));
                sheetView.SetParent(View.transform, i, localPosition);
            }
        }

        public override Container GetRandomContainer()
        {
            Container container;
            do
            {
                var elevation = Random.Range(0, Data.Elevation);
                var x = Random.Range(0, Data.Width - elevation);
                var y = Random.Range(0, Data.Height - elevation);

                container = GetContainer(new Vector3Int(x, y, elevation));
            }
            while (!container.IsContainerEmpty());

            return container;
        }

        public override List<Vector3Int> GetLowerIndexes(Container container, int targetZ)
        {
            if (container.Data.Index.z % 2 == 0)
            {
                if (targetZ % 2 == 0)
                {
                    return _zeroPositionList;
                }

                return _leftDownList;
            }

            if (targetZ % 2 == 1)
            {
                return _zeroPositionList;
            }

            return _upRightList;
        }

        public override List<Vector3Int> GetUpperIndexes(Container container, int targetZ)
        {
            if (container.Data.Index.z % 2 == 0)
            {
                if (targetZ % 2 == 0)
                {
                    return _zeroPositionList;
                }

                return _leftDownList;
            }

            if (targetZ % 2 == 1)
            {
                return _zeroPositionList;
            }

            return _upRightList;
        }

        private static List<Vector3Int> _upRightList = new()
        {
            Vector3Int.up,
            Vector3Int.right,
            Vector3Int.right + Vector3Int.up,
            Vector3Int.zero
        };

        private static List<Vector3Int> _leftDownList = new()
        {
            Vector3Int.down,
            Vector3Int.left,
            Vector3Int.left + Vector3Int.down,
            Vector3Int.zero
        };
    }
}
