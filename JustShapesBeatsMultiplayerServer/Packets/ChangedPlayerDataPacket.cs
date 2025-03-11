namespace JustShapesBeatsMultiplayerServer.Packets
{
    class ChangedPlayerDataPacket : IPacket
    {
        public Packet Packet { private set; get; }

        public ChangedPlayerDataPacket(ushort roomID, ushort playerID)
        {
            Packet = new Packet(PacketEnum.ChangedPlayerData);
            Packet.Add(roomID);
            Packet.Add(playerID);
        }
    }
}
