using System;
using System.Collections.Generic;
using System.Linq;
using BattleShipBrain;
using BattleShipConsoleUI;
using DAL;

namespace BattleShip
{
    static class Program
    {
        private static BSBrain? Brain;
        private static GameConfiguration _gameConfiguration = new GameConfiguration();
        // base path. Updated in Main(), path information provided by program arguments
        private static string _basePath = "";

        static void Main(string[] args)
        {
            try
            {
                _basePath = args[0];
            }
            catch (Exception e)
            {
                Console.WriteLine("Add path to current working directory to program variable. Example C:/Users/USER/Desktop/BattleShip");
                throw;
            }
            
            Brain = new BSBrain(_gameConfiguration);
            try
            {
                DataBaseAccess.CreateDefaultConfig();
                MainMenu();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Console.ReadLine();
                throw;
            }
        }

        // HELP METHODS
        private static string ApplyConfigFromLocal(string fileName)
        {
            var lsa = new LocalStorageAccess(_basePath);
            
            _gameConfiguration = lsa.GetConfiguration(fileName)!;
            _gameConfiguration.Name = fileName;
            return ChooseGameModeMenu();
        }
        
        private static string ApplyConfigFromDb(string name)
        {
            _gameConfiguration = DataBaseAccess.GetConfiguration(name)!;
            _gameConfiguration.Name = name;
            return ChooseGameModeMenu();
        }

        private static string CreteNewConfig()
        {
            var conf = new ConfigurationUi();
            var output = conf.RunConfiguration();
            if (output == null) return "";
            _gameConfiguration = output;

            var fileName = "";
            var saveLocation = ActionWindowsUi.AskSaveLocation();
            
            if (saveLocation != SaveOptions.DontSave)
            {
                fileName = ActionWindowsUi.AskSingleInput("Configuration name",
                Validation.ValidateConfigurationFileName, _basePath);
                _gameConfiguration.Name = fileName;
            }
            if (saveLocation is SaveOptions.SaveToDataBase or SaveOptions.SaveToLocalAndDataBase)
            {
                DataBaseAccess.SaveConfiguration(fileName, _gameConfiguration);
            }
            if (saveLocation is SaveOptions.SaveToLocalStorage or SaveOptions.SaveToLocalAndDataBase)
            {
                var las = new LocalStorageAccess(_basePath);
                las.SaveConfiguration(fileName, _gameConfiguration);
            }
            return ChooseGameModeMenu();;
        }

        private static string RunRandomGame()
        {
            Console.Clear();
            Brain = new BSBrain(_gameConfiguration);
            Brain.CreateRandomBoards();
            Brain.GamePhase = GamePhase.Started;
            var battleShipUi = new BoardUI(Brain.GetCurrentPlayerBoard, Brain.GetOppositePlayerBoard,
                Brain.PlaceBomb, (_gameConfiguration.BoardSizeX, _gameConfiguration.BoardSizeY), SaveGame,
                Brain.GetPlayerN, Brain.GetMoveN, Brain.CheckIfPlayerWon);
            battleShipUi.RunUI();
            return "";
        }

        private static string RunDefaultGame()
        {
            Brain!.GamePhase = GamePhase.ShipPlacement;
            Console.Clear();
            Brain = new BSBrain(_gameConfiguration);
            RunPlaceShips();
            Brain.GamePhase = GamePhase.Started;
            var battleShipUi = new BoardUI(Brain.GetCurrentPlayerBoard, Brain.GetOppositePlayerBoard,
                Brain.PlaceBomb, (_gameConfiguration.BoardSizeX, _gameConfiguration.BoardSizeY), SaveGame,
                Brain.GetPlayerN, Brain.GetMoveN, Brain.CheckIfPlayerWon);
            battleShipUi.RunUI();
            return "";
        }

        private static void RunPlaceShips()
        {
            var a = new PlaceShipsUI(Brain!.GetCurrentPlayerBoard, _gameConfiguration, Brain!.PlaceShip,
                Brain!.PlayerToMove, Brain.GameBoards[0].DeleteLastShip);
            a.RunUi();
            Brain.SwitchPlayer();
            var b = new PlaceShipsUI(Brain.GetCurrentPlayerBoard, _gameConfiguration, Brain!.PlaceShip,
                Brain.PlayerToMove, Brain.GameBoards[1].DeleteLastShip);
            b.RunUi();
            Brain.SwitchPlayer();
        }

        private static void SaveGame()
        {
            var lsa = new LocalStorageAccess(_basePath);
            switch (Brain!.GamePhase)
            {
                case GamePhase.Started:
                    var saveLocation = ActionWindowsUi.AskSaveLocation();
                    var name = "save-game-" + DateTime.Now.ToString("MM/dd/yyyy-H-mm");
                    if (saveLocation is SaveOptions.SaveToDataBase or SaveOptions.SaveToLocalAndDataBase)
                    {
                        DataBaseAccess.SaveGame(Brain, name);
                        var gameConfiguration = DataBaseAccess.GetConfiguration(Brain.CurrentGameConfiguration.Name);
                        if (gameConfiguration == null)
                        {
                            DataBaseAccess.SaveConfiguration(Brain.CurrentGameConfiguration.Name, Brain.CurrentGameConfiguration);
                        }
                        DataBaseAccess.ConnectSaveWithConfiguration(name, Brain.CurrentGameConfiguration.Name);
                    }

                    if (saveLocation is SaveOptions.SaveToLocalStorage or SaveOptions.SaveToLocalAndDataBase)
                    {
                        lsa.SaveGame(name, Brain);
                    }
                    break;
                case GamePhase.ResumedFromLocal:
                    lsa.UpdateGame(Brain);
                    break;
                case GamePhase.ResumedFromDb:
                    DataBaseAccess.UpdateGame(Brain);
                    break;
            }

        }

        private static string ApplySaveFromLocal(string name)
        {
            var lsa = new LocalStorageAccess(_basePath);
            var save = lsa.GetSavedGame(name);

            Brain = save;
            Brain!.GamePhase = GamePhase.ResumedFromLocal;
            var battleShipUi = new BoardUI(Brain.GetCurrentPlayerBoard, Brain.GetOppositePlayerBoard,
                Brain.PlaceBomb, (_gameConfiguration.BoardSizeX, _gameConfiguration.BoardSizeY), SaveGame,
                Brain.GetPlayerN, Brain.GetMoveN, Brain.CheckIfPlayerWon);
            battleShipUi.RunUI();
            return "";
        }

        private static string ApplySaveFromDb(string name)
        {
            var save = DataBaseAccess.GetSave(name);

            Brain = save;
            Brain.GamePhase = GamePhase.ResumedFromDb;
            
            var battleShipUi = new BoardUI(Brain.GetCurrentPlayerBoard, Brain.GetOppositePlayerBoard,
                Brain.PlaceBomb, (_gameConfiguration.BoardSizeX, _gameConfiguration.BoardSizeY), SaveGame,
                Brain.GetPlayerN, Brain.GetMoveN, Brain.CheckIfPlayerWon);
            battleShipUi.RunUI();
            
            return "";
        }
        
        // MENUS
        private static string MainMenu()
        {
            var menu = new Menu("Main Menu", EMenuLevel.Root);
            menu.AddMenuItems(new List<MenuItem>
                {
                new MenuItem("Start new game", GameConfigurationMenu),
                new MenuItem("Load game", BrowseSavedGamesLocationOptionsMenu)
            }
            );
            return menu.Run();
        }

        private static string ChooseGameModeMenu()
        {
            var menu = new Menu("Game configuration setting", EMenuLevel.SecondOrMore);
            menu.AddMenuItems(new List<MenuItem>
                {
                    new MenuItem("Place ships yourself", RunDefaultGame),
                    new MenuItem("Generate random board", RunRandomGame),
                }
            );
            return menu.Run();
        }
        
        private static string BrowseSavedConfigurationLocationOptions()
        {
            var menu = new Menu("Saved configurations", EMenuLevel.SecondOrMore);
            menu.AddMenuItems(new List<MenuItem>
                {
                    new MenuItem("Show local saves", LoadLocalConfigurationsMenu),
                    new MenuItem("Show cloud saves", LoadDbConfigurationsMenu),
                }
            );
            return menu.Run();
        }
        
        private static string LoadDbConfigurationsMenu()
        {
            var menu = new Menu("Game Configuration menu", EMenuLevel.SecondOrMore);
            using var db = new ApplicationDbContext();
            var fileNames = DataBaseAccess.GetConfigurationFileNames();
            var configItems = fileNames.Select(fileName => new MenuItem(fileName, ApplyConfigFromDb)).ToList();
            menu.AddMenuItems(configItems);
            return menu.Run();
        }
        
        private static string LoadLocalConfigurationsMenu()
        {
            var menu = new Menu("Local saves", EMenuLevel.SecondOrMore);
            var lsa = new LocalStorageAccess(_basePath);
            var fileNames = lsa.GetConfigurationFileNames();
            var configItems = fileNames.Select(fileName => new MenuItem(fileName, ApplyConfigFromLocal)).ToList();
            menu.AddMenuItems(configItems);
            return menu.Run();
        }

        private static string GameConfigurationMenu()
        {
            var menu = new Menu("Game configuration setting", EMenuLevel.First);
            menu.AddMenuItems(new List<MenuItem>
                {
                    new MenuItem("Crate new configuration", CreteNewConfig),
                    new MenuItem("Browse saved configurations", BrowseSavedConfigurationLocationOptions),
                    new MenuItem("Play with default configuration", _ =>
                    {
                        _gameConfiguration = new GameConfiguration(); return ChooseGameModeMenu();
                    })
                }
            );
            return menu.Run();
        }
        
        private static string BrowseSavedGamesLocationOptionsMenu()
        {
            var menu = new Menu("Saved games", EMenuLevel.First);
            menu.AddMenuItems(new List<MenuItem>
                {
                    new MenuItem("Show local saves", LoadLocalSavedGamesMenu),
                    new MenuItem("Show cloud saves", LoadDataBaseSavedGamesMenu),
                }
            );
            return menu.Run();
        }

        private static string LoadLocalSavedGamesMenu()
        {
            var menu = new Menu("Local saves", EMenuLevel.SecondOrMore);
            var fileNames = GameConfigurationOptions.GetSaveNames(_basePath);
            var configItems = fileNames.Select(fileName => new MenuItem(fileName, ApplySaveFromLocal)).ToList();
            menu.AddMenuItems(configItems);
            return menu.Run();
        }

        private static string LoadDataBaseSavedGamesMenu()
        {
            var menu = new Menu("Cloud saves", EMenuLevel.SecondOrMore);
            using var db = new ApplicationDbContext();
            var gameSaves = db.GameSaves.Where(save => save.GamePhase == (int) GamePhase.Started);
            var fileNames = gameSaves.Select(entry => entry.SaveName).ToList();
            var configItems = fileNames.Select(fileName => new MenuItem(fileName, ApplySaveFromDb)).ToList();
            menu.AddMenuItems(configItems);
            return menu.Run();
        }
    }
}
