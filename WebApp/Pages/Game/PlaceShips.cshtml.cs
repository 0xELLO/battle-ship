using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BattleShipBrain;
using DAL;
using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages.Game
{
    public class PlaceShips : PageModel
    {
        private readonly DAL.ApplicationDbContext _context;

        public PlaceShips(DAL.ApplicationDbContext context)
        {
            _context = context;
        }

        public Domain.GameSave SaveFromDb { get; set; } = default!;
        public GameConfig GameConfiguration { get; set; } = default!;
        public List<ShipView> GameShipsViews = new List<ShipView>();
        public BSBrain Brain { get; set; } = default!;

        [BindProperty]
        public int SaveId { get; set; }
        
        [BindProperty]
        public int Player { get; set; }
        
        public IActionResult OnGet(int? id, int? player)
        {
            if (id == null || player == null)
            {
                return NotFound();
            }

            Player = (int) player;
            SaveId = (int) id;

            var gameShipsFromDb = _context.GameShip.Where(gs => gs.GameSaveId == id && gs.Player == player).ToList();
            foreach (var gameShip in gameShipsFromDb)
            {
                var relatedConfig = _context.GameShipConfig.Find(gameShip.GameShipConfigId);
                var gameShipView = new ShipView(relatedConfig.ShipName, relatedConfig.ShipSizeX,
                    relatedConfig.ShipSizeY, gameShip.IsPlaced == 1, gameShip.GameShipId);
                GameShipsViews.Add(gameShipView);
            }

            SaveFromDb = _context.GameSaves.Find(id);
            GameConfiguration = _context.GameConfigs.Find(SaveFromDb.GameConfigId);

            Brain = DataBaseAccess.RestoreBrainFromSave((int)id);
            Brain.PlayerToMove = Player;
            
            return Page();
        }
        
        public async Task<IActionResult> OnPostAsync()
        {
            var gameSave = await _context.GameSaves.FindAsync(SaveId);
            gameSave.GamePhase = 2;
            _context.Update(gameSave);
            await _context.SaveChangesAsync();

            if (Player == 0)
            {
                return RedirectToPage("./PlaceShips", new {id = SaveId, player = 1});
            }
            if (Player == 1)
            {
                foreach (var gameShip in _context.GameShip.Where(gs => gs.GameSaveId == SaveId))
                {
                    _context.GameShip.Remove(gameShip);
                }
                await _context.SaveChangesAsync();
                
                return RedirectToPage("./PlayGame", new {id = SaveId});
            }
            return RedirectToPage("./PlaceShips", new {id = SaveId, player = 0});
        }
    }

    public class ShipView
    {
        public readonly string Name;
        public readonly int X;
        public readonly int Y;
        public readonly bool IsPlaced;
        public readonly int id;

        public ShipView(string name, int x, int y, bool isplaced, int id)
        {
            Name = name;
            X = x;
            Y = y;
            IsPlaced = isplaced;
            this.id = id;
        }
    }
}