namespace JustShapesBeatsMultiplayerServer.Packets
{
    class RequestLobbyOwnerPacket : IPacket
    {
        public ushort RoomID { private set; get; }

        public Packet Packet { private set; get; }

        public RequestLobbyOwnerPacket(Packet packet)
        {
            RoomID = packet.GetUshort();
        }

        public RequestLobbyOwnerPacket(ushort ownerPlayerID)
        {
            Packet = new Packet(PacketEnum.RequestLobbyOwner);
            Packet.Add(ownerPlayerID);
        }
    }
}
