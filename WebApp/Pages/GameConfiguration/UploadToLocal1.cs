using System;
using System.Collections.Generic;
using System.Linq;
using BattleShipBrain;
using DAL;
using Domain;
using Microsoft.AspNetCore.Mvc;
using WebApp.Pages_ShipConfiguration;

namespace WebApp.Pages_GameConfiguration
{
    public class UploadToLocal1 : Controller
    {
        private readonly DAL.ApplicationDbContext _context;

        public UploadToLocal1(DAL.ApplicationDbContext context)
        {
            _context = context;
        }
        
        // GET
        public IActionResult Index(int id)
        {
            Console.WriteLine(id + "Here");
            var configurationFromDb = _context.GameConfigs.Find(id);
            var lsa = new LocalStorageAccess(GlobalPath.BattleShipPath);

            var shipConfigs = new List<ShipConfig>();
            foreach (var gameShipConfig in  _context.GameShipConfig.Where(gsc => gsc.GameConfigId == configurationFromDb.GameConfigId))
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
                BoardSizeX = configurationFromDb.BoardSizeX,
                BoardSizeY = configurationFromDb.BoardSizeY,
                EShipTouchRule = (EShipTouchRule) configurationFromDb.EShipTouchRule,
                Name = configurationFromDb.ConfigName,
                ShipConfigs = shipConfigs
            };
            
            lsa.SaveConfiguration(gameConfiguration.Name, gameConfiguration);
            return RedirectToPage("./Index");
        }
    }
}