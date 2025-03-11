namespace JustShapesBeatsMultiplayerServer.Packets
{
    class RequestNumLobbyMembersPacket : IPacket
    {
        public ushort RoomID { private set; get; }

        public Packet Packet { private set; get; }

        public RequestNumLobbyMembersPacket(Packet packet)
        {
            RoomID = packet.GetUshort();
        }

        public RequestNumLobbyMembersPacket(byte numPlayers)
        {
            Packet = new Packet(PacketEnum.RequestNumLobbyMembers);
            Packet.Add(numPlayers);
        }
    }
}
