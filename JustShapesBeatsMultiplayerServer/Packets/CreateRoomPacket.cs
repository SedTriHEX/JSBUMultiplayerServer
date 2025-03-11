namespace JustShapesBeatsMultiplayerServer.Packets
{
    class CreateRoomPacket : IPacket
    {
        public int MaxPlayers { private set; get; }

        public Packet Packet { private set; get; }

        public CreateRoomPacket(Packet packet)
        {
            MaxPlayers = packet.GetInt();
        }

        public CreateRoomPacket(ushort roomId)
        {
            Packet = new Packet(PacketEnum.CreateRoom);
            Packet.Add(roomId);
        }
    }
}
