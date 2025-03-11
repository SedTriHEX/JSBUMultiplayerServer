using JustShapesBeatsMultiplayerServer.Managers;
using System;

namespace JustShapesBeatsMultiplayerServer
{
    class Program
    {
        public static ServerManager ServerManagerInstance;

        static void Main(string[] args)
        {
            ServerManagerInstance = new ServerManager();

            ServerManagerInstance.Init();

            ServerManagerInstance.Start();

            while (ServerManagerInstance.IsListening)
            {
                string command = Console.ReadLine();
                CommandHandler.HandleCommand(command);
            }
        }
    }
}
