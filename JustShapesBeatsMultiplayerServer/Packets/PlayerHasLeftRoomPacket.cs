namespace JustShapesBeatsMultiplayerServer.Packets
{
    class PlayerHasLeftRoomPacket : IPacket
    {
        public Packet Packet { private set; get; }

        public PlayerHasLeftRoomPacket(ushort roomID, ushort playerID)
        {
            Packet = new Packet(PacketEnum.PlayerHasLeftRoom);
            Packet.Add(roomID);
            Packet.Add(playerID);
        }
    }
}
