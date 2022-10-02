using System;
using System.Collections.Generic;
using BattleShipBrain;
using DAL;
using MenuSystem;

namespace BattleShipConsoleUI
{
    public static class ActionWindowsUi
    {
        public static void WinUi(int playerN, int moves)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Black;
            Console.WriteLine("Congratulations!!!");
            Console.WriteLine("Player {0} won the game", playerN);
            Console.WriteLine("Total moves: {0}", moves);
            OkButton();
        }

        public static void RunSaving()
        {
            Console.Clear();
            Console.WriteLine("Game Saved");
            OkButton();
        }

        private static void OkButton()
        {
            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.WriteLine("  OK  ");
            Console.ReadKey(true);
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
        }
        public static SaveOptions AskSaveLocation()
        {
            var menu = new ConfigurationMenuSystem("Choose where to save configuration",
                EConfiguraitonMenuType.Vertical);
            var res = SaveOptions.DontSave;
            menu.StaticMenuItems = new List<MenuItem>
            {
                new MenuItem("Local storage", delegate (){ res = SaveOptions.SaveToLocalStorage; return"";}),
                new MenuItem("Cloud storage", delegate() { res = SaveOptions.SaveToDataBase; return"";}),
                new MenuItem("Both", delegate (){ res = SaveOptions.SaveToLocalAndDataBase; return"";}),
                new MenuItem("Don't save", delegate (){ res = SaveOptions.DontSave; return"";})
            };
            menu.RunStandartMenu();
            return res;
        }

        public static string AskSingleInput(string inputName, Func<string, bool> validation, string path)
        {
            var done = false;
            var error = Error.NoError;
            var lsa = new LocalStorageAccess(path);
            string? input;
            do
            {
                Console.Clear();
                if (error != Error.NoError)
                {
                    Console.BackgroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine(error);
                    Console.BackgroundColor = ConsoleColor.Black;
                }
                
                Console.Write(inputName+  ": ");
                input = Console.ReadLine();
                
                if (!Validation.ValidateConfigurationFileName(input))
                {
                    error = Error.WrongFileName;
                    continue;
                }

                if (lsa.GetConfigurationFileNames().Contains(input!))
                {
                    error = Error.ConfigurationAlreadyExists;
                    continue;
                }
                done = true;
            } while (!done);
            return input!;
        }
    }
}