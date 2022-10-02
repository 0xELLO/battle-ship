using System;
using System.Collections.Generic;
using System.Linq;
using BattleShipBrain;
using MenuSystem;

namespace BattleShipConsoleUI
{
    public class ConfigurationUi
    {
        private int _gameBoardSizeX = 10;
        private int _gameBoardSizeY = 10;
        private EShipTouchRule _gameRule = EShipTouchRule.NoTouch;
        private readonly List<ShipConfig> _shipConfigs = new List<ShipConfig>();
        private GameConfiguration? _gameConfiguration = null;

        public GameConfiguration? RunConfiguration()
        {
            MainMenu();
            return _gameConfiguration;
        }

        // MENUS
        private void MainMenu()
        {
            ConfigurationMenuSystem menu = new ConfigurationMenuSystem("Game configuration Menu",
                EConfiguraitonMenuType.Horizontal);
            menu.DynamicTitleBlock = MainMenuDynamicTitleBlock;
            menu.StaticMenuItems = new List<MenuItem>
            {
                new MenuItem("Add ship", AddShipMenu),
                new MenuItem("Delete ship", DeleteShipMenu),
                new MenuItem("Change board size", ChangeSizeMenu),
                new MenuItem("Change board rule", ChangeRuleMenu),
                new MenuItem("Finish", FinishConfiguration),
                new MenuItem("Cancel")
            };
            menu.RunStandartMenu();
        }

        private void AddShipMenu()
        {
            var menu = new ConfigurationMenuSystem("Choose ship configuration", EConfiguraitonMenuType.Input);
            menu.SelectTitleBlock = new List<string>
            {
                "Your ship name",
                "How many ships of this type you want to be placed on board | maximum 10",
                "Ship length | maximum 10",
                "Ship width | maximum 10",
                "Back to menu"
            };
            var titles = new Dictionary<string, Func<string, bool>>
            {
                {"Name: ", Validation.ValidateShipName},
                {"  Quantity: ", Validation.ValidateShipInt},
                {"  X: ", Validation.ValidateShipInt},
                {"  Y: ", Validation.ValidateShipInt}
            };
            var a = menu.RunInput(titles);
            _shipConfigs.Add(new ShipConfig
            {
                Name = a[0].ToString(),
                Quantity = int.Parse(a[1].ToString()),
                ShipSizeX = int.Parse(a[2].ToString()),
                ShipSizeY = int.Parse(a[3].ToString()),
            });
        }

        private void DeleteShipMenu()
        {
            var menu = new ConfigurationMenuSystem("Choose ship to delete", EConfiguraitonMenuType.Vertical)
            {
                DynamicMenuItems = DynamicShipsItemsBlock
            };
            menu.RunStandartMenu();
        }

        private void ChangeRuleMenu()
        {
            var menu = new ConfigurationMenuSystem("Choose board rule", EConfiguraitonMenuType.Horizontal);
            menu.StaticMenuItems = new List<MenuItem>
            {
                new MenuItem("No touch rule", delegate()
                {
                    _gameRule = EShipTouchRule.NoTouch;
                    return "";
                }),
                new MenuItem("Corner touch rule", delegate()
                {
                    _gameRule = EShipTouchRule.CornerTouch;
                    return "";
                }),
                new MenuItem("Side touch rule", delegate()
                {
                    _gameRule = EShipTouchRule.SideTouch;
                    return "";
                }),
                new MenuItem("Cancel")
            };
            menu.SelectTitleBlock = new List<string>
            {
                "Ships cannot touch each other",
                "Ships can touch each other only at corners",
                "Ships can touch each other at corners and at sides",
                "Back to menu"
            };
            menu.RunStandartMenu();
        }

        private void ChangeSizeMenu()
        {
            var menu = new ConfigurationMenuSystem("Choose Board size", EConfiguraitonMenuType.Input);
            var titles = new Dictionary<string, Func<string, bool>>
            {
                {"Size X: ", Validation.ValidateBoardSize},
                {"  Size Y: ", Validation.ValidateBoardSize},
            };
            menu.SelectTitleBlock = new List<string> {"Maximum size 90 | Minimum size 5"};
            var a = menu.RunInput(titles);
            _gameBoardSizeX = int.Parse(a[0].ToString());
            _gameBoardSizeY = int.Parse(a[1].ToString());
        }

        // METHODS
        private string FinishConfiguration()
        {
            if (_shipConfigs.Count == 0) return Error.AddShipFirst;
            var gameConfiguration= new GameConfiguration
            {
                BoardSizeX = _gameBoardSizeX,
                BoardSizeY = _gameBoardSizeY,
                EShipTouchRule = _gameRule,
                ShipConfigs = _shipConfigs
            };
            if (!gameConfiguration.ControlIfConfigurationIsPossible()) return Error.ImpossibleBoard;
            _gameConfiguration = gameConfiguration;
            return "";
        }

        private List<string> MainMenuDynamicTitleBlock()
        {
            var result = new List<string>();
            result.Add("Board size is " + _gameBoardSizeX + "x" + _gameBoardSizeY);
            result.Add("Rule is: " + _gameRule);
            if (_shipConfigs.Count == 0) result.Add("No ships yet");
            else
            {
                result.AddRange(_shipConfigs.Select(shipConfig => shipConfig.ToString()));
            }
            return result;
        }

        private List<MenuItem> DynamicShipsItemsBlock()
        {
            var result = _shipConfigs.Select(
                shipConfig => new MenuItem(shipConfig.ToString(), DeleteShip)).ToList();
            result.Add(new MenuItem("Cancel"));
            return result;
        }

        private string DeleteShip(string i)
        {
            _shipConfigs.RemoveAt(Int32.Parse(i));
            return "";
        }
    }
}