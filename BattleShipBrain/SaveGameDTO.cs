using System.Collections.Generic;
using System.Linq;

namespace BattleShipBrain
{
    public class SaveGameDTO
    {
        
        public int CurrentPlayerNo { get; set; } = 0;
        public GameBoardDTO[] GameBoards  { get; set; } = new GameBoardDTO[2];

        public int MovesN { get; set; }
        
        public class GameBoardDTO
        {
            public List<List<BoardSquareState>> Board { get; set; } = new List<List<BoardSquareState>>();
            public List<Ship> Ships { get; set; } = new List<Ship>();
        }

        public void SetGameBoard(GameBoard[] gameBoards)
        {
            
            GameBoards[0] = new GameBoardDTO();
            GameBoards[1] = new GameBoardDTO();

            for (int i = 0; i < 2; i++)
            {
                var board = gameBoards[i].Board;
                
                var list = Enumerable.Range(0, board.GetLength(0))
                    .Select(row => Enumerable.Range(0, board.GetLength(1))
                        .Select(col => board[row, col]).ToList()).ToList();
                
                GameBoards[i].Board = list;
                GameBoards[i].Ships = gameBoards[i].Ships;
            }
        }

        public GameBoard[] GetGameBoard()
        {
            var gameBoard = new GameBoard[2];

            gameBoard[0] = new GameBoard();
            gameBoard[1] = new GameBoard();


            for (int i = 0; i < 2; i++)
            {
                var a = new BoardSquareState[GameBoards[i]!.Board!.Count, GameBoards[i].Board[i].Count];
            
                for (int x = 0; x < GameBoards[i].Board.Count; x++)
                {
                    for (int y = 0; y < GameBoards[i].Board[x].Count; y++)
                    {
                        a[x, y] = GameBoards[i].Board[x][y]!;
                    }
                }

                gameBoard[i].Board = a;
                gameBoard[i].Ships = GameBoards[i].Ships;
            }
            
            return gameBoard;
        }
    }
}
