using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using BattleShipBrain;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace BattleShipConsoleUI
{
    public class PlaceShipsUI
    {
        private readonly Func<BoardSquareState[,]> _getCurrentPlayerBoard;
        
        private readonly GameConfiguration _gameConfiguration;
        private readonly int _boardSizeX;
        private readonly int _boardSizeY;
        
        private readonly Func<string, List<Coordinate>, int, int, bool> _placeShip;
        private readonly int _player;
        private readonly Action _deleteLastShip;

        public PlaceShipsUI(Func<BoardSquareState[,]> getCurrentPlayerBoard,
            GameConfiguration gameConfiguration, Func<string, List<Coordinate>, int, int, bool> placeShip, int player,
            Action deleteLastShip)
        {
            _getCurrentPlayerBoard = getCurrentPlayerBoard;
            _gameConfiguration = gameConfiguration;
            _boardSizeX = gameConfiguration.BoardSizeX;
            _boardSizeY = gameConfiguration.BoardSizeY;
            _placeShip = placeShip;
            _player = player;
            _deleteLastShip = deleteLastShip;
        }
        
        public void RunUi()
        {

            var allShips = new List<ShipConfig>();
            
            foreach (var shipConfig in _gameConfiguration.ShipConfigs)
            {
                for (int i = 0; i < shipConfig.Quantity; i++)
                {
                    allShips.Add(shipConfig);
                }
            }
            
            for (var index = 0; index < allShips.Count; index++)
            {
                var success = false;
                do
                {
                    var input = OutPutMenu(_getCurrentPlayerBoard(),
                        (allShips[index].ShipSizeX, allShips[index].ShipSizeY), _player);
                    
                    // undo output
                    if (input == null)
                    {
                        if (index > 0) index--;
                        continue;
                    }

                    var coordinates = new List<Coordinate>();
                    foreach (var x in input.Value.Item1)
                    {
                        foreach (var y in input.Value.Item2)
                        {
                            var c = new Coordinate
                            {
                                X = x,
                                Y = y
                            };
                            coordinates.Add(c);
                        }
                    }
                    
                    // place ship controls the rule
                    success = _placeShip(allShips[index].Name, coordinates,
                        allShips[index].ShipSizeX, allShips[index].ShipSizeY);
                    
                } while (!success);
            }
        }
        
        private List<int> _currentItemsX = new List<int>();
        private List<int> _currentItemsY = new List<int>();

        private (List<int>, List<int>)? OutPutMenu(BoardSquareState[,] boardLeft, (int, int) shipSize, int player)
        {
            ConsoleKeyInfo key;
            
            _currentItemsX = new List<int>();
            _currentItemsY = new List<int>();
            
            for (var i = 0; i < shipSize.Item1; i++) _currentItemsX.Add(i);
            for (var i = 0; i < shipSize.Item2; i++) _currentItemsY.Add(i);
            
            do
            {
                Console.Clear();
                Console.WriteLine($"Player: {player}, place your ships", Console.ForegroundColor = ConsoleColor.Red);
                Console.WriteLine("Press 'Space' to rotate the ship");
                Console.WriteLine("Press 'Backspace' to undo last placement");
                Console.WriteLine();
                
                DrawBoardElements.DrawBoard(_boardSizeX, _boardSizeY, boardLeft, Highlight);

                key = Console.ReadKey(true);
    
                switch (key.Key)
                {
                    case ConsoleKey.DownArrow:
                    {
                        if (!_currentItemsY.Contains(_boardSizeY - 1)) 
                            _currentItemsY = _currentItemsY.Select(y => y + 1).ToList();
                        break;
                    }
                    case ConsoleKey.UpArrow:
                    {
                        if (!_currentItemsY.Contains(0))
                            _currentItemsY = _currentItemsY.Select(y => y - 1).ToList();
                        break;
                    }
                    case ConsoleKey.RightArrow:
                    {
                        if (!_currentItemsX.Contains(_boardSizeX - 1)) 
                            _currentItemsX = _currentItemsX.Select(x => x + 1).ToList();
                        break;
                    }
                    case ConsoleKey.LeftArrow:
                    {
                        if (!_currentItemsX.Contains(0))
                            _currentItemsX = _currentItemsX.Select(x => x - 1).ToList();
                        break;
                    }
                    case ConsoleKey.Spacebar:
                    {
                        var tmp = new List<int>(_currentItemsX);
                        _currentItemsX = new List<int>(_currentItemsY);
                        _currentItemsY = new List<int>(tmp);
                        break;
                    }
                    case ConsoleKey.Backspace:
                    {
                        _deleteLastShip();
                        return null;
                    }
                }

            } while (key.Key != ConsoleKey.Enter);

            return (_currentItemsX, _currentItemsY);
        }

        private void Highlight(int x, int y, BoardSquareState[,] board )
        {
            if (_currentItemsX.Contains(x) && _currentItemsY.Contains(y))
            {
                Console.ForegroundColor = ConsoleColor.Black;
                Console.BackgroundColor = ConsoleColor.White;
            } 
                        
            Console.Write($" {board[x, y].ToString()} ", Console.ForegroundColor = ConsoleColor.White);
        }
    }
}