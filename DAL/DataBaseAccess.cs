using System.Collections.Generic;
using System.Linq;
using BattleShipBrain;
using Domain;

namespace DAL
{
    public static class DataBaseAccess
    {
        public static void SaveConfiguration(string name, GameConfiguration gameConfiguration)
        {
            using var db = new ApplicationDbContext();
            
            var gameConfig = new GameConfig()
            {
                ConfigName = name,
                EShipTouchRule = (int) gameConfiguration.EShipTouchRule,
                BoardSizeX = gameConfiguration.BoardSizeX,
                BoardSizeY = gameConfiguration.BoardSizeY,
            };
            db.GameConfigs.Add(gameConfig);

            foreach (var shipConfig in gameConfiguration.ShipConfigs)
            {
                var gameShipConfig = new GameShipConfig()
                {
                    ShipName = CheckSaveFileName(shipConfig.Name),
                    Quantity = shipConfig.Quantity,
                    ShipSizeX = shipConfig.ShipSizeX,
                    ShipSizeY = shipConfig.ShipSizeY,
                    GameConfig = gameConfig
                };
                db.GameShipConfig.Add(gameShipConfig);

            }
            db.SaveChanges();
        }

        public static void DeleteAllUnessasary()
        {
            using var db = new ApplicationDbContext();
            foreach (var gameShip in db.GameShip)
            {
                db.GameShip.Remove(gameShip);
            }

            db.SaveChanges();

            foreach (var gameSave in db.GameSaves)
            {
                db.GameSaves.Remove(gameSave);
            }
            db.SaveChanges();
        }

        public static void CreateDefaultConfig()
        {
            using var db = new ApplicationDbContext();
            if (!db.GameConfigs.Any(config => config.ConfigName == "Default"))
            {
                var gameconfig = new GameConfiguration();
                var gameconfDb = db.GameConfigs.Add(new GameConfig
                {
                    GameConfigId = 0,
                    ConfigName = "Default",
                    BoardSizeX = gameconfig.BoardSizeX,
                    BoardSizeY = gameconfig.BoardSizeY,
                    EShipTouchRule = (int )gameconfig.EShipTouchRule,
                    Complete = 1,
                    GameSaves = null
                });
                db.SaveChanges();
                foreach (var shipConfig in gameconfig.ShipConfigs)
                {
                    db.GameShipConfig.Add(new GameShipConfig
                    {
                        ShipName = shipConfig.Name,
                        Quantity = shipConfig.Quantity,
                        ShipSizeX = shipConfig.ShipSizeX,
                        ShipSizeY = shipConfig.ShipSizeY,
                        GameConfigId = gameconfDb.Entity.GameConfigId,
                    });
                }

                db.SaveChanges();
            }
        }

        public static GameConfiguration? GetConfiguration(string name)
        {
            using var db = new ApplicationDbContext();
            var gameConfigFromDb = db.GameConfigs.FirstOrDefault(gc => gc.ConfigName == name);
            if (gameConfigFromDb == null) return null;

            var shipConfigFromDb = db.GameShipConfig.Where
                (sc => sc.GameConfigId == gameConfigFromDb.GameConfigId);
            
            var shipConfigurations = new List<ShipConfig>();
            foreach (var sc in shipConfigFromDb)
            {
                shipConfigurations.Add(new ShipConfig
                {
                    Name = sc.ShipName,
                    Quantity = sc.Quantity,
                    ShipSizeX = sc.ShipSizeX,
                    ShipSizeY = sc.ShipSizeY
                });
            } 
            
            return new GameConfiguration
            {
                BoardSizeX = gameConfigFromDb.BoardSizeX,
                BoardSizeY = gameConfigFromDb.BoardSizeY,
                EShipTouchRule = (EShipTouchRule) gameConfigFromDb.EShipTouchRule,
                ShipConfigs = shipConfigurations
            };
        }

        public static List<string> GetConfigurationFileNames()
        {
            using var db = new ApplicationDbContext();
            return db.GameConfigs.Select(gc => gc.ConfigName).ToList();
        }

        public static void SaveGame(BSBrain brain, string name)
        {
            var (board1, board2) = brain.GetGameBoardsAsString();
            using var db = new ApplicationDbContext();
            
            var gameSave = new GameSave
            {
                SaveName = name,
                GameCurrentPlayerNumber = brain.PlayerToMove,
                GameMovesNumber = brain.MoveNumber,
                FirstGameBoard = board1,
                SecondGameBoard = board2,
                GamePhase = (int) GamePhase.Started
            };
            db.GameSaves.Add(gameSave);
            db.SaveChanges();
        }

        public static string CheckSaveFileName(string filename)
        {
            var tempFileName = filename.Clone().ToString();
            if (ConfigurationNameExists(filename))
            {
                filename += "-1";
                if (ConfigurationNameExists(filename))
                {
                    var i = 2;
                    while (ConfigurationNameExists(filename))
                    {
                        filename = (string) tempFileName!.Clone();
                        filename += "-" + i;
                        i++;
                    }
                }
            }
            return filename;
        }

        public static bool ConfigurationNameExists(string filename)
        {
            var files = GetConfigurationFileNames();
            return files.All(save => save == filename);
        }

        public static void ConnectSaveWithConfiguration(string saveName, string configName)
        {
            using var db = new ApplicationDbContext();
            var save = db.GameSaves.Where(gameSave => gameSave.SaveName == saveName);
            var config = db.GameConfigs.Where(gameConfig => gameConfig.ConfigName == configName);
            save.First().GameConfigId = config.First().GameConfigId;
            db.GameSaves.Update(save.First());
            db.SaveChanges();
        }

        public static BSBrain GetSave(string name)
        {
            using var db = new ApplicationDbContext();
            var gameSaveFromDb = db.GameSaves.FirstOrDefault(gs => gs.SaveName == name);
            var brain = new BSBrain(new GameConfiguration());
            brain.GameName = name;
            brain.MoveNumber = gameSaveFromDb!.GameMovesNumber;
            brain.PlayerToMove = gameSaveFromDb!.GameCurrentPlayerNumber;
            brain.RestoreBoardsFromString(gameSaveFromDb.FirstGameBoard, gameSaveFromDb.SecondGameBoard);
            return brain;
        }

        public static void UpdateGame(BSBrain brain)
        {
            using var db = new ApplicationDbContext();
            var gameSave = db.GameSaves.FirstOrDefault(gs => gs.SaveName == brain.GameName);
            var (board1, board2) = brain.GetGameBoardsAsString();

            gameSave!.GameCurrentPlayerNumber = brain.PlayerToMove;
            gameSave.GameMovesNumber = brain.MoveNumber;
            gameSave.FirstGameBoard = board1;
            gameSave.SecondGameBoard = board2;
            db.SaveChanges();
        }

        public static BSBrain RestoreBrainFromSave(int saveId)
        {
            using var db = new ApplicationDbContext();
            var gameSaveFromDb = db.GameSaves.Find(saveId);
            var gameConfigurationFromDb = db.GameConfigs.Find(gameSaveFromDb.GameConfigId);
            
            var shipConfigs = new List<ShipConfig>();
            foreach (var gameShipConfig in  db.GameShipConfig.Where(gsc => gsc.GameConfigId == gameConfigurationFromDb.GameConfigId))
            {
                shipConfigs.Add(new ShipConfig
                {
                    Name = gameShipConfig.ShipName,
                    Quantity = gameShipConfig.Quantity,
                    ShipSizeX = gameShipConfig.ShipSizeX,
                    ShipSizeY = gameShipConfig.ShipSizeY
                });
            }
            
            var gameConfiguration = new GameConfiguration
            {
                BoardSizeX = gameConfigurationFromDb.BoardSizeX,
                BoardSizeY = gameConfigurationFromDb.BoardSizeY,
                EShipTouchRule = (EShipTouchRule) gameConfigurationFromDb.EShipTouchRule,
                ShipConfigs = shipConfigs
            };

            var board1 = gameSaveFromDb.FirstGameBoard;
            var board2 = gameSaveFromDb.SecondGameBoard;
            var brain = new BSBrain(gameConfiguration);
            brain.RestoreBoardsFromString(board1, board2);
            brain.PlayerToMove = gameSaveFromDb.GameCurrentPlayerNumber;
            brain.MoveNumber = gameSaveFromDb.GameMovesNumber;
            return brain;
        }
    }
}