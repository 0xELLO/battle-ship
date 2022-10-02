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
    public class DetailsModel : PageModel
    {
        private readonly DAL.ApplicationDbContext _context;

        public DetailsModel(DAL.ApplicationDbContext context)
        {
            _context = context;
        }

        public GameConfig GameConfig { get; set; }  = default!;
        public List<GameShipConfig> GameShipConfigs { get; set; }  = default!;
        

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            GameConfig = await _context.GameConfigs.FirstOrDefaultAsync(m => m.GameConfigId == id);
            GameShipConfigs = _context.GameShipConfig.Where(m => m.GameConfigId == id).ToList();
            

            if (GameConfig == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}
