namespace JustShapesBeatsMultiplayerServer.Packets
{
    class RequestMemberDataPacket : IPacket
    {
        public ushort RoomID { private set; get; }

        public ushort PlayerID { private set; get; }

        public string Key { private set; get; }

        public Packet Packet { private set; get; }

        public RequestMemberDataPacket(Packet packet)
        {
            RoomID = packet.GetUshort();
            PlayerID = packet.GetUshort();
            Key = packet.GetString();
        }

        public RequestMemberDataPacket(string value)
        {
            Packet = new Packet(PacketEnum.RequestMemberData);
            Packet.Add(value);
        }
    }
}
