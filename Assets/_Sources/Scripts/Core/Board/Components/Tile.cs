using System.Threading;
using CerberusFramework.Core.MVC;
using CFGameClient.Core.Events;
using Cysharp.Threading.Tasks;
using MessagePipe;

namespace GameClient.Core.Board.Components
{
    public class Tile : Controller<TileView, TileData>
    {
        private IPublisher<InputTakenEvent> _inputTakenEventPublisher;

        public override void Initialize(CancellationToken cancellationToken)
        {
            base.Initialize(cancellationToken);

            _inputTakenEventPublisher = GlobalMessagePipe.GetPublisher<InputTakenEvent>();

            View.Selected += OnSelected;
            View.Released += OnReleased;

            Data.Despawned = false;
        }

        public override bool Dispose()
        {
            if (base.Dispose())
            {
                return true;
            }

            View.Selected -= OnSelected;
            View.Released -= OnReleased;

            Data.Container.RemoveTile();

            Data.Despawned = true;

            return false;
        }

        private void OnSelected()
        {
        }

        private void OnReleased()
        {
            _inputTakenEventPublisher.Publish(new InputTakenEvent(this, Data.Container));
        }

        public void SetSelectable(bool selectable)
        {
            View.SetSelectable(selectable);
        }

        public void InsertTileToContainer(Container container, bool forceMove = true)
        {
            Data.Container = container;

            Data.Container.InsertTile(this);
            View.SetUnderContainer(Data.Container.Data.Index, container.View, forceMove);
            View.SetTileState(Data.TileState);
            View.SetSprite(Data.Sprite);
        }

        public async UniTask MoveViewToLocalZero(CancellationToken cancellationToken)
        {
            await View.MoveViewToLocalZero(cancellationToken);
        }

        public void SetTileState(TileState tileState)
        {
            Data.SetTileState(tileState);
            View.SetTileState(Data.TileState);
        }
    }
}
