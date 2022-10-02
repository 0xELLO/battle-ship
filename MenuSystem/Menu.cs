using System;
using System.Collections.Generic;
using System.Net.Mime;

namespace BattleShipConsoleUI
{
    public class Menu
    {

        private readonly string _title;
        private readonly EMenuLevel _menuLevel;
        
        private readonly List<MenuItem> _menuItems = new List<MenuItem>();
        private readonly List<MenuItem> _menuSpecialItems = new List<MenuItem>();
        
        private readonly MenuItem _menuItemExit = new MenuItem("Exit");
        private readonly MenuItem _menuItemReturn = new MenuItem( "Return to previous menu");
        private readonly MenuItem _menuItemMain = new MenuItem( "Return to main menu");
        
        public Menu(string title, EMenuLevel menuLevel)
        {
            _title = title;
            _menuLevel = menuLevel;

            switch (_menuLevel)
            {
                case EMenuLevel.Root:
                    _menuSpecialItems.Add(_menuItemExit);
                    break;
                case EMenuLevel.First:
                    _menuSpecialItems.Add(_menuItemReturn);
                    _menuSpecialItems.Add(_menuItemExit);
                    break;
                case EMenuLevel.SecondOrMore:
                    _menuSpecialItems.Add(_menuItemReturn);
                    _menuSpecialItems.Add(_menuItemMain);
                    _menuSpecialItems.Add(_menuItemExit);
                    break;
            }
        }

        public void AddMenuItem(MenuItem item)
        {
            _menuItems.Add(item);
        }
        
        public void AddMenuItems(List<MenuItem> items)
        {
            foreach (var menuItem in items)
            {
                AddMenuItem(menuItem);
            }
        }

        public string Run()
        {
            var runDone = false;

            do
            {
                var input = OutPutMenu();
                var isInputValid = _menuItems.Count > input;

                var output = "";

                switch (isInputValid)
                {
                    case true:
                        var item = _menuItems[input];
                        var runMethod = item.RunMethod;

                        if (runMethod == null)
                        { 
                            output = item.RunMethodApply?.Invoke(item.ToString());
                            
                            if (output == "exit")
                            {
                                return "exit";
                            }
                            if (output == "main" && _menuLevel != EMenuLevel.Root)
                            {
                                return "main";
                            }
                        }
                        else
                        {
                            output = runMethod?.Invoke();
                            
                            if (output == "exit")
                            {
                                return "exit";
                            }
                            if (output == "main" && _menuLevel != EMenuLevel.Root)
                            {
                                return "main";
                            }
                        }
                        break;
                    case false:
                        runDone = true;
                        switch (_menuLevel)
                        {
                            case EMenuLevel.First:
                                if (_menuItems.Count == input) return "return";
                                if (_menuItems.Count + 1 == input) return "exit";
                                break;
                            case EMenuLevel.SecondOrMore:
                                if (_menuItems.Count == input) return "return";
                                if (_menuItems.Count + 1 == input) return "main";
                                if (_menuItems.Count + 2 == input) return "exit";
                                break;
                        }

                        break;
                }
                
            } while (!runDone);

            return "";
        }

        private int OutPutMenu()
        {
            ConsoleKeyInfo key;
            var currentItem = 0;
            do
            {
                Console.Clear();
                Console.WriteLine(_title);
                Console.WriteLine("-------------------");
                
                for (var i = 0; i < _menuItems.Count; i++)
                {
                    if (currentItem == i)
                    {
                        Console.BackgroundColor = ConsoleColor.White;
                        Console.ForegroundColor = ConsoleColor.Black;
                    }
                    Console.WriteLine(_menuItems[i]);
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.ForegroundColor = ConsoleColor.White;
                }
                
                Console.WriteLine("-------------------");


                for (var i = 0; i < _menuSpecialItems.Count; i++)
                {
                    
                    if (currentItem == i + _menuItems.Count)
                    {
                        Console.BackgroundColor = ConsoleColor.White;
                        Console.ForegroundColor = ConsoleColor.Black;
                    }

                    Console.WriteLine(_menuSpecialItems[i]);
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.ForegroundColor = ConsoleColor.White;

                }
                
                key = Console.ReadKey(true);
    
                switch (key.Key.ToString())
                {
                    case "DownArrow":
                    {
                        currentItem++;
                        if (currentItem > _menuItems.Count + _menuSpecialItems.Count - 1) currentItem = 0;
                        break;
                    }
                    case "UpArrow":
                    {
                        currentItem--;
                        if (currentItem < 0) currentItem = (_menuItems.Count + _menuSpecialItems.Count - 1);
                        break;
                    }
                }

            } while (key.KeyChar != 13);

            return currentItem;
        }

    }
}