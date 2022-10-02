using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using BattleShipBrain;

namespace BattleShip
{
    public static class GameConfigurationOptions
    {
        public static GameConfiguration LoadConfig(string path, string filename, GameConfiguration configuration)
        {
            var pathToFile = path + Path.DirectorySeparatorChar + "Configs"
                             + Path.DirectorySeparatorChar + filename;

            var pathToFolder = path + Path.DirectorySeparatorChar + "Configs";

            var jsonOptions = new JsonSerializerOptions {WriteIndented = true};
            
            var confJsonStr = JsonSerializer.Serialize(configuration, jsonOptions);

            if (!Directory.Exists(pathToFolder)) Directory.CreateDirectory(pathToFolder);
            

            if (!File.Exists(pathToFile))
            {
                File.WriteAllText(pathToFile, confJsonStr);
            }
            else
            {
                var confText = File.ReadAllText(pathToFile);
                configuration = JsonSerializer.Deserialize<GameConfiguration>(confText)!;
            }

            return configuration;
        }

        public static GameConfiguration GetConfig(string path, string filename)
        {
            var pathToFile = path + Path.DirectorySeparatorChar + "Configs"
                             + Path.DirectorySeparatorChar + filename;

            var pathToFolder = path + Path.DirectorySeparatorChar + "Configs";
            
            
            var confText = File.ReadAllText(pathToFile);
            var configuration = JsonSerializer.Deserialize<GameConfiguration>(confText)!;

            return configuration;
        }
        

        public static void UploadSave(string path, string filename, String saveJsonStr)
        {
            var pathToFile = path + Path.DirectorySeparatorChar + "Saves"
                             + Path.DirectorySeparatorChar + filename;

            var pathToFolder = path + Path.DirectorySeparatorChar + "Saves";
            
            if (!Directory.Exists(pathToFolder)) Directory.CreateDirectory(pathToFolder);
            
            if (!File.Exists(pathToFile))
            {
                File.WriteAllText(pathToFile, saveJsonStr);
            }
        }

        public static string GetSave(string path, string filename)
        {
            var saveText = "";
            
            var pathToFile = path + Path.DirectorySeparatorChar + "Saves"
                             + Path.DirectorySeparatorChar + filename;

            var pathToFolder = path + Path.DirectorySeparatorChar + "Saves";
            
            if (!Directory.Exists(pathToFolder)) Directory.CreateDirectory(pathToFolder);
            
            if (File.Exists(pathToFile))
            {
                saveText = File.ReadAllText(pathToFile);
            }

            return saveText;
        }


        public static List<string> GetSaveNames(string path)
        {
            var directory = new DirectoryInfo(path + Path.DirectorySeparatorChar + "Saves");

            return GetFiles(directory);
        }


        public static List<string> GetConfigNames(string path)
        {
            var directory = new DirectoryInfo(path + Path.DirectorySeparatorChar + "Configs");

            return GetFiles(directory);
        }

        private static List<string> GetFiles(DirectoryInfo dir)
        {
            var files = dir.GetFiles("*.json");

            return files.Select(file => Path.GetFileNameWithoutExtension(file.ToString())).ToList();
        }
    }
}