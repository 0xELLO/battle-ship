namespace Domain
{
    public class GameShip
    {
        // PK
        public int GameShipId { get; set; }

        public string? Coordinates { get; set; }
        public int IsPlaced { get; set; }
        public int Player  {get; set; }
        
        public int? GameSaveId { get; set; }
        public GameSave? GameSave { get; set; }
        
        public int? GameShipConfigId { get; set; }
        public GameShipConfig? GameShipConfigs { get; set; }
    }
}