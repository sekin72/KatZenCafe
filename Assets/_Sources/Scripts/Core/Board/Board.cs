using System.Collections.Generic;
using System.Threading;
using CerberusFramework.Core.MVC;
using CFGameClient;
using CFGameClient.Core;
using CFGameClient.Core.Systems.ViewSpawnerSystem;
using GameClient.Core.Board.Components;
using UnityEngine;
using VContainer;
using Container = GameClient.Core.Board.Components.Container;

namespace GameClient.Core.Board
{
    public abstract class Board : Controller<BoardView, BoardData>
    {
        protected GameSession _session { get; private set; }
        protected IObjectResolver _objectResolver;
        protected IViewSpawnerSystem _viewSpawnerSystem;

        [Inject]
        public void Inject(GameSession session, IObjectResolver objectResolver)
        {
            _session = session;
            _objectResolver = objectResolver;
            _viewSpawnerSystem = _session.GetSystem<IViewSpawnerSystem>();
        }

        public override bool Dispose()
        {
            if (base.Dispose())
            {
                return true;
            }

            for (var i = 0; i < Data.Elevation; i++)
            {
                Data.Sheets[i].Dispose();
                _viewSpawnerSystem.Despawn(GamePoolKeys.Sheet, Data.Sheets[i].View);
            }

            return false;
        }

        private Sheet GetSheet(int yIndex)
        {
            if (yIndex < 0 || yIndex >= Data.Elevation)
            {
                return null;
            }

            return Data.Sheets[yIndex];
        }

        public Container GetContainer(Vector3Int index)
        {
            if (index.z >= Data.Elevation || index.x < 0 || index.y < 0)
            {
                return null;
            }

            var sheet = GetSheet(index.z);

            if (sheet == null)
            {
                return null;
            }

            return sheet.GetContainer(index);
        }

        public abstract Container GetRandomContainer();
        public virtual List<Vector3Int> GetLowerIndexes(Container container, int targetZ)
        {
            return _zeroPositionList;
        }

        public virtual List<Vector3Int> GetUpperIndexes(Container container, int targetZ)
        {
            return _zeroPositionList;
        }

        protected static List<Vector3Int> _zeroPositionList = new() { Vector3Int.zero };
    }

    public enum BoardType
    {
        Pyramid = 0,
        Stack,
        Deck
    }

    public static class BoardExtensions
    {
        public static bool IsPyramidBoard(this Board board)
        {
            return board.Data.BoardType.IsPyramidBoard();
        }
    }

    public static class BoardTypeExtensions
    {
        public static bool IsPyramidBoard(this BoardType boardType)
        {
            return boardType == BoardType.Pyramid;
        }
    }
}
