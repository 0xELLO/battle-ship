using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BattleShipConsoleUI;

namespace MenuSystem
{
    public class ConfigurationMenuSystem
    {
        private readonly string _title;
        private readonly EConfiguraitonMenuType _menuType;

        public Func<List<MenuItem>>? DynamicMenuItems = null;
        public List<MenuItem>? StaticMenuItems = null;
        public Func<List<string>>? DynamicTitleBlock = null;
        public List<string>? SelectTitleBlock = null;
        private string _error = "";

        public ConfigurationMenuSystem(string title, EConfiguraitonMenuType menuType)
        {
            _title = title;
            _menuType = menuType;
        }

        public void RunStandartMenu()
        {
            var done = false;
            var currentElement = 0;
            do
            {
                Console.Clear();
                WriteTitleBlock(currentElement);
                
                var currentMenuItems = new List<MenuItem>();
                if (DynamicMenuItems != null)
                {
                    currentMenuItems = DynamicMenuItems();
                } else if (StaticMenuItems != null)
                {
                    currentMenuItems = StaticMenuItems;
                }
                
                for (int i = 0; i < currentMenuItems.Count; i++)
                {
                    switch (_menuType)
                    {
                        case EConfiguraitonMenuType.Vertical:
                            if (currentElement == i) WriteWithWhite(currentMenuItems[i].ToString(), true);
                            else Console.WriteLine(currentMenuItems[i]);
                            break;
                        case EConfiguraitonMenuType.Horizontal:
                            if (currentElement == i) WriteWithWhite(currentMenuItems[i].ToString(), false);
                            else Console.Write(currentMenuItems[i]);
                            if (i != currentMenuItems.Count - 1) Console.Write(" | ");
                            break;
                    }
                }

                var key = Console.ReadKey();

                switch (key.Key, _menuType)
                {
                    case (ConsoleKey.DownArrow, EConfiguraitonMenuType.Vertical):
                    case (ConsoleKey.RightArrow, EConfiguraitonMenuType.Horizontal):
                        currentElement++;
                        if (currentElement > currentMenuItems.Count - 1) currentElement = 0;
                        break;
                    case (ConsoleKey.UpArrow, EConfiguraitonMenuType.Vertical):
                    case (ConsoleKey.LeftArrow, EConfiguraitonMenuType.Horizontal):
                        currentElement--;
                        if (currentElement < 0) currentElement = currentMenuItems.Count - 1;
                        break;
                    case (ConsoleKey.Enter, EConfiguraitonMenuType.Vertical):
                    case (ConsoleKey.Enter, EConfiguraitonMenuType.Horizontal):
                        if (currentElement == currentMenuItems.Count - 1)
                        {
                            done = true;
                            break;
                        }
                        if (currentMenuItems[currentElement].RunAction != null)
                            currentMenuItems[currentElement].RunAction?.Invoke();
                        else if (currentMenuItems[currentElement].RunMethod != null)
                        {
                            var output = currentMenuItems[currentElement].RunMethod?.Invoke();
                            if (output != "") _error = output!;
                            else done = true;
                        } 
                        else
                        {
                            currentMenuItems[currentElement].RunMethodApply?.Invoke(currentElement.ToString());
                        }
                        break;
                }
            } while (!done);
        }
        
        public List<StringBuilder> RunInput(Dictionary<string, Func<string, bool>> titles)
        {
            var currentElement = 0;

            var elements = titles.Select(_ => new StringBuilder()).ToList();

            do
            {
                Console.Clear();
                WriteTitleBlock(currentElement);

                for (int i = 0; i < titles.Count; i++)
                {
                    Console.Write(titles.ElementAt(i).Key);
                    Console.Write(elements[i]);
                }

                var cursorDistance = 0;
                for (var i = 0; i < currentElement + 1; i++)
                {
                    cursorDistance += titles.ElementAt(i).Key.Length;
                }
                cursorDistance += elements.Sum(element => element.Length);
                Console.CursorLeft = cursorDistance;
                
                var key = Console.ReadKey();
                var text = elements[currentElement];
                HandleInput(key, ref text, ref currentElement, titles.ElementAt(currentElement).Value);


            } while (currentElement != titles.Count);

            return elements;
        }
        
        private void HandleInput(ConsoleKeyInfo key, ref StringBuilder text, ref int currentElement,
            Func<string, bool> validation)
        {
            switch (key.Key)
            {
                case ConsoleKey.Enter:
                    if(text.Length > 0) currentElement++;
                    break;
                case ConsoleKey.Backspace:
                    if(text.Length > 0) text.Remove(text.Length - 1, 1);
                    break;
                case ConsoleKey.Escape:
                    text.Clear();
                    break;
                default:
                {
                    if (validation(text.ToString() + key.KeyChar)) text.Append(key.KeyChar);
                    break;
                }
            }
        }
        
        private void WriteTitleBlock(int i)
        {
            Console.WriteLine(_title, Console.ForegroundColor = ConsoleColor.Cyan);
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("---------------------------");
            if (_error != "")
            {
                WriteWithRed(_error);
                _error = "";
            }

            if (DynamicTitleBlock != null)
            {
                foreach (var element in DynamicTitleBlock()) Console.WriteLine(element);
                Console.WriteLine("---------------------------");
            }
            else if (SelectTitleBlock != null)
            {
                if (i >= SelectTitleBlock.Count) i = 0;
                Console.WriteLine(SelectTitleBlock[i]);
                Console.WriteLine("---------------------------");
            }
        }

        private static void WriteWithRed(string text)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.DarkRed;
            Console.WriteLine(text);
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Black;
        }
        private static void WriteWithWhite(string text, bool line)
        {
            Console.ForegroundColor = ConsoleColor.Black;
            Console.BackgroundColor = ConsoleColor.White;
            if (line) Console.WriteLine(text);
            else Console.Write(text);
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Black;
        }
    }
}