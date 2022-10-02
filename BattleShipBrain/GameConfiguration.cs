using System;
using System.Collections.Generic;
using System.Text.Json;

namespace BattleShipBrain
{
    public class GameConfiguration
    {
        public int BoardSizeX { get; set; } = 10;
        public int BoardSizeY { get; set; } = 10;
        public EShipTouchRule EShipTouchRule { get; set; } = BattleShipBrain.EShipTouchRule.SideTouch;

        public string Name { get; set; } = "Default";   
        
        public List<ShipConfig> ShipConfigs { get; set; } = new List<ShipConfig>()
        {
            new ShipConfig()
            {
                Name = "Patrol",
                Quantity = 5,
                ShipSizeY = 1,
                ShipSizeX = 1,
            },
            new ShipConfig()
            {
                Name = "Cruiser",
                Quantity = 4,
                ShipSizeY = 1,
                ShipSizeX = 2,
            },
            new ShipConfig()
            {
                Name = "Submarine",
                Quantity = 3,
                ShipSizeY = 1,
                ShipSizeX = 3,
            },
            new ShipConfig()
            {
                Name = "Battleship",
                Quantity = 2,
                ShipSizeY = 1,
                ShipSizeX = 4,
            },
            new ShipConfig()
            {
                Name = "Carrier",
                Quantity = 1,
                ShipSizeY = 1,
                ShipSizeX = 5,
            },
        };
        
        public override string ToString()
        {
            var jsonOptions = new JsonSerializerOptions()
            {
                WriteIndented = true
            };
            return JsonSerializer.Serialize(this, jsonOptions);
        }

        public string GetShipConfigsAsString()
        {
            var jsonOptions = new JsonSerializerOptions()
            {
                WriteIndented = true
            };
            
            return JsonSerializer.Serialize(ShipConfigs, jsonOptions);
        }
        
        public bool ControlIfConfigurationIsPossible()
        {
            var taken = 0;
            var overlap = 0;
            
            foreach (var shipConfig in ShipConfigs)
            {
                for (int q = 0; q < shipConfig.Quantity; q++)
                {
                    //var occupiedByOne = ((2 + shipConfig.ShipSizeY) * 2)
                     //                   + (shipConfig.ShipSizeX * 2)
                       //                 + (shipConfig.ShipSizeX * shipConfig.ShipSizeY);

                    var occupiedByOne = (shipConfig.ShipSizeX * shipConfig.ShipSizeY);
                    var overlapByOne = ((shipConfig.ShipSizeX * 2) + (shipConfig.ShipSizeY * 2)) + 4;
                    if (overlapByOne > overlap)
                    {
                        overlap = overlapByOne;
                        occupiedByOne += overlapByOne;
                    }

                    switch (EShipTouchRule)
                    {
                        case EShipTouchRule.NoTouch:
                            taken += occupiedByOne;
                            break;
                        case EShipTouchRule.CornerTouch:
                            taken += occupiedByOne - 4;
                            break;
                        case EShipTouchRule.SideTouch:
                            taken += shipConfig.ShipSizeX * shipConfig.ShipSizeY;
                            break;
                    }

                    var temp = (BoardSizeX + 2) * (BoardSizeY + 2);
                    if (taken > temp)
                    {
                        return false;
                    }

                }
            }
            Console.WriteLine("Taken: " + taken);
            return true;
        }
    }
}