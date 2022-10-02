using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public class GameSave
    {
        // PK
        public int GameSaveId { get; set; }

        [MaxLength (128)]
        [MinLength(2)]
        [Display(Name = "Name")]
        public string SaveName { get; set; } = default!;
        [Display(Name = "Player to move")]
        public int GameCurrentPlayerNumber { get; set; }
        [Display(Name = "Moves number")]
        public int GameMovesNumber { get; set; }
        public string FirstGameBoard { get; set; } = default!;
        public string SecondGameBoard { get; set; } = default!;
        public int GamePhase { get; set; } = default!;
        
        
        // Fk
        public List<GameShip>? GameShips { get; set; }
        
        // FK
        public int? GameConfigId { get; set; }
        [Display(Name = "Board configuration")]
        public GameConfig? GameConfig { get; set; }
    }
}