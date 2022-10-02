using System;
using System.Threading.Tasks;
using BattleShipBrain;
using DAL;
using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages.Game
{
    public class PlayGame : PageModel
    {
        private readonly DAL.ApplicationDbContext _context;
        
        public PlayGame(DAL.ApplicationDbContext context)
        {
            _context = context;
        }
        
        [BindProperty]
        public int GameSaveId { get; set; } = default!;
        public GameConfig GameConfigFromDb { get; set; } = default!;
        
        public BoardSquareState[,] CurrentPlayerBoard { get; set; } = default!;
        public BoardSquareState[,] OppositePlayerBoard { get; set; } = default!;
        public BSBrain Brain { get; set; } = default!;

        public IActionResult OnGet(int? id)
        {
            GameSaveId = (int) id!;
            var gameSave = _context.GameSaves.Find(id);
            GameConfigFromDb = _context.GameConfigs.Find(gameSave.GameConfigId);
            Brain = DataBaseAccess.RestoreBrainFromSave((int) id!);
            CurrentPlayerBoard = Brain.GetCurrentPlayerBoard();
            OppositePlayerBoard = Brain.GetOppositePlayerBoard();

            return Page();
        }
        
        [BindProperty]
        public string CoordinateString { get; set; } = default!;

        public async Task<IActionResult> OnPostAsync()
        {
            var gameSave = await _context.GameSaves.FindAsync(GameSaveId);
            Brain = DataBaseAccess.RestoreBrainFromSave(GameSaveId);
            
            var split = CoordinateString.Split("-");
            var baseX = int.Parse(split[0]);
            var baseY = int.Parse(split[1]);
            
            Brain.PlaceBomb(baseX, baseY);
            
            var (board1, board2) = Brain.GetGameBoardsAsString();

            gameSave.FirstGameBoard = board1;
            gameSave.SecondGameBoard = board2;
            gameSave.GameCurrentPlayerNumber = Brain.PlayerToMove;
            gameSave.GameMovesNumber += 1;
            _context.GameSaves.Update(gameSave);
            await _context.SaveChangesAsync();
            
            Brain.SwitchPlayer();
            if (Brain.CheckIfPlayerWon())
            {
                gameSave.GamePhase = 5;
                _context.GameSaves.Update(gameSave);
                await _context.SaveChangesAsync();
                return RedirectToPage("./WinPage", new {player = Brain.PlayerToMove});
            }

            return RedirectToPage("./PlayGame", new {id = GameSaveId});
        }
    }
}