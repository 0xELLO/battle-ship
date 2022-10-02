using BattleShipBrain;
using DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApp.Pages_ShipConfiguration;

namespace WebApp.Pages.GameSave
{
    public class SaveLocal : PageModel
    {
        
        private readonly DAL.ApplicationDbContext _context;

        public SaveLocal(DAL.ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet(int id)
        {
            var gameSave = _context.GameSaves.Find(id);
            var lsa = new LocalStorageAccess(GlobalPath.BattleShipPath);
            var brain = DataBaseAccess.RestoreBrainFromSave(id);
            var name = "from-database-" + gameSave.SaveName;
            lsa.SaveGame(name, brain);
            return RedirectToPage("./Index");
        }
    }
}