using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BattleShipBrain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DAL;
using Domain;

namespace WebApp.Pages_GameConfiguration
{
    public class EditModel : PageModel
    {
        private readonly DAL.ApplicationDbContext _context;

        public EditModel(DAL.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public GameConfig GameConfig { get; set; }  = default!;
        public List<GameShipConfig> GameShipConfigs { get; set; }  = default!;
        
        public bool IsError { get; set; } = false;

        public async Task<IActionResult> OnGetAsync(int? id, string? message)
        {
            if (id == null)
            {
                return NotFound();
            }
            
            if (message == "error")
            {
                IsError = true;
            }

            GameConfig = await _context.GameConfigs.FirstOrDefaultAsync(m => m.GameConfigId == id);
            GameShipConfigs = _context.GameShipConfig.Where(m => m.GameConfigId == id).ToList();
            
            if (GameConfig == null)
            {
                return NotFound();
            }
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (GameConfig.BoardSizeX < 3 && GameConfig.BoardSizeY < 3)
            {
                GameConfig.Complete = 0;
                _context.Update(GameConfig);
                await _context.SaveChangesAsync();
                return RedirectToPage("/", new {id = GameConfig.GameConfigId, message = "error"});
            }

            var gameConfiguration = new GameConfiguration
            {
                BoardSizeX = GameConfig.BoardSizeX,
                BoardSizeY = GameConfig.BoardSizeY,
                EShipTouchRule = (EShipTouchRule) GameConfig.EShipTouchRule,
                ShipConfigs = new List<ShipConfig>()
            };

            var shipConfigsFromDb = _context.GameShipConfig.Where(gsc => gsc.GameConfigId == GameConfig.GameConfigId);
            foreach (var gameShipConfig in shipConfigsFromDb)
            {
                gameConfiguration.ShipConfigs.Add(new ShipConfig
                {
                    Name = gameShipConfig.ShipName,
                    Quantity = gameShipConfig.Quantity,
                    ShipSizeY = gameShipConfig.ShipSizeY,
                    ShipSizeX = gameShipConfig.ShipSizeX
                });
            }

            if (!gameConfiguration.ControlIfConfigurationIsPossible())
            {
                GameConfig.Complete = 0;
                _context.Update(GameConfig);
                await _context.SaveChangesAsync();
                return RedirectToPage("", new {id = GameConfig.GameConfigId, message = "error"});
            }

            GameConfig.Complete = 1;
            _context.Attach(GameConfig).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GameConfigExists(GameConfig.GameConfigId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }
        
        [HttpPost]
        public IActionResult Index(ShipTouchRuleViewModel model)
        {
            return Content(model.EShipTouchRule.ToString());
        }

        private bool GameConfigExists(int id)
        {
            return _context.GameConfigs.Any(e => e.GameConfigId == id);
        }
    }
}
