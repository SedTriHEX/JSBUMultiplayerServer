using System;
using System.Collections.Generic;

namespace JustShapesBeatsMultiplayerServer
{
    class CommandHandler
    {
        private static Dictionary<string, Action<string[]>> _methods;

        private static bool _inited = false;

        public static void Init()
        {
            if (_inited)
                return;

            _methods = new Dictionary<string, Action<string[]>>()
            {
                ["exit"] = Exit,
                ["playerlist"] = ShowPlayerList,
                ["roomlist"] = ShowRoomList,
                ["gameip"] = ShowListListenIp,
            };
        }

        public static void HandleCommand(string command)
        {
            if (!_inited)
                Init();

            CommandParser.CommandInfo info = CommandParser.Parse(command);
            if (info.FailedParse)
            {
                Debug.LogError("Failed parse command.");
                return;
            }

            string commandName = info.CommandName.ToLower();

            if (!_methods.ContainsKey(commandName))
            {
                Debug.LogError("Unknown command");
                return;
            }
            _methods[commandName].Invoke(info.Arguments.ToArray());
        }

        private static void Exit(string[] arguments)
        {
            Program.ServerManagerInstance.Stop();
        }

        private static void ShowPlayerList(string[] arguments)
        {
            
        }

        private static void ShowRoomList(string[] arguments)
        {

        }

        private static void ShowListListenIp(string[] arguments)
        {
            string[] list = Helper.GetDeviceLocalIPAddresses();
            for (int i = 0; i < list.Length; i++)
            {
                Debug.Log($"[{i}] - {list[i]}");
            }
        }
    }
}
