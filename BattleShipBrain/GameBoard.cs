using System.Collections.Generic;
using System.Linq;

namespace BattleShipBrain
{
    public class GameBoard
    {
        public BoardSquareState[,] Board { get; set; } = default!;
        public List<Ship> Ships { get; set; } = new List<Ship>();
        public EShipTouchRule Rule { get; set; } = EShipTouchRule.SideTouch;
        
        public bool AddShip(string name, List<Coordinate> coordinates)
        {
            if (!ControlRule(coordinates)) return false;
            
            var ship = new Ship(name, coordinates);

            foreach (var coordinate in ship.Coordinates)
            {
                Board[coordinate.X, coordinate.Y].IsShip = true;
            }
            
            Ships.Add(ship);
            return true;
        }

        public void DeleteShipWithCoordinates(List<Coordinate> coordinates)
        {
            foreach (var ship in Ships)
            {
                foreach (var coordinate in coordinates)
                {
                    if (ship.Coordinates.Contains(coordinate))
                    {
                        foreach (var c in ship.Coordinates)
                        {
                            Board[c.X, c.Y].IsShip = false;
                        }
                        Ships.Remove(ship);
                        return;
                    } 
                }
                
            }
        }
        
        public void DeleteLastShip()
        {
            if (Ships.Count > 0)
            {
                var lastShip = Ships.Last();
                
                foreach (var coordinate in lastShip.Coordinates)
                {
                    Board[coordinate.X, coordinate.Y].IsShip = false;
                }

                Ships.Remove(lastShip);
            }
        }

        public bool CheckIfLost()
        {
            return Ships.All(ship => ship.IsShipSunk(Board));
        }
        
        private bool ControlRule(List<Coordinate> coordinates)
        {
            if (coordinates.Any(coordinate => coordinate.X >= Board.GetLength(0)
                                              || coordinate.Y >= Board.GetLength(1))) return false;
            if (coordinates.Any(coordinate => Board[coordinate.X, coordinate.Y].IsShip)) return false;
            
            switch (Rule)
            {
                case EShipTouchRule.NoTouch:
                    return LookCorners(coordinates) && LookSides(coordinates);
                case EShipTouchRule.CornerTouch:
                    return LookSides(coordinates);
            }

            return true;
        }

        private bool LookSides(List<Coordinate> coordinates)
        {
            foreach (var coordinate in coordinates)
            {
                if (IsCoordinateOccupied(coordinate, 0, -1)) return false;
                if (IsCoordinateOccupied(coordinate, 0, +1)) return false;
                if (IsCoordinateOccupied(coordinate, -1, 0)) return false;
                if (IsCoordinateOccupied(coordinate, +1, 0)) return false;
            }

            return true;
        }

        private bool LookCorners(List<Coordinate> coordinates)
        {
            foreach (var coordinate in coordinates)
            {
                if (IsCoordinateOccupied(coordinate, -1, -1)) return false;
                if (IsCoordinateOccupied(coordinate, +1, +1)) return false;
                if (IsCoordinateOccupied(coordinate, +1, -1)) return false;
                if (IsCoordinateOccupied(coordinate, -1, +1)) return false;
            }

            return true;
        }

        private bool IsCoordinateOccupied(Coordinate coordinate, int x, int y)
        {
            return IsCorrect(coordinate.X + x, coordinate.Y + y)
                   && Board[coordinate.X + x, coordinate.Y + y].IsShip;
        }

        private bool IsCorrect(int x, int y)
        {
            return x >= 0 && y >= 0 && x < Board.GetLength(0) && y < Board.GetLength(1);
        }
    }
}