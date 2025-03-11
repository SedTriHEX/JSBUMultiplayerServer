namespace JustShapesBeatsMultiplayerServer.Packets
{
    class RequestFriendPersonaNamePacket : IPacket
    {
        public ushort PlayerID { private set; get; }

        public Packet Packet { private set; get; }

        public RequestFriendPersonaNamePacket(Packet packet)
        {
            PlayerID = packet.GetUshort();
        }

        public RequestFriendPersonaNamePacket(string personaName)
        {
            Packet = new Packet(PacketEnum.RequestFriendPersonaName);
            Packet.Add(personaName);
        }
    }
}
