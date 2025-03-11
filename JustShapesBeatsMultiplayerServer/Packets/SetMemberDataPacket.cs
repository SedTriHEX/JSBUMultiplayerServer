namespace JustShapesBeatsMultiplayerServer.Packets
{
    class SetMemberDataPacket : IPacket
    {
        public ushort RoomID { private set; get; }

        public string Key { private set; get; }

        public string Value { private set; get; }

        public Packet Packet { private set; get; }

        public SetMemberDataPacket(Packet packet)
        {
            RoomID = packet.GetUshort();
            Key = packet.GetString();
            Value = packet.GetString();
        }
    }
}
