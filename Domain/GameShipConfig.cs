using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public class GameShipConfig
    {
        // PK
        public int GameShipConfigId { get; set; }
        
        [MaxLength (128)]
        [MinLength (2)]
        
        [Display(Name = "Ship name")]
        public string ShipName { get; set; } = default!;

        public int Quantity { get; set; }
        [Display(Name = "Ship size x axis")]
        public int ShipSizeX { get; set; }
        [Display(Name = "Ship size y axis")]
        public int ShipSizeY { get; set; }
        
        public int? GameConfigId { get; set; }
        public GameConfig? GameConfig { get; set; }
        
        public List<GameShip>? GameShips { get; set; }


    }
}