using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using BattleShipBrain;
using Domain;

namespace DAL
{
    public class LocalStorageAccess
    {
        private const string DirConfigsName = "Configs";
        private const string DirSavesName = "Saves";
        private readonly string _configsPath;
        private readonly string _savesPath;
        private readonly JsonSerializerOptions _jsonSerializerOptions = new() {WriteIndented = true};

        public LocalStorageAccess(string basePath)
        {
            _configsPath = basePath + System.IO.Path.DirectorySeparatorChar + DirConfigsName;
            _savesPath = basePath + System.IO.Path.DirectorySeparatorChar + DirSavesName;
            
            CheckForFolders();
        }

        public void CheckForFolders()
        {
            if (!Directory.Exists(_configsPath)) Directory.CreateDirectory(_configsPath);
            if (!Directory.Exists(_savesPath)) Directory.CreateDirectory(_savesPath);
        }

        public void SaveConfiguration(string fileName, GameConfiguration gameConfiguration)
        {
            if (!Directory.Exists(_configsPath)) Directory.CreateDirectory(_configsPath);
            var jsonStr = JsonSerializer.Serialize(gameConfiguration, _jsonSerializerOptions);
            fileName = CheckNameConfiguration(fileName);
            File.WriteAllText(_configsPath + System.IO.Path.DirectorySeparatorChar + fileName + ".json", jsonStr);
        }

        public string CheckNameConfiguration(string filename)
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

        public GameConfiguration? GetConfiguration(string fileName)
        {
            if (!Directory.Exists(_configsPath))
            {
                Directory.CreateDirectory(_configsPath);
                return null;
            }
            var fileContent = File.ReadAllText(_configsPath + System.IO.Path.DirectorySeparatorChar + fileName + ".json");
            return JsonSerializer.Deserialize<GameConfiguration>(fileContent);
        }
        
        public List<string> GetConfigurationFileNames()
        {
            return GetFiles(new DirectoryInfo(_configsPath));
        }

        public bool ConfigurationNameExists(string filename)
        {
            var files = GetConfigurationFileNames();
            return files.Any(file => filename == file);
        }

        public void SaveGame(string fileName, BSBrain brain)
        {
            if (!Directory.Exists(_savesPath)) Directory.CreateDirectory(_savesPath);
            var jsonStr = brain.GetBrainJson();
            File.WriteAllText(_savesPath + System.IO.Path.DirectorySeparatorChar + fileName + ".json", jsonStr);
        }

        public BSBrain? GetSavedGame(string fileName)
        {
            if (!Directory.Exists(_savesPath))
            {
                Directory.CreateDirectory(_savesPath);
                return null;
            }

            var fileContent = File.ReadAllText(_savesPath + System.IO.Path.DirectorySeparatorChar + fileName + ".json");
            var brain = new BSBrain(new GameConfiguration());
            brain.RestoreBrainFromJson(fileContent);
            brain.GameName = fileName;
            return brain;
        }

        public List<string> GetSaveGameFileNames()
        {
            return GetFiles(new DirectoryInfo(_savesPath));
        }

        public void UpdateGame(BSBrain newGameSave)
        {
            File.Delete(_savesPath + newGameSave.GameName + ".json");
            SaveGame(newGameSave.GameName, newGameSave);
        }
        
        private static List<string> GetFiles(DirectoryInfo dir)
        {
            var files = dir.GetFiles("*.json");
            return files.Select(file => Path.GetFileNameWithoutExtension(file.ToString())).ToList();
        }

    }
}