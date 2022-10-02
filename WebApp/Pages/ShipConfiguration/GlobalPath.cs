using System;
using System.IO;

namespace WebApp.Pages_ShipConfiguration
{
    public static class GlobalPath
    {
        public static string BasePath = Directory.GetParent(Directory.GetParent(Directory.GetParent(Directory.GetParent(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory)!.ToString())!.ToString().ToString())!.ToString())!.ToString()!)!.ToString();
        public static string BattleShipPath = BasePath + Path.DirectorySeparatorChar + "BattleShip";
    }
}