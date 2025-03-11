namespace JustShapesBeatsMultiplayerServer.Packets
{
    class JoinRoomPacket : IPacket
    {
        public ushort RoomID { private set; get; }

        public Packet Packet { private set; get; }

        public JoinRoomPacket(Packet packet)
        {
            RoomID = packet.GetUshort();
        }

        public JoinRoomPacket(bool connectedInRoom)
        {
            byte byteConnectedInRoom = (byte)(connectedInRoom ? 1 : 0);

            Packet = new Packet(PacketEnum.JoinRoom);
            Packet.Add(byteConnectedInRoom);
        }
    }
}
