using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using DAL;
using Domain;

namespace WebApp.Pages_GameConfiguration
{
    public class CreateModel : PageModel
    {
        private readonly DAL.ApplicationDbContext _context;

        public CreateModel(DAL.ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty] public GameConfig GameConfig { get; set; } = default!;

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.GameConfigs.Add(GameConfig);
            await _context.SaveChangesAsync();

            return RedirectToPage("../GameConfiguration/Edit", new {id = GameConfig.GameConfigId});
        }
        [HttpPost]
        public IActionResult Index(ShipTouchRuleViewModel model)
        {
            return Content(model.EShipTouchRule.ToString());
        }
    }
    public class ShipTouchRuleViewModel
    {
        public BattleShipBrain.EShipTouchRule EShipTouchRule { get; set; }
    }
}
