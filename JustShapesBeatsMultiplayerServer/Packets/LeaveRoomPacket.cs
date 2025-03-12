namespace JustShapesBeatsMultiplayerServer.Packets
{
    class LeaveRoomPacket : IPacket
    {
        public ushort RoomID { private set; get; }

        public Packet Packet { private set; get; }

        public LeaveRoomPacket(Packet packet)
        {
            RoomID = packet.GetUshort();
        }
    }
}
