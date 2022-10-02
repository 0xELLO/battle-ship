using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using BattleShipBrain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using DAL;
using Domain;
using WebApp.Pages_ShipConfiguration;

namespace WebApp.Pages_GameSave
{
    public class IndexModel : PageModel
    {
        private readonly DAL.ApplicationDbContext _context;

        public IndexModel(DAL.ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<GameSave> GameSave { get;set; } = default!;
        public List<string> LocalSaves { get;set; } = default!;

        public async Task OnGetAsync()
        {
            GameSave = await _context.GameSaves.Where(save => save.GamePhase == (int) GamePhase.Started)
                .Include(g => g.GameConfig).ToListAsync();
            var lsa = new LocalStorageAccess(GlobalPath.BattleShipPath);
            LocalSaves = lsa.GetSaveGameFileNames();
        }
        
        [BindProperty]
        public string SaveName { get; set; } = default!;

        public async Task<IActionResult> OnPostAsync()
        {
            var lsa = new LocalStorageAccess(GlobalPath.BattleShipPath);
            var brain = lsa.GetSavedGame(SaveName);
            var config = brain!.CurrentGameConfiguration;
            var (board1, board2) = brain.GetGameBoardsAsString();
            var ships = JsonSerializer.Serialize(brain.GameBoards[0].Ships);
            
            var gameConfig = new GameConfig
            {
                ConfigName = "restored-from-" + SaveName,
                BoardSizeX = config.BoardSizeX,
                BoardSizeY = config.BoardSizeY,
                EShipTouchRule = (int) config.EShipTouchRule,
            };
            
            _context.GameConfigs.Add(gameConfig);
            await _context.SaveChangesAsync();

            var shipConfigs = new HashSet<ShipConfig>();
            foreach (var ship in brain.GameBoards[0].Ships)
            {
                shipConfigs.Add(new ShipConfig
                {
                    Name = ship.Name,
                    Quantity = brain.GameBoards[0].Ships.FindAll(s => s.Name == ship.Name).Count,
                    ShipSizeX = ship.GetShipSize().Item1,
                    ShipSizeY = ship.GetShipSize().Item2
                });
            }

            foreach (var shipConfig in shipConfigs)
            {
                var gameShip = new GameShipConfig
                {
                    ShipName = shipConfig.Name,
                    Quantity = shipConfig.Quantity,
                    ShipSizeX = shipConfig.ShipSizeX,
                    ShipSizeY = shipConfig.ShipSizeY,
                    GameConfigId = gameConfig.GameConfigId,
                };
                _context.GameShipConfig.Add(gameShip);
                await _context.SaveChangesAsync();
            }
            

            var save = new GameSave
            {
                SaveName = SaveName,
                GameCurrentPlayerNumber = brain.PlayerToMove,
                GameMovesNumber = 0,
                FirstGameBoard = board1,
                SecondGameBoard = board2,
                GamePhase = 2,
                GameConfigId = gameConfig.GameConfigId,
            };

            _context.GameSaves.Add(save);
            await _context.SaveChangesAsync();
            
            return RedirectToPage("../Game/PlayGame", new {id = save.GameSaveId});
        }
    } 
    
}
