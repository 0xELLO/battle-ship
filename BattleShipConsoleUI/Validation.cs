using System;
using System.IO;
using System.Linq;

namespace BattleShipConsoleUI
{
    public static class Validation
    {
        public static bool ValidateShipName(string input)
        {
            return input.Length <= 20;
        }

        public static bool ValidateShipInt(string input)
        {
            return ValidateInt(10, input);
        }

        private static bool ValidateInt(int maxSize, string input)
        {
            foreach (var c in input.ToCharArray())
            {
                if (!int.TryParse(c.ToString(), out _)) return false;
            }

            return int.TryParse(input, out var x) && x <= maxSize && x != 0;
        }


        public static bool ValidateBoardSize(string input)
        {
            return ValidateInt(90, input);
        }

        public static bool ValidateConfigurationFileName(string? input)
        {
            if (input is null or "") return false;
            foreach (var invalidChar in Path.GetInvalidFileNameChars())
            {
                if (input.Any(character => character == invalidChar)) return false;
            }
            return input.Last() != ' ' && input.Last() != '.';
        }
    }
}