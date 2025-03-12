namespace JustShapesBeatsMultiplayerServer.Packets
{
    struct PacketEnum
    {
        public const byte ConnectedClient = 1;

        public const byte RequestListRooms = 2;

        public const byte CreateRoom = 3;

        public const byte RequestNumLobbyMembers = 4;

        public const byte RequestLobbyOwner = 5;

        public const byte SetLobbyData = 6;

        public const byte SetMemberData = 7;

        public const byte RequestLobbyMemberByIndexRequest = 8;

        public const byte SetLobbyOwner = 9;

        public const byte RequestMemberData = 10;

        public const byte RequestFriendPersonaName = 11;

        public const byte RequestLobbyData = 12;

        public const byte JoinRoom = 13;

        public const byte LeaveRoom = 14;

        // TODO: 15 GetCommunityRoomList

        public const byte ChangedPlayerData = 50;

        public const byte PlayerHasLeftRoom = 51;

        public const byte JoinedPlayer = 52;

        // TODO: public const byte SendInvite = 53;

        // TODO: public const byte InviteOnRoom = 54;

        public const byte PingPong = 60;
    }
}
