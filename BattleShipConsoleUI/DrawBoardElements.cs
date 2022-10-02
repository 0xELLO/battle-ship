using System;
using BattleShipBrain;

namespace BattleShipConsoleUI
{
    public static class DrawBoardElements
    {
        private static readonly char[] Alpha = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();

        public static void DrawBoard(int boardSizeX, int boardSizeY, BoardSquareState[,] boardLeft,
            Action<int, int, BoardSquareState[,]> highlightElement, BoardSquareState[,]? boardRight = null)
        {
            var boardsCount = boardRight == null ? 1 : 2;

            DrawAlphabeticLabel(boardSizeX, boardsCount);
            
            for (var y = 0; y < boardSizeY; y++)
            {
                DrawHorizontalLine(boardSizeX, boardsCount);
                
                Console.WriteLine();
                
                if (y > 9) Console.Write(y + " ", Console.ForegroundColor = ConsoleColor.Blue);
                else Console.Write(" " + y + " ", Console.ForegroundColor = ConsoleColor.Blue);
                
                Console.Write("|", Console.ForegroundColor = ConsoleColor.Green);
                
                for (var x = 0; x < boardSizeX; x++)
                {
                    highlightElement(x, y, boardLeft);
                    
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.Write("|", Console.ForegroundColor = ConsoleColor.Green);
                }

                if (boardRight != null)
                { 
                    Console.Write("        ");
                    
                    if (y > 9) Console.Write(y + " ", Console.ForegroundColor = ConsoleColor.Blue);
                    else Console.Write(" " + y + " ", Console.ForegroundColor = ConsoleColor.Blue);
                    
                    Console.Write("|", Console.ForegroundColor = ConsoleColor.Green);
                    
                    for (var x = 0; x < boardSizeX; x++)
                    {
                        DrawElement(boardRight[x, y].ToString(), false);
                        Console.Write("|", Console.ForegroundColor = ConsoleColor.Green);
                    }
                }
                Console.WriteLine();
            }
                
            DrawHorizontalLine(boardSizeX, boardsCount);
        }
        
        private static void DrawHorizontalLine(int length, int times)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            
            Console.Write("   ");

            for (int t = 0; t < times; t++)
            {
                for (var i = 0; i < length - 1; i++)
                {
                    Console.Write("+---");
                }
                Console.Write("+---+");
                Console.Write(new string(' ', 11));
            }
        }

        private static void DrawAlphabeticLabel(int length, int times)
        {
            Console.Write("   ");

            for (int t = 0; t < times; t++)
            {
                for (var i = 0; i < length; i++)
                {
                    if (i > 25) Console.Write(" " + Alpha[i % 26] + Alpha[i % 26] + " ", Console.ForegroundColor = ConsoleColor.Blue);
                    else Console.Write("  " + Alpha[i] + " ", Console.ForegroundColor = ConsoleColor.Blue);
                }
                Console.Write(new string(' ', 12));
            }
            Console.WriteLine();

        }
        
        public static void DrawElement(string element, bool hidden)
        {
            Console.ForegroundColor = ConsoleColor.White;
            switch (element)
            {
                case "8":
                    if (hidden) Console.Write("   ");
                    else Console.Write(" " + element + " ");
                    break;
                case "X":
                    Console.Write(" " + element + " ", Console.ForegroundColor = ConsoleColor.Red);
                    break;
                case "-":
                    Console.Write(" " + element + " ", Console.ForegroundColor = ConsoleColor.Blue);
                    break;
                case " ":
                    Console.Write("   ");
                    break;
            }
        }
    }
}