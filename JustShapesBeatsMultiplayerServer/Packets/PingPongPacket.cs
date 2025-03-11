namespace JustShapesBeatsMultiplayerServer.Packets
{
    class PingPongPacket : IPacket
    {
        public bool AvailableRequestData { private set; get; }

        public ushort PlayerID { private set; get; }

        public bool InRoom { private set; get; }

        public ushort RoomID { private set; get; }

        public ushort PlayerIDOwnerOfRoom { private set; get; }

        public Packet Packet { private set; get; }

        public PingPongPacket(Packet packet)
        {
            AvailableRequestData = packet.GetBool();
            if (!AvailableRequestData) return;

            PlayerID = packet.GetUshort();
            InRoom = packet.GetBool();
            if (InRoom)
            {
                RoomID = packet.GetUshort();
                PlayerIDOwnerOfRoom = packet.GetUshort();
            }
        }

        public PingPongPacket()
        {
            Packet = new Packet(PacketEnum.PingPong);
            Packet.Add(true);
        }
    }
}
