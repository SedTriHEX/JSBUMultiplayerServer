namespace JustShapesBeatsMultiplayerServer.Packets
{
    class JoinedPlayerPacket : IPacket
    {
        public Packet Packet { private set; get; }

        public JoinedPlayerPacket(ushort roomID, ushort playerID)
        {
            Packet = new Packet(PacketEnum.JoinedPlayer);
            Packet.Add(roomID);
            Packet.Add(playerID);
        }
    }
}
