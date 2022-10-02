using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using DAL;
using Domain;

namespace WebApp.Pages_GameConfiguration
{
    public class DeleteModel : PageModel
    {
        private readonly DAL.ApplicationDbContext _context;

        public DeleteModel(DAL.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public GameConfig GameConfig { get; set; }  = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            GameConfig = await _context.GameConfigs.FirstOrDefaultAsync(m => m.GameConfigId == id);

            if (GameConfig == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            GameConfig = await _context.GameConfigs.FindAsync(id);

            if (GameConfig != null)
            {
                var gameShipConfigs = _context.GameShipConfig.Where(gsc => gsc.GameConfigId == GameConfig.GameConfigId);

                foreach (var gameShipConfig in gameShipConfigs)
                {
                    _context.GameShipConfig.Remove(gameShipConfig);
                }

                await _context.SaveChangesAsync();
                
                _context.GameConfigs.Remove(GameConfig);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
