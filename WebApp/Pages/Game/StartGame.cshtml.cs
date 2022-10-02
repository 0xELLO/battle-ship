using System.Linq;
using System.Threading.Tasks;
using BattleShipBrain;
using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebApp.Pages.Game
{
    public class StartGame : PageModel
    {
        private readonly DAL.ApplicationDbContext _context;

        public StartGame(DAL.ApplicationDbContext context)
        {
            _context = context;
        }
        

        public IActionResult OnGet()
        {
            ViewData["GameConfigId"] = new SelectList(_context.GameConfigs, "GameConfigId", "ConfigName");
            return Page();
        }
        
        [BindProperty]
        public int GameConfigId { get; set; } = default!;
        
        [BindProperty]
        public ShipPlacementType ShipPlacementType { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {

            var gameConfigFromDb = await _context.GameConfigs.FindAsync(GameConfigId);

            var brain = new BSBrain(new BattleShipBrain.GameConfiguration
            {
                BoardSizeX = gameConfigFromDb.BoardSizeX,
                BoardSizeY = gameConfigFromDb.BoardSizeY,
                EShipTouchRule = (EShipTouchRule) gameConfigFromDb.EShipTouchRule
            });

            var (board1, board2) = brain.GetGameBoardsAsString();
            
            var board = new Domain.GameSave
            {
                SaveName = "test",
                GameCurrentPlayerNumber = 0,
                GameMovesNumber = 0,
                FirstGameBoard = board1,
                SecondGameBoard = board2,
                GameConfigId = GameConfigId
            };

            _context.GameSaves.Add(board);
            
            await _context.SaveChangesAsync();


            foreach (var gameShipConfigFromDb in _context.GameShipConfig.Where(gsc => gsc.GameConfigId == GameConfigId))
            {
                for (int i = 0; i < gameShipConfigFromDb.Quantity; i++)
                {
                    var ship = new GameShip()
                    {
                        IsPlaced = 0,
                        GameSaveId = board.GameSaveId,
                        GameShipConfigId = gameShipConfigFromDb.GameShipConfigId,
                        Player = 0
                    };
                    _context.GameShip.Add(ship);
                    
                    var ship2 = new GameShip()
                    {
                        IsPlaced = 0,
                        GameSaveId = board.GameSaveId,
                        GameShipConfigId = gameShipConfigFromDb.GameShipConfigId,
                        Player = 1
                    };
                    _context.GameShip.Add(ship2);
                }
            }
            
            await _context.SaveChangesAsync();
            
            if (ShipPlacementType == ShipPlacementType.Manual)
            {
                return RedirectToPage("./PlaceShips", new {id = board.GameSaveId, player = 0});
            }
            return RedirectToPage("../Index");
        }
    }
}