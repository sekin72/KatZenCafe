using System;
using System.Collections.Generic;
using System.Threading;
using CerberusFramework.Core;
using CerberusFramework.Managers.Sound;
using CerberusFramework.Managers.Vibration;
using CFGameClient.Core;
using Cysharp.Threading.Tasks;
using GameClient.Core.Board;
using GameClient.Core.Board.Components;
using GameClient.Core.Events;
using GameClient.Core.Systems.BoardControllerSystem;
using GameClient.Core.Systems.TileControllerSystem;
using MessagePipe;
using UnityEngine;
using VContainer;
using Container = GameClient.Core.Board.Components.Container;

namespace GameClient.Core.Systems.DeckControllerSystem
{
    [CreateAssetMenu(fileName = "DeckControllerSystem", menuName = "GameClient/Systems/DeckControllerSystem", order = 6)]
    public class DeckControllerSystem : GameSystem, IDeckControllerSystem
    {
        public override Type RegisterType => typeof(IDeckControllerSystem);
        private DeckBoard DeckBoard;

        private Stack<TileMovementData> _tileMovementData;
        private IDisposable _messageSubscription;
        private ITileControllerSystem _tileControllerSystem;
        private IBoardControllerSystem _boardControllerSystem;

        private Dictionary<Sprite, List<Tile>> _deckedTiles;
        private List<Sprite> _spritesOrder;

        private int _matchCount;
        private int _currentDeckCounter;

        private CancellationTokenSource _inputCancellationTokenSource;
        private CancellationToken _inputCancellationToken;

        private SoundManager _soundManager;
        private VibrationManager _vibrationManager;

        [Inject]
        public void Inject(SoundManager soundManager, VibrationManager vibrationManager)
        {
            _soundManager = soundManager;
            _vibrationManager = vibrationManager;
        }

        public override async UniTask Initialize(GameSessionBase gameSessionBase, CancellationToken cancellationToken)
        {
            await base.Initialize(gameSessionBase, cancellationToken);

            _currentDeckCounter = 0;
            _deckedTiles = new();
            _tileMovementData = new();
            _spritesOrder = new();

            _tileControllerSystem = _session.GetSystem<ITileControllerSystem>();
            _boardControllerSystem = _session.GetSystem<IBoardControllerSystem>();
            DeckBoard = (DeckBoard)_boardControllerSystem.GetBoard(_session.LevelDesignData.BoardDesigns.Count);
            _matchCount = _session.GameSettings.MatchCount;

            var bagBuilder = DisposableBag.CreateBuilder();
            GlobalMessagePipe.GetSubscriber<MoveTileEvent>().Subscribe(OnMoveTileEvent).AddTo(bagBuilder);
            _messageSubscription = bagBuilder.Build();

        }

        public override void Activate()
        {
        }

        public override void Deactivate()
        {
            _messageSubscription.Dispose();
        }

        public override void Dispose()
        {
        }

        private void OnMoveTileEvent(MoveTileEvent evt)
        {
            MoveTile(evt.Tile, evt.FromContainer).Forget();
        }

        private async UniTask MoveTile(Tile tile, Container fromContainer)
        {
            if (_currentDeckCounter >= DeckBoard.Data.Width)
            {
                return;
            }

            _vibrationManager.Vibrate(VibrationType.LightImpact);

            _inputCancellationTokenSource?.Cancel();
            _inputCancellationTokenSource?.Dispose();

            _inputCancellationTokenSource = new CancellationTokenSource();
            _inputCancellationToken = _inputCancellationTokenSource.Token;

            _currentDeckCounter++;
            TileMovementData tileMovementData = new(tile, fromContainer);
            _tileMovementData.Push(tileMovementData);

            tile.Data.Decked = true;
            tile.Data.Container.RemoveTile();
            tileMovementData.FromContainer.ForceBelowIndexesToCheckTileState();

            var sprite = tile.Data.Sprite;
            if (!_deckedTiles.ContainsKey(sprite))
            {
                _deckedTiles.Add(sprite, new());
                _spritesOrder.Add(sprite);
            }

            _deckedTiles[sprite].Add(tile);

            var container = RearrangeOthersAndFindMyPlace(tile);
            tile.InsertTileToContainer(container, false);
            tile.SetSelectable(false);

            await tile.MoveViewToLocalZero(_inputCancellationToken);

            CheckDeck();
        }

        private Container RearrangeOthersAndFindMyPlace(Tile newTile)
        {
            Container myContainer = null;
            List<KeyValuePair<Tile, Container>> tileContainerPairs = new();

            var containerIndex = 0;
            for (var i = 0; i < _spritesOrder.Count; i++)
            {
                var tileList = _deckedTiles[_spritesOrder[i]];
                for (var j = 0; tileList != null && j < tileList.Count; j++)
                {
                    var tile = tileList[j];
                    var oldContainer = tile.Data.Container;

                    if (tile == newTile)
                    {
                        myContainer = DeckBoard.GetContainer(Vector3Int.right * containerIndex);
                    }
                    else if (oldContainer.Data.Index != Vector3Int.right * containerIndex)
                    {
                        tile.Data.Container.RemoveTile();
                        tileContainerPairs.Add(new KeyValuePair<Tile, Container>(tile, DeckBoard.GetContainer(Vector3Int.right * containerIndex)));
                    }

                    containerIndex++;
                }
            }

            for (var i = 0; i < tileContainerPairs.Count; i++)
            {
                var tile = tileContainerPairs[i].Key;
                tile.InsertTileToContainer(tileContainerPairs[i].Value, false);
                tile.SetSelectable(false);
                tile.MoveViewToLocalZero(_inputCancellationToken).Forget();
            }

            return myContainer;
        }

        private void RearrangeDeck()
        {
            List<KeyValuePair<Tile, Container>> tileContainerPairs = new();

            var containerIndex = 0;
            for (var i = 0; i < _spritesOrder.Count; i++)
            {
                var tileList = _deckedTiles[_spritesOrder[i]];
                for (var j = 0; tileList != null && j < tileList.Count; j++)
                {
                    var tile = tileList[j];
                    var oldContainer = tile.Data.Container;

                    if (oldContainer.Data.Index != Vector3Int.right * containerIndex)
                    {
                        tile.Data.Container.RemoveTile();
                        tileContainerPairs.Add(new KeyValuePair<Tile, Container>(tile, DeckBoard.GetContainer(Vector3Int.right * containerIndex)));
                    }

                    containerIndex++;
                }
            }

            for (var i = 0; i < tileContainerPairs.Count; i++)
            {
                var tile = tileContainerPairs[i].Key;
                tile.InsertTileToContainer(tileContainerPairs[i].Value, false);
                tile.SetSelectable(false);
                tile.MoveViewToLocalZero(_inputCancellationToken).Forget();
            }
        }

        private void CheckDeck()
        {
            var despawned = false;
            for (var i = 0; i < _spritesOrder.Count; i++)
            {
                var sprite = _spritesOrder[i];
                var tileList = _deckedTiles[sprite];

                if (tileList.Count >= _matchCount)
                {
                    for (var j = 0; j < _matchCount; j++)
                    {
                        var tile = tileList[j];
                        tile.Data.Container.RemoveTile();
                        _tileControllerSystem.DespawnTile(tile);
                        _currentDeckCounter--;
                    }

                    _deckedTiles[sprite].RemoveRange(0, _matchCount);
                    despawned = true;
                }

                if (_deckedTiles[sprite].Count == 0)
                {
                    _deckedTiles.Remove(sprite);
                    _spritesOrder.Remove(sprite);
                    i--;
                }
            }

            if (_currentDeckCounter >= DeckBoard.Data.Width)
            {
                _session.LevelFinished(false);
                return;
            }

            if (despawned)
            {
                _soundManager.PlayOneShot(CFSoundTypes.SuccessSound);
                _vibrationManager.Vibrate(VibrationType.Success);
                RearrangeDeck();
            }
        }

        public void Undo()
        {
            if (_currentDeckCounter <= 0)
            {
                return;
            }

            TileMovementData tileMovementData;
            Tile tile;
            do
            {
                tileMovementData = _tileMovementData.Pop();

                tile = tileMovementData.Tile;
            }
            while (tile.Data.Despawned);

            if (tile.Data.Despawned)
            {
                return;
            }

            _inputCancellationTokenSource?.Cancel();
            _inputCancellationTokenSource?.Dispose();

            _inputCancellationTokenSource = new CancellationTokenSource();
            _inputCancellationToken = _inputCancellationTokenSource.Token;

            _currentDeckCounter--;

            tile.Data.Container.RemoveTile();
            tileMovementData.Tile.InsertTileToContainer(tileMovementData.FromContainer, false);
            tile.MoveViewToLocalZero(_inputCancellationToken).Forget();
            tile.Data.Decked = false;
            tileMovementData.Tile.SetSelectable(true);

            tileMovementData.FromContainer.ForceBelowIndexesToCheckTileState();

            var sprite = tile.Data.Sprite;
            if (_deckedTiles.ContainsKey(sprite))
            {
                _deckedTiles[sprite].Remove(tile);
                if (_deckedTiles[sprite].Count == 0)
                {
                    _deckedTiles.Remove(sprite);
                    _spritesOrder.Remove(sprite);
                }
            }

            RearrangeDeck();
        }

        public void OpenUpAdditionalSpace()
        {
            _inputCancellationTokenSource?.Cancel();
            _inputCancellationTokenSource?.Dispose();

            _inputCancellationTokenSource = new CancellationTokenSource();
            _inputCancellationToken = _inputCancellationTokenSource.Token;

            if (_currentDeckCounter < 3)
            {
                return;
            }

            var additionalDeckBoard = _boardControllerSystem.AddAdditionalDeckBoard();
            for (var i = 0; i < 3; i++)
            {
                var fromContainer = DeckBoard.GetContainer(Vector3Int.right * (_currentDeckCounter - 1));
                var toContainer = additionalDeckBoard.GetContainer(Vector3Int.right * ((DeckBoard.Data.Width / 2) - 1 + i));
                _currentDeckCounter--;

                var tile = fromContainer.Data.Tile;

                fromContainer.RemoveTile();
                tile.InsertTileToContainer(toContainer, false);
                tile.SetSelectable(true);

                var sprite = tile.Data.Sprite;
                _deckedTiles[sprite].Remove(tile);
                if (_deckedTiles[sprite].Count == 0)
                {
                    _deckedTiles.Remove(sprite);
                    _spritesOrder.Remove(sprite);
                }

                tile.MoveViewToLocalZero(_inputCancellationToken).Forget();
            }

            RearrangeDeck();
        }
    }
}
