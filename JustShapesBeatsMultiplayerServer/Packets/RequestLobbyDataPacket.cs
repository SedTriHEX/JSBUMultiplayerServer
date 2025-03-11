namespace JustShapesBeatsMultiplayerServer.Packets
{
    class RequestLobbyDataPacket : IPacket
    {
        public ushort RoomID { private set; get; }

        public string Key { private set; get; }

        public Packet Packet { private set; get; }

        public RequestLobbyDataPacket(Packet packet)
        {
            RoomID = packet.GetUshort();
            Key = packet.GetString();
        }

        public RequestLobbyDataPacket(string value)
        {
            Packet = new Packet(PacketEnum.RequestLobbyData);
            Packet.Add(value);
        }
    }
}
