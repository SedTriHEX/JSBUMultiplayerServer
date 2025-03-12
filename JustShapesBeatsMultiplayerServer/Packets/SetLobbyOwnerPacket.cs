using System;
using System.Collections.Generic;
using System.Text;

namespace JustShapesBeatsMultiplayerServer.Packets
{
    class SetLobbyOwnerPacket : IPacket
    {
        public ushort RoomID { private set; get; }

        public ushort PlayerID { private set; get; }

        public Packet Packet { private set; get; }

        public SetLobbyOwnerPacket(Packet packet)
        {
            RoomID = packet.GetUshort();
            PlayerID = packet.GetUshort();
        }
    }
}
