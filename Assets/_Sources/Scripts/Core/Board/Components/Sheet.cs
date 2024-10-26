using System.Threading;
using CerberusFramework.Core.MVC;
using CFGameClient;
using CFGameClient.Core;
using CFGameClient.Core.Systems.ViewSpawnerSystem;
using UnityEngine;
using VContainer;

namespace GameClient.Core.Board.Components
{
    public class Sheet : Controller<SheetView, SheetData>
    {
        private GameSession _session { get; set; }
        protected IObjectResolver _objectResolver;
        private IViewSpawnerSystem _viewSpawnerSystem;

        private CancellationTokenSource _cancellationTokenSource;

        [Inject]
        public void Inject(GameSession session, IObjectResolver objectResolver)
        {
            _session = session;
            _objectResolver = objectResolver;
            _viewSpawnerSystem = _session.GetSystem<IViewSpawnerSystem>();
        }

        public override void Initialize(CancellationToken cancellationToken)
        {
            base.Initialize(cancellationToken);

            _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            for (var x = 0; x < Data.Width; x++)
            {
                for (var y = 0; y < Data.Height; y++)
                {
                    CreateContainer(new Vector3Int(x, y, Data.ZIndex));
                }
            }
        }

        public override bool Dispose()
        {
            if (base.Dispose())
            {
                return true;
            }

            for (var x = 0; x < Data.Width; x++)
            {
                for (var z = 0; z < Data.Height; z++)
                {
                    Data.Containers[x, z]?.Dispose();
                    _viewSpawnerSystem.Despawn(GamePoolKeys.Container, Data.Containers[x, z].View);
                }
            }

            return false;
        }

        private void CreateContainer(Vector3Int index)
        {
            var x = index.x;
            var y = index.y;
            var tileSize = _session.GameSettings.TileSize;

            var containerData = new ContainerData(
                Data.BoardID,
                index);

            var containerView = _viewSpawnerSystem.Spawn<ContainerView>(GamePoolKeys.Container);
            var container = new Container();
            _objectResolver.Inject(container);
            container.SetDataAndView(containerData, containerView);
            container.Initialize(_cancellationTokenSource.Token);
            container.Activate();

            Data.Containers[x, y] = container;

            containerView.SetParent(View.transform, containerData.Index, new Vector3(x * tileSize.x, y * tileSize.y, 0));
        }

        public Container GetContainer(Vector3Int index)
        {
            if (index.z != Data.ZIndex || index.x >= Data.Width || index.y >= Data.Height)
            {
                return null;
            }

            return Data.Containers[index.x, index.y];
        }
    }

    public static class SheetExtensions
    {
    }
}
