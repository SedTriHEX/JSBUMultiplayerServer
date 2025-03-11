namespace JustShapesBeatsMultiplayerServer.Packets
{
    class RequestLobbyMemberByIndexPacket : IPacket
    {
        public ushort RoomID { private set; get; }

        public ushort Index { private set; get; }

        public Packet Packet { private set; get; }

        public RequestLobbyMemberByIndexPacket(Packet packet)
        {
            RoomID = packet.GetUshort();
            Index = packet.GetUshort();
        }

        public RequestLobbyMemberByIndexPacket(ushort playerID)
        {
            Packet = new Packet(PacketEnum.RequestLobbyMemberByIndexRequest);
            Packet.Add(playerID);
        }
    }
}
