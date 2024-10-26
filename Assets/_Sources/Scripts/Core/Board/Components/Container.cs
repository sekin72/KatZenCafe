using System.Collections.Generic;
using System.Threading;
using CerberusFramework.Core.MVC;
using CFGameClient.Core;
using GameClient.Core.Events;
using GameClient.Core.Systems.BoardControllerSystem;
using MessagePipe;
using UnityEngine;
using VContainer;

namespace GameClient.Core.Board.Components
{
    public class Container : Controller<ContainerView, ContainerData>
    {
        protected GameSession _session { get; private set; }

        private Board _board;

        private IPublisher<ContainerCreatedEvent> _containerCreatedEventPublisher;

        [Inject]
        public void Inject(GameSession session)
        {
            _session = session;
        }

        public override void Initialize(CancellationToken cancellationToken)
        {
            base.Initialize(cancellationToken);

            _board = _session.GetSystem<IBoardControllerSystem>().GetBoard(Data.BoardID);

            _containerCreatedEventPublisher = GlobalMessagePipe.GetPublisher<ContainerCreatedEvent>();
            _containerCreatedEventPublisher.Publish(new ContainerCreatedEvent(this));
        }

        public void InsertTile(Tile tile)
        {
            Data.Tile = tile;
        }

        public void RemoveTile()
        {
            Data.Tile = null;
        }

        public void ForceBelowIndexesToCheckTileState()
        {
            var z = 0;

            for (var i = Data.Index.z - 1; i >= 0; i--)
            {
                var lowerIndexes = _board.GetLowerIndexes(this, i);
                z++;

                for (var j = 0; j < lowerIndexes.Count; j++)
                {
                    var index = lowerIndexes[j];
                    var container = _board.GetContainer(Data.Index + index + (Vector3Int.back * z));

                    if (container.IsContainerNullOrEmpty())
                    {
                        continue;
                    }

                    container.CheckTileState();
                }
            }
        }

        public bool CheckTileState()
        {
            var z = 0;

            for (var i = Data.Index.z + 1; i < _board.Data.Elevation; i++)
            {
                var upperIndexes = _board.GetUpperIndexes(this, i);
                z++;

                for (var j = 0; j < upperIndexes.Count; j++)
                {
                    var index = upperIndexes[j];
                    var container = _board.GetContainer(Data.Index + index + (Vector3Int.forward * z));

                    if (container.IsContainerNullOrEmpty())
                    {
                        continue;
                    }

                    Data.Tile.SetTileState(TileState.Hidden);
                    return false;
                }
            }

            Data.Tile.SetTileState(TileState.Shown);
            return true;
        }
    }

    public static class ContainerExtensions
    {
        public static bool IsContainerNullOrEmpty(this Container container)
        {
            return container == null || container.Data.Tile == null;
        }

        public static bool IsContainerEmpty(this Container container)
        {
            return container.Data.Tile == null;
        }

        public static bool IsContainerNull(this Container container)
        {
            return container == null;
        }
    }
}
