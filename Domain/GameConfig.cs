using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public class GameConfig
    {
        // PK
        public int GameConfigId { get; set; }

        [MaxLength(128)]
        [MinLength(2)]
        [Display(Name ="Name")]
        public string ConfigName { get; set; } = default!;
        
        [Display(Name ="Board size X axis")]
        public int BoardSizeX { get; set; }
        
        [Display(Name ="Board size Y axis")]
        public int BoardSizeY { get; set; }
        
        [Display(Name ="Game rule")]
        public int EShipTouchRule { get; set; }
        
        public int Complete { get; set; }
        
        
        //public string ShipConfigurations { get; set; } = default!;

        public List<GameShipConfig>? ShipConfigInGameConfigs { get; set; }
        public List<GameSave>? GameSaves { get; set; }
    }
}