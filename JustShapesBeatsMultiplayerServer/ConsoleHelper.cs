using System;

namespace JustShapesBeatsMultiplayerServer
{
    internal class ConsoleHelper
    {
        public static string WriteReadLine(string message)
        {
            Console.WriteLine(message);
            return Console.ReadLine();
        }

        public static void UpdateTitle(int playerCount, int roomCount)
        {
            Console.Title = $"JSBU Server ({Constants.ServerVersion}) | Players: {playerCount} | Rooms: {roomCount}";
        }
    }
}
