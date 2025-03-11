namespace JustShapesBeatsMultiplayerServer.Packets
{
    class ConnectedClientPacket : IPacket
    {
        public const byte OK = 101;

        public string NickName { private set; get; }

        public string GameVersion { private set; get; }

        public Packet Packet { private set; get; }

        public ConnectedClientPacket(Packet packet)
        {
            NickName = packet.GetString();
            GameVersion = packet.GetString();
        }

        public ConnectedClientPacket(byte status, ushort playerID)
        {
            Packet = new Packet(PacketEnum.ConnectedClient);
            Packet.Add(status);
            Packet.Add(playerID);
        }
    }
}
