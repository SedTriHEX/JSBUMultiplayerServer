using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace JustShapesBeatsMultiplayerServer
{
    class Helper
    {
        public static Random Random = new Random();

        public static string GenerateGuid()
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(Guid.NewGuid().ToString().Remove(6)));
        }

        public static string[] GetDeviceLocalIPAddresses()
        {
            List<string> list = new List<string>();
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                    list.Add(ip.ToString());
            }
            return list.ToArray();
        }
    }
}
