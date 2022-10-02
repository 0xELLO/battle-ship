using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DAL;
using Domain;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace WebApp.Pages_ShipConfiguration
{
    public class EditModel : PageModel
    {
        private readonly DAL.ApplicationDbContext _context;

        public EditModel(DAL.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public GameShipConfig GameShipConfig { get; set; }  = default!;
        [BindProperty]
        public int GameConfigId { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            GameShipConfig = await _context.GameShipConfig
                .Include(g => g.GameConfig).FirstOrDefaultAsync(m => m.GameShipConfigId == id);

            if (GameShipConfig == null)
            {
                return NotFound();
            }

            GameConfigId = (int) id;
            ViewData["GameConfigId"] = new SelectList(_context.GameConfigs, "GameConfigId", "ConfigName");
            var gameConfig = _context.GameConfigs.Where(config => config.GameConfigId == GameShipConfig.GameConfigId);
            GameConfigId = gameConfig.First().GameConfigId;
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

            _context.Attach(GameShipConfig).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GameShipConfigExists(GameShipConfig.GameShipConfigId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("../GameConfiguration/Edit", new {id = GameConfigId});
        }

        private bool GameShipConfigExists(int id)
        {
            return _context.GameShipConfig.Any(e => e.GameShipConfigId == id);
        }
    }
}
