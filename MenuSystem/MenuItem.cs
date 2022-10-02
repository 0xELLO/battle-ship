using System;
using BattleShipBrain;

namespace BattleShipConsoleUI
{
    public class MenuItem
    {

        private readonly string _title;
        public Func<string>? RunMethod { get; set; } 
        public Func<string, string>? RunMethodApply { get; set; }
        public Action? RunAction { get; set; }
        
        public int shortCut { get; set; } = 0;

        public MenuItem(string title, Func<string> runMethod)
        {
            _title = title;
            RunMethod = runMethod;
        }

        public MenuItem(string title, Func<string, string> runMethod)
        {
            _title = title;
            RunMethodApply = runMethod;
        }
        public MenuItem(string title)
        {
            _title = title;
            RunMethod = null;
        }

        public MenuItem(string title, Action action)
        {
            _title = title;
            RunAction = action;
            
            
        }

        public override string ToString()
        {
            return _title;
        }
    }
}