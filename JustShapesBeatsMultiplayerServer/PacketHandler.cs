using JustShapesBeatsMultiplayerServer.Data;
using System.Collections.Generic;

namespace JustShapesBeatsMultiplayerServer
{
    public delegate void PacketDelegate(Packet packet, Client client);

    public class PacketHandler
    {
        private static Dictionary<byte, PacketDelegate> _packetHandlers;

        public static void Init()
        {
            _packetHandlers = new Dictionary<byte, PacketDelegate>();
        }

        public static void AddPacketHandler(byte packetID, PacketDelegate packetHandler)
        {
            if (_packetHandlers.ContainsKey(packetID))
            {
                Debug.LogError($"Failed add packet handler. _packetHandlers.ContainsKey(packetId) == true, PacketID: {packetID}");
                return;
            }
            Debug.Log($"Added Packet Handler. PacketID: {packetID}");
            _packetHandlers.Add(packetID, packetHandler);
        }

        public static void Invoke(byte packetID, Packet packet, Client client)
        {
            if (!_packetHandlers.ContainsKey(packetID))
            {
                Debug.LogError($"Failed invoke packet handler. _packetHandlers.ContainsKey(packetId) == false, PacketID: {packetID}");
                return;
            }
            _packetHandlers[packetID].Invoke(packet, client);
        }
    }
}
