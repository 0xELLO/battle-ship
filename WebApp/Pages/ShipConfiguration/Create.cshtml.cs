using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using DAL;
using Domain;

namespace WebApp.Pages_ShipConfiguration
{
    public class CreateModel : PageModel
    {
        private readonly DAL.ApplicationDbContext _context;

        public CreateModel(DAL.ApplicationDbContext context)
        {
            _context = context;
        }

        public int GameConfigId { get; set; }

        public IActionResult OnGet(int? id)
        {
    
            if (id == null)
            {
                return NotFound();
            }

            GameConfigId = (int) id;
            ViewData["GameConfigId"] = new SelectList(_context.GameConfigs, "GameConfigId", "ConfigName");
                return Page();
        }

        [BindProperty]
        public GameShipConfig GameShipConfig { get; set; }  = default!;

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.GameShipConfig.Add(GameShipConfig);
            await _context.SaveChangesAsync();

            return RedirectToPage("../GameConfiguration/Edit", new {id = GameShipConfig.GameConfigId});
        }
    }
}
