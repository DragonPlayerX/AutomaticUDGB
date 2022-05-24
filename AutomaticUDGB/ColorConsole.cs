using System;

namespace AutomaticUDGB
{
    public static class ColorConsole
    {
        public static void Msg(string message, bool newLine = true)
        {
            ConsoleColor color = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.White;
            if (newLine)
                Console.WriteLine(message);
            else
                Console.Write(message);
            Console.ForegroundColor = color;
        }

        public static void Success(string message)
        {
            ConsoleColor color = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(message);
            Console.ForegroundColor = color;
        }

        public static void Warning(string message)
        {
            ConsoleColor color = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(message);
            Console.ForegroundColor = color;
        }

        public static void Error(string message)
        {
            ConsoleColor color = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ForegroundColor = color;
        }
    }
}
