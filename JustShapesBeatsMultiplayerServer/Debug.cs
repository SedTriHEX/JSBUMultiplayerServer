using System;

namespace JustShapesBeatsMultiplayerServer
{
    class Debug
    {
        public static void Log(object obj)
        {
            Log(obj.ToString());
        }

        public static void Log(string text)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"|{GetStringNowTime()}| |LOG| {text}");
        }

        public static void LogError(string text)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"|{GetStringNowTime()}| |ERROR| {text}");
        }

        public static void LogWarning(string text)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"|{GetStringNowTime()}| |WARNING| {text}");
        }

        private static string GetStringNowTime()
        {
            return DateTime.Now.ToString();
        }
    }
}
