using JustShapesBeatsMultiplayerServer.Data;
using System.Collections.Generic;

namespace JustShapesBeatsMultiplayerServer.Packets
{
    class RequestRoomListPacket : IPacket
    {
        public byte VersionNET { private set; get; }

        public JSBLobbyMode LobbyMode { private set; get; }

        public JSBDifficulty Difficulty { private set; get; }

        public Packet Packet { private set; get; }

        public RequestRoomListPacket(Packet packet)
        {
            VersionNET = packet.GetByte();
            LobbyMode = (JSBLobbyMode)packet.GetByte();
            Difficulty = (JSBDifficulty)packet.GetInt();
        }

        public RequestRoomListPacket(List<KeyValuePair<ushort, Room>> rooms)
        {
            Packet = new Packet(PacketEnum.RequestListRooms);
            Packet.Add(rooms.Count);

            for (int i = 0; i < rooms.Count; i++)
                Packet.Add(rooms[i].Key);
        }
    }
}
