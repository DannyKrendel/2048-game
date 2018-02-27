using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game
{
    // Class for changing color for cells
    static class ColorChanger
    {
        // Get color for corresponding value of cell
        static ConsoleColor GetCellColor(string value)
        {
            switch (value)
            {
                case "2":
                    return ConsoleColor.Blue;
                case "4":
                    return ConsoleColor.Magenta;
                case "8":
                    return ConsoleColor.Cyan;
                case "16":
                    return ConsoleColor.Green;
                case "32":
                    return ConsoleColor.Yellow;
                case "64":
                    return ConsoleColor.DarkBlue;
                case "128":
                    return ConsoleColor.DarkMagenta;
                case "256":
                    return ConsoleColor.DarkCyan;
                case "512":
                    return ConsoleColor.DarkGreen;
                case "1024":
                    return ConsoleColor.DarkYellow;
                default:
                    return ConsoleColor.Red;
            }
        }

        // Color certain cell with optional background color
        public static void ColorCell(string value, ConsoleColor bgColor = ConsoleColor.Black)
        {
            ConsoleColor defaultFg = Console.ForegroundColor;
            ConsoleColor defaultBg = Console.BackgroundColor;

            Console.ForegroundColor = GetCellColor(value);
            Console.BackgroundColor = bgColor;

            Console.WriteLine(value);

            Console.ForegroundColor = defaultFg;
            Console.BackgroundColor = defaultBg;
        }
    }
}
