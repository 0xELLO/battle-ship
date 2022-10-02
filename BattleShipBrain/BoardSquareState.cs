namespace BattleShipBrain
{
    public struct BoardSquareState
    {
        public bool IsShip { get; set; }
        public bool IsBomb { get; set; }
        public bool IsSunk { get; set; }

        public override string ToString()
        {
            switch (IsEmpty: IsShip, IsBomb)
            {
                case (false, false):
                    return " ";
                case (false, true):
                    return "-";
                case (true, false):
                    return "8";
                case (true, true):
                    return "X";
            }
        }
    }
}
