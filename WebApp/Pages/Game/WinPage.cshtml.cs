using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages.Game
{
    public class WinPage : PageModel
    {
        public int Player { get; set; }
        public void OnGet(int player)
        {
            Player = player;
        }
    }
}