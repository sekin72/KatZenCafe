using System;
using System.Collections.Generic;
using System.Threading;
using CerberusFramework.Core;
using CerberusFramework.Utilities.Extensions;
using CFGameClient.Core;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using GameClient.Core.Board.Components;
using GameClient.Core.Systems.DeckControllerSystem;
using GameClient.Core.Systems.TileControllerSystem;
using GameClient.UI;
using UnityEngine;

namespace GameClient.Core.Systems.BoosterSystem
{
    [CreateAssetMenu(fileName = "BoosterSystem", menuName = "GameClient/Systems/BoosterSystem", order = 7)]
    public class BoosterSystem : GameSystem, IBoosterSystem
    {
        public override Type RegisterType => typeof(IBoosterSystem);
        private IDeckControllerSystem _deckControllerSystem;
        private ITileControllerSystem _tileControllerSystem;

        private BoosterPanel _boosterPanel;

        private bool _boosterOngoing = false;

        private CancellationTokenSource _cancellationTokenSource;
        private CancellationToken _cancellationToken;

        public override async UniTask Initialize(GameSessionBase gameSessionBase, CancellationToken cancellationToken)
        {
            await base.Initialize(gameSessionBase, cancellationToken);

            _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            _cancellationToken = _cancellationTokenSource.Token;

            _deckControllerSystem = _session.GetSystem<IDeckControllerSystem>();
            _tileControllerSystem = _session.GetSystem<ITileControllerSystem>();

            _boosterOngoing = false;

            _boosterPanel = _session.BoosterPanel;

            _boosterPanel.UndoButtonClicked += OnUndoButtonClicked;
            _boosterPanel.ExtraSpaceButtonClicked += OnExtraSpaceButtonClicked;
            _boosterPanel.ShuffleButtonClicked += OnShuffleButtonClicked;

            _boosterPanel.Initialize();
        }

        public override void Activate()
        {
        }

        public override void Deactivate()
        {
            _boosterPanel.Dispose();

            _boosterPanel.UndoButtonClicked -= OnUndoButtonClicked;
            _boosterPanel.ExtraSpaceButtonClicked -= OnExtraSpaceButtonClicked;
            _boosterPanel.ShuffleButtonClicked -= OnShuffleButtonClicked;
        }

        public override void Dispose()
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
        }

        private void OnUndoButtonClicked()
        {
            if (_boosterOngoing)
            {
                return;
            }

            _deckControllerSystem.Undo();
        }

        private void OnExtraSpaceButtonClicked()
        {
            if (_boosterOngoing)
            {
                return;
            }

            _deckControllerSystem.OpenUpAdditionalSpace();
        }

        private void OnShuffleButtonClicked()
        {
            if (_boosterOngoing)
            {
                return;
            }

            Shuffle().Forget();
        }

        private async UniTask Shuffle()
        {
            _boosterOngoing = true;

            var tiles = _tileControllerSystem.AllTiles;
            List<Container> containers = new();

            var tasks = new List<UniTask>();
            var screenCenter = Vector3.right * _session.LevelDesignData.BoardDesigns[0].Width / 4f;
            for (var i = 0; i < tiles.Count; i++)
            {
                var tile = tiles[i];

                if (tile.Data.Despawned || tile.Data.Decked)
                {
                    tiles.RemoveAt(i--);
                    continue;
                }

                containers.Add(tile.Data.Container);
                tile.Data.Container.RemoveTile();
                tile.SetSelectable(false);

                tasks.Add(tile.View.transform.DOMove(screenCenter, .5f).ToUniTask(TweenCancelBehaviour.KillAndCancelAwait, _cancellationToken));
            }

            await UniTask.WhenAll(tasks);

            containers.Shuffle();
            tasks = new List<UniTask>();

            for (var i = 0; i < tiles.Count; i++)
            {
                var tile = tiles[i];

                tile.InsertTileToContainer(containers[i], false);
                tile.SetSelectable(false);

                tasks.Add(tile.MoveViewToLocalZero(_cancellationToken));
            }

            for (var i = 0; i < containers.Count; i++)
            {
                containers[i].CheckTileState();
            }

            await UniTask.WhenAll(tasks);

            for (var i = 0; i < tiles.Count; i++)
            {
                var tile = tiles[i];
                tile.SetSelectable(true);
                containers[i].CheckTileState();
            }

            _boosterOngoing = false;
        }
    }
}
