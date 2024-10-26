using System;
using System.Collections.Generic;
using System.Threading;
using CerberusFramework.Core;
using CerberusFramework.Managers.Asset;
using CerberusFramework.Utilities.Extensions;
using CFGameClient;
using CFGameClient.Core;
using CFGameClient.Core.Systems.ViewSpawnerSystem;
using Cysharp.Threading.Tasks;
using GameClient.Core.Board;
using GameClient.Core.Board.Components;
using GameClient.Core.Systems.BoardControllerSystem;
using GameClient.GameData;
using UnityEngine;
using VContainer;
using Container = GameClient.Core.Board.Components.Container;
using Random = UnityEngine.Random;

namespace GameClient.Core.Systems.TileControllerSystem
{
    [CreateAssetMenu(fileName = "TileControllerSystem", menuName = "GameClient/Systems/TileControllerSystem", order = 3)]
    public class TileControllerSystem : GameSystem, ITileControllerSystem
    {
        public override Type RegisterType => typeof(ITileControllerSystem);

        protected IObjectResolver _objectResolver;
        private IViewSpawnerSystem _viewSpawnerSystem;
        private IBoardControllerSystem _boardControllerSystem;
        private AddressableManager _addressableManager;
        private SpritesHolder _spritesHolder;

        public List<Tile> AllTiles { get; private set; }
        private int _matchCount;
        private int _remainingTileCount;

        private CancellationTokenSource _cancellationTokenSource;

        [Inject]
        public void Inject(IObjectResolver objectResolver, AddressableManager addressableManager)
        {
            _objectResolver = objectResolver;
            _addressableManager = addressableManager;
        }

        public override async UniTask Initialize(GameSessionBase gameSessionBase, CancellationToken cancellationToken)
        {
            await base.Initialize(gameSessionBase, cancellationToken);

            _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            AllTiles = new();
            _spritesHolder = await _addressableManager.LoadAssetAsync<SpritesHolder>("SpritesHolder");
            _viewSpawnerSystem = _session.GetSystem<IViewSpawnerSystem>();
            _boardControllerSystem = _session.GetSystem<IBoardControllerSystem>();
            _matchCount = _session.GameSettings.MatchCount;
            _remainingTileCount = 0;
        }

        public override void Activate()
        {
            var levelData = _session.LevelDesignData;

            var totalTileCount = 0;

            for (var i = 0; i < levelData.BoardDesigns.Count; i++)
            {
                var design = levelData.BoardDesigns[i];
                switch (design.BoardType)
                {
                    case (int)BoardType.Pyramid:
                        {
                            var count = design.TilePositions.Count > 0
                                 ? design.TilePositions.Count
                                 : _matchCount * Random.Range(design.RandomTileCountMin, design.RandomTileCountMax);

                            design.TileCount = count;
                        }

                        break;
                    case (int)BoardType.Stack:
                        design.TileCount = design.Elevation * design.Width * design.Height;
                        break;
                }

                totalTileCount += design.TileCount;
            }

            var spriteList = GetShuffledSpriteList(totalTileCount);

            for (var i = 0; i < levelData.BoardDesigns.Count; i++)
            {
                var design = levelData.BoardDesigns[i];

                List<Container> containersList = new();
                switch (design.BoardType)
                {
                    case (int)BoardType.Pyramid:
                        {
                            containersList = FillPyramidBoard(i, design, spriteList);
                        }

                        break;
                    case (int)BoardType.Stack:
                        {
                            containersList = FillStackBoard(i, design, spriteList);
                        }

                        break;
                }

                for (var j = 0; j < containersList.Count; j++)
                {
                    containersList[j].CheckTileState();
                }
            }
        }

        public override void Deactivate()
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
        }

        public override void Dispose()
        {
            for (var i = 0; i < AllTiles.Count; i++)
            {
                AllTiles[i].Dispose();
                _viewSpawnerSystem.Despawn(GamePoolKeys.Tile, AllTiles[i].View);
            }

            AllTiles.Clear();
        }

        private List<Sprite> GetShuffledSpriteList(int totalTileCount)
        {
            List<Sprite> allSpritesList = new(_spritesHolder.Sprites);
            allSpritesList.Shuffle();
            var maxSpriteCount = _session.GameSettings.MaxSpriteCount;

            var selectedSpritesList = new List<Sprite>();
            for (var i = 0; i < totalTileCount / _matchCount; i++)
            {
                var sprite = allSpritesList[Random.Range(0, maxSpriteCount)];
                for (var j = 0; j < _matchCount; j++)
                {
                    selectedSpritesList.Add(sprite);
                }
            }

            selectedSpritesList.Shuffle();

            return selectedSpritesList;
        }

        private List<Container> FillStackBoard(int id, BoardDesignData boardDesignData, List<Sprite> spriteList)
        {
            var containerList = new List<Container>();

            var stackBoard = _boardControllerSystem.GetBoard(id);
            var stackBoardTileCount = boardDesignData.Elevation * boardDesignData.Width * boardDesignData.Height;

            for (var i = 0; i < stackBoardTileCount; i++)
            {
                var sprite = spriteList[0];
                spriteList.RemoveAt(0);

                var tileData = new TileData(sprite);
                var tileView = _viewSpawnerSystem.Spawn<TileView>(GamePoolKeys.Tile);
                var tile = new Tile();
                _objectResolver.Inject(tile);
                tile.SetDataAndView(tileData, tileView);

                _remainingTileCount++;
                AllTiles.Add(tile);

                var container = stackBoard.GetRandomContainer();
                containerList.Add(container);

                tile.InsertTileToContainer(container);

                tile.Initialize(_cancellationTokenSource.Token);
                tile.Activate();
            }

            return containerList;
        }

        private List<Container> FillPyramidBoard(int id, BoardDesignData boardDesignData, List<Sprite> spriteList)
        {
            var containerList = new List<Container>();
            var pyramidBoard = _boardControllerSystem.GetBoard(id);

            for (var i = 0; i < boardDesignData.TileCount; i++)
            {
                var sprite = spriteList[0];
                spriteList.RemoveAt(0);

                var tileData = new TileData(sprite);
                var tileView = _viewSpawnerSystem.Spawn<TileView>(GamePoolKeys.Tile);
                var tile = new Tile();
                _objectResolver.Inject(tile);
                tile.SetDataAndView(tileData, tileView);
                _remainingTileCount++;
                AllTiles.Add(tile);

                Container container;
                if (boardDesignData.TilePositions.Count > 0)
                {
                    container = pyramidBoard.GetContainer(boardDesignData.TilePositions[i]);
                    containerList.Add(container);
                }
                else
                {
                    container = pyramidBoard.GetRandomContainer();
                    containerList.Add(container);
                }

                tile.InsertTileToContainer(container);

                tile.Initialize(_cancellationTokenSource.Token);
                tile.Activate();
            }

            return containerList;
        }

        public void DespawnTile(Tile tile)
        {
            tile.Dispose();
            _viewSpawnerSystem.Despawn(GamePoolKeys.Tile, tile.View);
            AllTiles.Remove(tile);
            _remainingTileCount--;

            if (_remainingTileCount == 0)
            {
                _session.LevelFinished(true);
            }
        }
    }
}
