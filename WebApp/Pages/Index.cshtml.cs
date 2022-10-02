using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BattleShipBrain;
using DAL;
using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using WebApp.Pages.Game;

namespace WebApp.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly DAL.ApplicationDbContext _context;


        public IndexModel(ILogger<IndexModel> logger, DAL.ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }
        
        [BindProperty]
        public int GameConfigId { get; set; }
        
        [BindProperty]
        public ShipPlacementType ShipPlacementType { get; set; }

        public IActionResult OnGet()
        {
            DataBaseAccess.CreateDefaultConfig();
            ViewData["GameConfigId"] = new SelectList(_context.GameConfigs.Where(gc => gc.Complete == 1), "GameConfigId", "ConfigName");
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var gameConfigFromDb = await _context.GameConfigs.FindAsync(GameConfigId);

            List<ShipConfig> shipConfigs = new List<ShipConfig>();
            foreach (var gameShipConfig in _context.GameShipConfig.Where(gsc => gsc.GameConfigId == GameConfigId))
            {
                shipConfigs.Add(new ShipConfig
                {
                    Name = gameShipConfig.ShipName,
                    Quantity = gameShipConfig.Quantity,
                    ShipSizeY = gameShipConfig.ShipSizeX,
                    ShipSizeX = gameShipConfig.ShipSizeY
                });
            }


            var gameConfiguration = new BattleShipBrain.GameConfiguration
            {
                BoardSizeX = gameConfigFromDb.BoardSizeX,
                BoardSizeY = gameConfigFromDb.BoardSizeY,
                EShipTouchRule = (EShipTouchRule) gameConfigFromDb.EShipTouchRule,
                ShipConfigs = shipConfigs
            };

            var brain = new BSBrain(gameConfiguration);
            if (ShipPlacementType == ShipPlacementType.Random) {brain.CreateRandomBoards();}
            var (board1, board2) = brain.GetGameBoardsAsString();
            var name = "save-game-" + DateTime.Now.ToString("MM/dd/yyyy-H-mm");

            var gameSave = new Domain.GameSave
            {
                SaveName = name,
                GameCurrentPlayerNumber = 0,
                GameMovesNumber = 0,
                FirstGameBoard = board1,
                SecondGameBoard = board2,
                GameConfigId = gameConfigFromDb.GameConfigId,
                GamePhase = ShipPlacementType == ShipPlacementType.Random ? 2 : 1
            };
            
            _context.GameSaves.Add(gameSave);
            await _context.SaveChangesAsync();

            if (ShipPlacementType == ShipPlacementType.Random)
            {
                return RedirectToPage("./Game/PlayGame", new {id = gameSave.GameSaveId});
            }
            
            foreach (var gameShipConfigFromDb in _context.GameShipConfig.Where(gsc => gsc.GameConfigId == GameConfigId))
            {
                for (int i = 0; i < gameShipConfigFromDb.Quantity; i++)
                {
                    var ship = new GameShip()
                    {
                        IsPlaced = 0,
                        GameSaveId = gameSave.GameSaveId,
                        GameShipConfigId = gameShipConfigFromDb.GameShipConfigId,
                        Player = 0
                    };
                    _context.GameShip.Add(ship);
                    
                    var ship2 = new GameShip()
                    {
                        IsPlaced = 0,
                        GameSaveId = gameSave.GameSaveId,
                        GameShipConfigId = gameShipConfigFromDb.GameShipConfigId,
                        Player = 1
                    };
                    _context.GameShip.Add(ship2);
                }
            }
            
            await _context.SaveChangesAsync();
            
            return RedirectToPage("./Game/PlaceShips", new {id = gameSave.GameSaveId, player = 0});
        }
    }
}