using System;
using System.Collections.Generic;
using System.Threading;
using CerberusFramework.Core;
using CerberusFramework.Managers.Asset;
using CFGameClient;
using CFGameClient.Core;
using CFGameClient.Core.Systems.ViewSpawnerSystem;
using CFGameClient.Data;
using Cysharp.Threading.Tasks;
using GameClient.Core.Board;
using GameClient.GameData;
using UnityEngine;
using VContainer;

namespace GameClient.Core.Systems.BoardControllerSystem
{
    [CreateAssetMenu(fileName = "BoardControllerSystem", menuName = "GameClient/Systems/BoardControllerSystem", order = 5)]
    public class BoardControllerSystem : GameSystem, IBoardControllerSystem
    {
        public override Type RegisterType => typeof(IBoardControllerSystem);

        protected IObjectResolver _objectResolver;
        private IViewSpawnerSystem _viewSpawnerSystem;
        private AddressableManager _addressableManager;

        private LevelDesignData _levelDesignData;
        private GameSettings _gameSettings;

        private List<Board.Board> _allBoards;

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

            _allBoards = new List<Board.Board>();
            _levelDesignData = _session.LevelDesignData;
            _gameSettings = _session.GameSettings;
            _viewSpawnerSystem = _session.GetSystem<IViewSpawnerSystem>();

            CreateBoards();
        }

        public override void Activate()
        {
        }

        public override void Deactivate()
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
        }

        public override void Dispose()
        {
            for (var i = 0; i < _allBoards.Count; i++)
            {
                _allBoards[i].Dispose();
                _viewSpawnerSystem.Despawn(GamePoolKeys.Board, _allBoards[i].View);
            }
        }

        private void CreateBoards()
        {
            _allBoards = new();

            for (var i = 0; i < _levelDesignData.BoardDesigns.Count; i++)
            {
                var design = _levelDesignData.BoardDesigns[i];

                switch (design.BoardType)
                {
                    case (int)BoardType.Pyramid:
                        {
                            var boardData = new BoardData(i, design.Elevation, design.Width, design.Height, design);
                            var boardView = _viewSpawnerSystem.Spawn<BoardView>(GamePoolKeys.Board);
                            var board = new PyramidBoard();
                            _objectResolver.Inject(board);
                            board.SetDataAndView(boardData, boardView);

                            boardView.transform.position = (design.Position.x * _gameSettings.TileSize.x * Vector3.right) +
                                                            (design.Position.y * _gameSettings.TileSize.y * Vector3.up);

                            _allBoards.Add(board);
                        }

                        break;
                    case (int)BoardType.Stack:
                        {
                            var boardData = new BoardData(i, design.Elevation, design.Width, design.Height, design);
                            var boardView = _viewSpawnerSystem.Spawn<BoardView>(GamePoolKeys.Board);
                            var board = new StackBoard();
                            _objectResolver.Inject(board);
                            board.SetDataAndView(boardData, boardView);

                            boardView.transform.position = (design.Position.x * _gameSettings.TileSize.x * Vector3.right) +
                                                            (design.Position.y * _gameSettings.TileSize.y * Vector3.up);

                            _allBoards.Add(board);
                        }

                        break;
                }
            }

            var boardDesign = new BoardDesignData
            {
                BoardType = (int)BoardType.Deck,
                Elevation = 1,
                Width = _session.GameSettings.DeckSize,
                Height = 1,
                Position = new Vector3(-3, -3)
            };

            var deckBoardData = new BoardData(_allBoards.Count, 1, _session.GameSettings.DeckSize, 1, boardDesign);
            var deckBoardView = _viewSpawnerSystem.Spawn<BoardView>(GamePoolKeys.Board);
            var deckBoard = new DeckBoard();
            _objectResolver.Inject(deckBoard);
            deckBoard.SetDataAndView(deckBoardData, deckBoardView);

            deckBoardView.transform.position = (boardDesign.Position.x * Vector3.right * _gameSettings.TileSize.x) + (boardDesign.Position.y * Vector3.up);

            _allBoards.Add(deckBoard);

            for (var i = 0; i < _allBoards.Count; i++)
            {
                _allBoards[i].Initialize(_cancellationTokenSource.Token);
                _allBoards[i].Activate();
            }
        }

        public Board.Board GetBoard(int boardID)
        {
            return _allBoards[boardID];
        }

        public DeckBoard AddAdditionalDeckBoard()
        {
            var boardDesign = new BoardDesignData
            {
                BoardType = (int)BoardType.Deck,
                Elevation = 1,
                Width = _session.GameSettings.DeckSize,
                Height = 1,
                Position = new Vector3(-3, -3)
            };

            var additionalDeckBoardIndex = _allBoards.Count - 3;
            var deckBoardData = new BoardData(3 + additionalDeckBoardIndex, 1, _session.GameSettings.DeckSize, 1, boardDesign);
            var deckBoardView = _viewSpawnerSystem.Spawn<BoardView>(GamePoolKeys.Board);
            var additionalDeckBoard = new DeckBoard();
            _objectResolver.Inject(additionalDeckBoard);
            additionalDeckBoard.SetDataAndView(deckBoardData, deckBoardView);

            deckBoardView.transform.position = (boardDesign.Position.x * Vector3.right * _gameSettings.TileSize.x) +
                (boardDesign.Position.y * Vector3.up) + (_gameSettings.TileSize.y * Vector3.up) +
                (additionalDeckBoardIndex * Vector3.back);

            _allBoards.Add(additionalDeckBoard);

            additionalDeckBoard.Initialize(_cancellationTokenSource.Token);
            additionalDeckBoard.Activate();

            return additionalDeckBoard;
        }
    }
}
