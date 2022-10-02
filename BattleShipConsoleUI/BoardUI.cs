using System;
using BattleShipBrain;

namespace BattleShipConsoleUI
{
    public class BoardUI
    {
        private readonly Func<BoardSquareState[,]> _getCurrentPlayerBoard;
        private readonly Func<BoardSquareState[,]> _getOppositePlayerBoard;

        private readonly Action<int, int> _placeBomb;
        private readonly int _boardSizeX;
        private readonly int _boardSizeY;
        
        private static (int, int) _size;
        
        private readonly Action _saveGame;
        private readonly Func<int> _getPlayerN;
        private readonly Func<int> _getMoveN;
        private readonly Func<bool> _checkIfPlayerWon;

        public BoardUI(Func<BoardSquareState[,]> getCurrentPlayerBoard, Func<BoardSquareState[,]> getOppositePlayerBoard,
            Action<int, int> placeBomb, (int, int) size, Action saveGame,
            Func<int> getPlayerN, Func<int> getMoveN, Func<bool> checkIfPlayerWon)
        {
            _getCurrentPlayerBoard = getCurrentPlayerBoard;
            _getOppositePlayerBoard = getOppositePlayerBoard;
            _placeBomb = placeBomb;
            _size = size;
            _boardSizeX = size.Item1;
            _boardSizeY = size.Item2;
            
            _saveGame = saveGame;
            _getPlayerN = getPlayerN;
            _getMoveN = getMoveN;
            _checkIfPlayerWon = checkIfPlayerWon;
        }

        public void RunUI()
        {
            var runDone = false;
            do
            {
                var input = OutPutMenu(_getOppositePlayerBoard(), _getCurrentPlayerBoard());
                if (input == (-1, -1))
                {
                    runDone = true;
                    _saveGame();
                    ActionWindowsUi.RunSaving();
                }
                else
                {
                    _placeBomb(input.Item1, input.Item2);
                }

                if (!_checkIfPlayerWon()) continue;
                ActionWindowsUi.WinUi(_getPlayerN(), _getMoveN());
                runDone = true;

            } while (!runDone);
            
        }
        
        private int currentItemX = 0;
        private int currentItemY = 0;
        
        private (int, int) OutPutMenu(BoardSquareState[,] boardLeft, BoardSquareState[,] boardRight)
        {
            ConsoleKeyInfo key;
            currentItemX = 0;
            currentItemY = 0;
            
            do
            {
                Console.Clear();

                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Player " + _getPlayerN().ToString() + " to move. Move: " + _getMoveN().ToString());
                Console.WriteLine("Press 'S' to save the game");
                Console.WriteLine();
                DrawTitle();
                Console.WriteLine();
                
                DrawBoardElements.DrawBoard(_boardSizeX, _boardSizeY, boardLeft, Hightlight, boardRight);
                
                key = Console.ReadKey(true);
    
                switch (key.Key)
                {
                    case ConsoleKey.DownArrow:
                    {
                        currentItemY++;
                        if (currentItemY > _size.Item2 - 1) currentItemY = 0;
                        break;
                    }
                    case ConsoleKey.UpArrow:
                    {
                        currentItemY--;
                        if (currentItemY < 0) currentItemY = _size.Item2 - 1;
                        break;
                    }
                    case ConsoleKey.RightArrow:
                    {
                        currentItemX++;
                        if (currentItemX > _size.Item1 - 1) currentItemX = 0;
                        break;
                    }
                    case ConsoleKey.LeftArrow:
                    {
                        currentItemX--;
                        if (currentItemX < 0) currentItemX = _size.Item1 - 1;
                        break;
                    }
                    case ConsoleKey.S:
                    {
                        return (-1, -1);
                    }
                }

            } while (key.Key != ConsoleKey.Enter);

            return (currentItemX, currentItemY);
        }

        private void Hightlight(int x, int y, BoardSquareState[,] boardLeft)
        {
            if (currentItemX == x && currentItemY == y)
            {
                Console.ForegroundColor = ConsoleColor.Black;
                Console.BackgroundColor = ConsoleColor.White;
            } 
                        
            DrawBoardElements.DrawElement(boardLeft[x, y].ToString(), true);
        }

        private void DrawTitle()
        {
            Console.Write("   Enemy's board");
            Console.Write(new string(' ', _boardSizeX  * 2 + 19));
            Console.Write("Your board");
        }
    }
}