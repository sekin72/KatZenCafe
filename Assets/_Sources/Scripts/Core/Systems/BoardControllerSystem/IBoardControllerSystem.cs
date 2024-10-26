using CerberusFramework.Core.Systems;
using GameClient.Core.Board;

namespace GameClient.Core.Systems.BoardControllerSystem
{
    public interface IBoardControllerSystem : IGameSystem
    {
        Board.Board GetBoard(int boardID);
        DeckBoard AddAdditionalDeckBoard();
    }
}
