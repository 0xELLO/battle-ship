using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BattleShipBrain;
using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
using DAL;

namespace WebApp.Pages.Game
{
    public class ChooseLocation : PageModel
    {
        private readonly DAL.ApplicationDbContext _context;
        
        public ChooseLocation(DAL.ApplicationDbContext context)
        {
            _context = context;
        }
        
        public Domain.GameSave GameSaveFromDb  { get; set; } = default!;
        public GameShip GameShipFromDb  { get; set; } = default!;
        public GameConfig GameConfigFromDb { get; set; } = default!;
        
        public GameShipConfig GameShipConfigFromDb { get; set; } = default!;
        
        public BSBrain Brain { get; set; } = default!;
        
        [BindProperty]
        public int SizeX { get; set; } 
        [BindProperty]
        public int SizeY { get; set; }
        [BindProperty]
        public int ShipId { get; set; }
        public bool Error { get; set; }
        
        public IActionResult OnGet(int? boardId, int? shipId, string? error, int? player)
        {
            if (boardId == null || player == null || shipId == null)
            {
                return NotFound();
            }

            if (error != null)
            {
                Error = true;
            }

            Player = (int) player;
            ShipId = (int) shipId;
            GameSaveFromDb = _context.GameSaves.Find(boardId);
            GameShipFromDb = _context.GameShip.Find(shipId);
            GameConfigFromDb = _context.GameConfigs.Find(GameSaveFromDb.GameConfigId);
            GameShipConfigFromDb = _context.GameShipConfig.Find(GameShipFromDb.GameShipConfigId);
            
            var shipConfigFromDb = _context.GameShipConfig.Find(GameShipFromDb.GameShipConfigId);
            SizeX = shipConfigFromDb.ShipSizeX;
            SizeY = shipConfigFromDb.ShipSizeY;

            Brain = DataBaseAccess.RestoreBrainFromSave((int)boardId);
            Brain.PlayerToMove = (int)player;

            if (GameShipFromDb.Coordinates != null)
            {
                var coorenidates = JsonSerializer.Deserialize<List<Coordinate>>(GameShipFromDb.Coordinates);
                Brain.GameBoards[Brain.PlayerToMove].DeleteShipWithCoordinates(coorenidates!);
            }

            return Page();
        }
        
        [BindProperty]
        public int Player { get; set; }  = default!;
        
        [BindProperty]
        public string CoordinateString { get; set; }  = default!;
        
        [BindProperty]
        public int GameSaveId { get; set; }

        [BindProperty]
        public string shipName { get; set; } = default!;
        
        [BindProperty]
        public bool changeLocation { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {

            var split = CoordinateString.Split("-");
            var baseX = int.Parse(split[0]);
            var baseY = int.Parse(split[1]);
            
            var coordinates = new List<Coordinate>();
            for (int y = 0; y < SizeY; y++)
            {
                for (int x = 0; x < SizeX; x++)
                {
                    coordinates.Add(new Coordinate
                    {
                        X = baseX + x,
                        Y = baseY + y
                    });
                }
            }
            
            GameShipFromDb = _context.GameShip.Find(ShipId);

            GameSaveFromDb = await _context.GameSaves.FindAsync(GameSaveId);
            Brain = DataBaseAccess.RestoreBrainFromSave(GameSaveId);
            Brain.PlayerToMove = (int)Player;
            
            if (GameShipFromDb.Coordinates != null)
            {
                var coorenidates = JsonSerializer.Deserialize<List<Coordinate>>(GameShipFromDb.Coordinates);
                Brain.GameBoards[Brain.PlayerToMove].DeleteShipWithCoordinates(coorenidates!);
            }
            
            var success = Brain.PlaceShip(shipName, coordinates, 0, 0);
            if (!success)
            {
                return RedirectToPage("./ChooseLocation",
                    new {boardId = GameSaveFromDb.GameSaveId, shipId = ShipId, error = "true", player = Player});
            }
            
            var (board1, board2)= Brain.GetGameBoardsAsString();
            GameSaveFromDb.FirstGameBoard = board1;
            GameSaveFromDb.SecondGameBoard = board2;
            _context.GameSaves.Update(GameSaveFromDb);

            var ship = _context.GameShip.Find(ShipId);
            ship.Coordinates = JsonSerializer.Serialize(coordinates);
            ship.IsPlaced = 1;
            _context.GameShip.Update(ship);
            
            await _context.SaveChangesAsync();
            
            return RedirectToPage("./PlaceShips", new {id = GameSaveFromDb.GameSaveId, player = Player});
        }
    }
}