using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BattleShipBrain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using DAL;
using Domain;
using WebApp.Pages_ShipConfiguration;

namespace WebApp.Pages_GameConfiguration
{
    public class IndexModel : PageModel
    {
        private readonly DAL.ApplicationDbContext _context;

        public IndexModel(DAL.ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<GameConfig> GameConfig { get;set; }  = default!;
        public List<string> LocalGameConfig { get;set; }  = default!;
        
        public async Task OnGetAsync()
        {
            var lsa = new LocalStorageAccess(GlobalPath.BattleShipPath);
            LocalGameConfig = new List<string>(lsa.GetConfigurationFileNames());
            GameConfig = await _context.GameConfigs.ToListAsync();
        }

        public async Task<bool> IsInUse(int id)
        {
            return _context.GameSaves.Any(gs => gs.GameConfigId == id);
        }

        [BindProperty]
        public string ConfigName { get; set; } = default!;
        
        public async Task<IActionResult> OnPostSaveToDb()
        {
            var lsa = new LocalStorageAccess(GlobalPath.BattleShipPath);
            var config = lsa.GetConfiguration(ConfigName);
            var game = _context.GameConfigs.Add(new GameConfig
            {
                ConfigName = ConfigName,
                BoardSizeX = config!.BoardSizeX,
                BoardSizeY = config!.BoardSizeY,
                EShipTouchRule = (int) config.EShipTouchRule,
            });

            await _context.SaveChangesAsync();
            
            foreach (var shipConfig in config.ShipConfigs)
            {
                _context.GameShipConfig.Add(new GameShipConfig
                {
                    ShipName = shipConfig.Name,
                    Quantity = shipConfig.Quantity,
                    ShipSizeX = shipConfig.ShipSizeX,
                    ShipSizeY = shipConfig.ShipSizeY,
                    GameConfigId = game.Entity.GameConfigId,
                });
            }
            
            await _context.SaveChangesAsync();

            return RedirectToPage("");
        }
        [BindProperty]
        public int GameConfigId { get; set; } = default!;
        public async Task<IActionResult> OnPostUploadToLocal()
        {
            var configurationFromDb = await _context.GameConfigs.FindAsync(GameConfigId);
            var lsa = new LocalStorageAccess(GlobalPath.BattleShipPath);

            var shipConfigs = new List<ShipConfig>();
            foreach (var gameShipConfig in  _context.GameShipConfig.Where(gsc => gsc.GameConfigId == configurationFromDb.GameConfigId))
            {
                shipConfigs.Add(new ShipConfig
                {
                    Name = gameShipConfig.ShipName,
                    Quantity = gameShipConfig.Quantity,
                    ShipSizeX = gameShipConfig.ShipSizeX,
                    ShipSizeY = gameShipConfig.ShipSizeY
                });
            }

            var gameConfiguration = new GameConfiguration
            {
                BoardSizeX = configurationFromDb.BoardSizeX,
                BoardSizeY = configurationFromDb.BoardSizeY,
                EShipTouchRule = (EShipTouchRule) configurationFromDb.EShipTouchRule,
                Name = configurationFromDb.ConfigName,
                ShipConfigs = shipConfigs
            };
            
            lsa.SaveConfiguration(configurationFromDb.ConfigName, gameConfiguration);
            return RedirectToPage("./Index");
        }
    }
}
