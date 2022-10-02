using System;
using BattleShipBrain;

namespace BattleShipConsoleUI
{
    // OLD UI, used shortcut navigation, switched to arrow key. New UI is BoardUI
    public class BSConsoleUI
    {

        private readonly Func<BoardSquareState[,]> _getCurrentBoardStateFirstPlayer;
        private readonly Func<BoardSquareState[,]> _getCurrentBoardStateSecondPlayer;
        private readonly Func<int, int, int, string> _placeBomb;
        private static (int, int) _size;
        private static int _move = 1;

        public BSConsoleUI(Func<BoardSquareState[,]> getCurrentBoardStateFirstPlayer,
            Func<BoardSquareState[,]> getCurrentBoardStateSecondPlayer, Func<int, int, int, string> placeBomb,
            (int, int) size)
        {
            _getCurrentBoardStateFirstPlayer = getCurrentBoardStateFirstPlayer;
            _getCurrentBoardStateSecondPlayer = getCurrentBoardStateSecondPlayer;
            _placeBomb = placeBomb;
            _size = size;
        }

        public void RunUI()
        {
            var done = false;

            do
            {
                var firstPlayerBoard = _getCurrentBoardStateFirstPlayer();
                var secondPlayerBoard = _getCurrentBoardStateSecondPlayer();
                
                // Draw first player playground First player board and Second player board, but with hidden ships
                DrawDelimiter(0);
                Console.WriteLine("First player move. Move: " + _move, Console.ForegroundColor = ConsoleColor.Red);
                DrawDelimiter(1);
                Console.WriteLine("First Player Board",Console.ForegroundColor = ConsoleColor.Yellow );
                DrawBoard(firstPlayerBoard, false);
                DrawDelimiter(1);
                Console.WriteLine("Second Player Board. Board to attack!!",Console.ForegroundColor = ConsoleColor.Yellow );
                DrawBoard(secondPlayerBoard, true);

                var input = AskChoice();
                _placeBomb(input.Item1, input.Item2, 1);

                // Draw second player playground Second player board and First player board, but with hidden ships
                DrawDelimiter(0);
                Console.WriteLine("Second player move. Move: " + _move, Console.ForegroundColor = ConsoleColor.Red);
                DrawDelimiter(1);
                Console.WriteLine("Second Player Board", Console.ForegroundColor = ConsoleColor.Yellow);
                DrawBoard(secondPlayerBoard,false);
                DrawDelimiter(1);
                Console.WriteLine("First Player Board. Board to attack!!",Console.ForegroundColor = ConsoleColor.Yellow );
                DrawBoard(firstPlayerBoard, true);
               
                input = AskChoice();
                _placeBomb(input.Item1, input.Item2, 0);

                _move++;
                
            } while (!done);
            
        }

        private static (int, int) AskChoice()
        {
            var doneX = false;
            var doneY = false;

            var valueX = 0;
            var valueY = 0;
            
            // until x is correct
            do
            {
                Console.Write("Your choice X: ", Console.ForegroundColor = ConsoleColor.Cyan);
                var input = Console.ReadLine()?.Trim();
                var success = int.TryParse(input, out valueX);
                doneX = HandleInput(valueX, success);

            } while (!doneX);
            
            // until y is correct
            do
            {
                Console.Write("Your choice Y: ", Console.ForegroundColor = ConsoleColor.Cyan);
                var input = Console.ReadLine()?.Trim();
                var success = int.TryParse(input, out valueY);
                doneY = HandleInput(valueY, success);

            } while (!doneY);

            return (valueX, valueY);
        }


        private static bool HandleInput(int? input, bool success)
        {
            if (success && input != null && _size.Item1 > input && input >= 0) return true;
            Console.WriteLine("Use correct input", Console.ForegroundColor = ConsoleColor.Red);
            return false;
        }


        private static void DrawBoard(BoardSquareState[,] board, bool hidden)
        {
            for (var y = 0; y < board.GetLength(1); y++)
            {
                DrawHorizontalFrame(board.GetLength(0));
                
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("| ");
                
                for (var x = 0; x < board.GetLength(0); x++)
                {
                    DrawElement(board[x, y].ToString(), hidden);

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write(" | ");
                }
                Console.WriteLine();
            }
            DrawHorizontalFrame(board.GetLength(0));
        }

        private static void DrawElement(string element, bool hidden)
        {
            Console.ForegroundColor = ConsoleColor.White;
            switch (element)
            {
                case "8":
                    if (hidden) Console.Write(" ");
                    else Console.Write(element, Console.ForegroundColor = ConsoleColor.White);
                    break;
                case "X":
                    Console.Write(element, Console.ForegroundColor = ConsoleColor.Red);
                    break;
                case "-":
                    Console.Write(element, Console.ForegroundColor = ConsoleColor.Blue);
                    break;
                case " ":
                    Console.Write(" ");
                    break;
            }
        }

        private static void DrawHorizontalFrame(int length)
        {
            Console.ForegroundColor = ConsoleColor.Green;

            for (var i = 0; i < length - 1; i++)
            {
                Console.Write("+---");
            }
            Console.Write("+---+");

            Console.WriteLine();
        }

        private static void DrawDelimiter(int type)
        {
            switch (type)
            {
                case 0:
                    Console.WriteLine(new string('=', _size.Item1 * 4 + 1),
                        Console.ForegroundColor = ConsoleColor.White);
                    break;
                case 1:
                    Console.WriteLine("<" + new string('-', _size.Item1 * 4 - 1) + ">",
                        Console.ForegroundColor = ConsoleColor.White);
                    break;
            }
        }
    }
}