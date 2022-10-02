namespace BattleShipBrain
{
    public class ShipConfig
    {
        public string Name { get; set; } = "Sample name";

        public int Quantity { get; set; }
        
        public int ShipSizeY { get; set; } 
        public int ShipSizeX { get; set; }

        public override string ToString()
        {
            return $"Name: {Name}, Quantity: {Quantity}, Size: {ShipSizeX}x{ShipSizeY}";
        }
    }
    
}