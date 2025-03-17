using JustShapesBeatsMultiplayerServer.Data;
using JustShapesBeatsMultiplayerServer.Packets;
using System.Collections.Generic;
using System.Linq;

namespace JustShapesBeatsMultiplayerServer.Managers
{
    class RoomManager
    {
        public Dictionary<ushort, Room> Rooms { private set; get; }

        private ushort _nextRoomID;
        private ClientManager _clientManager;

        public RoomManager()
        {
            Rooms = new Dictionary<ushort, Room>();

            PacketHandler.AddPacketHandler(PacketEnum.RequestListRooms, RequestListRooms);
            PacketHandler.AddPacketHandler(PacketEnum.CreateRoom, CreateRoomRequest);
            PacketHandler.AddPacketHandler(PacketEnum.RequestLobbyOwner, RequestLobbyOwner);
            PacketHandler.AddPacketHandler(PacketEnum.RequestNumLobbyMembers, RequestNumLobbyMembers);
            PacketHandler.AddPacketHandler(PacketEnum.SetLobbyData, SetLobbyData);
            PacketHandler.AddPacketHandler(PacketEnum.RequestLobbyMemberByIndexRequest, RequestLobbyMemberByIndexRequest);
            PacketHandler.AddPacketHandler(PacketEnum.RequestMemberData, RequestMemberData);
            PacketHandler.AddPacketHandler(PacketEnum.JoinRoom, JoinRoomRequest);
            PacketHandler.AddPacketHandler(PacketEnum.RequestLobbyData, RequestLobbyData);
            PacketHandler.AddPacketHandler(PacketEnum.SetLobbyOwner, SetLobbyOwner);
            PacketHandler.AddPacketHandler(PacketEnum.LeaveRoom, LeaveRoomRequest);
        }

        public void SetClientManager(ref ClientManager clientManager)
        {
            _clientManager = clientManager;
        }

        public void RemoveRoom(Room room)
        {
            if (!Rooms.ContainsKey(room.ID))
            {
                Debug.LogWarning($"Failed remove room. Reason=(!Rooms.ContainsKey(room.ID) == true) RoomID: {room.ID}");
                return;
            }
            Rooms.Remove(room.ID);
            ConsoleHelper.UpdateTitle(_clientManager.Clients.Count, Rooms.Count);
        }

        #region PACKETS
        private void RequestListRooms(Packet packet, Client client)
        {
            RequestRoomListPacket requsetRoomListPacket = new RequestRoomListPacket(packet);

            Debug.Log($"Request Room List. LobbyMode={requsetRoomListPacket.LobbyMode}; Diff={requsetRoomListPacket.Difficulty}");

            List<KeyValuePair<ushort, Room>> rooms = Rooms.ToList();
            rooms = rooms.Where(room => 
                room.Value.PlayerCount <= 3 && 
                room.Value.Mode == requsetRoomListPacket.LobbyMode && 
                room.Value.Difficulty == requsetRoomListPacket.Difficulty).ToList();

            RequestRoomListPacket resultRequstRoomListPacket = new RequestRoomListPacket(rooms);

            client.SendPacket(resultRequstRoomListPacket);
        }

        private void CreateRoomRequest(Packet packet, Client client)
        {
            CreateRoomPacket createRoomPacket = new CreateRoomPacket(packet);

            if (client.InRoom)
            {
                if (!Rooms.ContainsKey(client.Room.ID))
                {
                    Debug.LogWarning("Invalid room. Reason=(Rooms.ContainsKey(client.Room.ID) == false)");
                    client.ClearRoom();
                    return;
                }

                if (!client.Room.CheckHasPlayer(client))
                {
                    Debug.LogWarning("Invalid player room. Reason=(client.Room.CheckHasPlayer(client) == false)");
                    client.ClearRoom();
                    return;
                }

                client.Room.LeavePlayer(client);
                client.ClearRoom();
            }
            Room room = CreateNewRoom(client, JSBLobbyMode.Public, createRoomPacket.MaxPlayers);

            Debug.Log($"Created new room! ID={room.ID} Owner={client.NickName} PlayerID={client.PlayerID}");

            CreateRoomPacket resultPacket = new CreateRoomPacket(room.ID);
            client.SendPacket(resultPacket);
        }

        private void RequestLobbyOwner(Packet packet, Client client)
        {
            RequestLobbyOwnerPacket requestLobbyOwnerPacket = new RequestLobbyOwnerPacket(packet);

            if (!Rooms.ContainsKey(requestLobbyOwnerPacket.RoomID))
            {
                Debug.LogWarning("Invalid room. Reason=(Rooms.ContainsKey(requestLobbyOwnerPacket.RoomID) == false)");
                return;
            }

            ushort ownerID = Rooms[requestLobbyOwnerPacket.RoomID].Owner.PlayerID;
            RequestLobbyOwnerPacket resultRequestLobbyOwnerPacket = new RequestLobbyOwnerPacket(ownerID);
            client.SendPacket(resultRequestLobbyOwnerPacket);
        }

        private void RequestNumLobbyMembers(Packet packet, Client client)
        {
            RequestNumLobbyMembersPacket requestNumLobbyMembersPacket = new RequestNumLobbyMembersPacket(packet);

            if (!Rooms.ContainsKey(requestNumLobbyMembersPacket.RoomID))
            {
                Debug.LogWarning("Invalid room. Reason=(Rooms.ContainsKey(requestNumLobbyMembersPacket.RoomID) == false)");
                return;
            }

            byte count = (byte)Rooms[requestNumLobbyMembersPacket.RoomID].Players.Count;

            RequestNumLobbyMembersPacket resultRequestNumLobbyMembersPacket = new RequestNumLobbyMembersPacket(count);
            client.SendPacket(resultRequestNumLobbyMembersPacket);
        }

        private void SetLobbyData(Packet packet, Client client)
        {
            SetLobbyDataPacket setLobbyDataPacket = new SetLobbyDataPacket(packet);

            if (!Rooms.ContainsKey(setLobbyDataPacket.RoomID))
            {
                Debug.LogWarning("Invalid room. Reason=(Rooms.ContainsKey(setLobbyDataPacket.RoomID) == false)");
                return;
            }

            Rooms[setLobbyDataPacket.RoomID].SetValue(setLobbyDataPacket.Key, setLobbyDataPacket.Value);
        }

        private void RequestLobbyMemberByIndexRequest(Packet packet, Client client)
        {
            RequestLobbyMemberByIndexPacket requestLobbyMemberByIndexPacket = new RequestLobbyMemberByIndexPacket(packet);

            if (!Rooms.ContainsKey(requestLobbyMemberByIndexPacket.RoomID))
            {
                Debug.LogWarning("Invalid room. Reason=(Rooms.ContainsKey(requestLobbyMemberByIndexPacket.RoomID) == false)");
                return;
            }

            Room room = Rooms[requestLobbyMemberByIndexPacket.RoomID];

            if (requestLobbyMemberByIndexPacket.Index >= room.Players.Count)
            {
                Debug.LogWarning("Invalid index player. Reason=(requestLobbyMemberByIndexPacket.Index >= room.Players.Count)");
                return;
            }

            List<KeyValuePair<ushort, Client>> players = room.Players.ToList();

            ushort requestPlayerID = players[requestLobbyMemberByIndexPacket.Index].Value.PlayerID;

            RequestLobbyMemberByIndexPacket resultRequestLobbyMemberByIndexPacket = new RequestLobbyMemberByIndexPacket(requestPlayerID);

            client.SendPacket(resultRequestLobbyMemberByIndexPacket);
        }

        private void RequestMemberData(Packet packet, Client client)
        {
            RequestMemberDataPacket requestMemberDataPacket = new RequestMemberDataPacket(packet);

            if (!Rooms.ContainsKey(requestMemberDataPacket.RoomID))
            {
                Debug.LogWarning("Invalid room. Reason=(Rooms.ContainsKey(requestMemberDataPacket.RoomID) == false)");
                return;
            }

            Room room = Rooms[requestMemberDataPacket.RoomID];

            if (!_clientManager.Clients.ContainsKey(requestMemberDataPacket.PlayerID))
            {
                Debug.LogWarning("Invalid player. Reason=(_clientManager.Clients.ContainsKey(requestMemberDataPacket.PlayerID) == false)");
                return;
            }

            if(!room.Players.ContainsKey(requestMemberDataPacket.PlayerID))
            {
                Debug.LogWarning("Invalid player room. Reason=(room.Players.ContainsKey(requestMemberDataPacket.PlayerID) == false)");
                return;
            }

            Client requestPlayer = room.Players[requestMemberDataPacket.PlayerID];

            if (!requestPlayer.InRoom && room.Players.ContainsKey(requestMemberDataPacket.PlayerID))
            {
                Debug.LogWarning("Invalid player room. Reason=(!requestPlayer.InRoom && room.Players.ContainsKey(requestMemberDataPacket.PlayerID))");
                return;
            }

            if (requestPlayer.InRoom && requestPlayer.Room.ID != room.ID)
            {
                Debug.LogWarning("Invalid player room. Reason=(requestPlayer.InRoom && requestPlayer.Room.ID != room.ID)");
                return;
            }

            if (!requestPlayer.InRoom)
            {
                Debug.LogWarning("Invalid player room. Reason=(requestPlayer.InRoom == false)");
                return;
            }

            string value = requestPlayer.GetValue(requestMemberDataPacket.Key);

            RequestMemberDataPacket resultRequestMemberDataPacket = new RequestMemberDataPacket(value);

            client.SendPacket(resultRequestMemberDataPacket);
        }

        private void JoinRoomRequest(Packet packet, Client client)
        {
            JoinRoomPacket joinRoomPacket = new JoinRoomPacket(packet);
            bool connectedInRoom = false;

            if (Rooms.ContainsKey(joinRoomPacket.RoomID))
            {
                if (client.InRoom)
                {
                    Debug.LogWarning($"Player has room.");
                    client.Room.LeavePlayer(client);
                    client.ClearRoom();
                }

                Room room = Rooms[joinRoomPacket.RoomID];

                connectedInRoom = room.TryAddPlayer(client);
            }
            else
            {
                Debug.LogWarning($"Invalid room. Reason=(Rooms.ContainsKey(joinRoomPacket.RoomID))");
            }

            JoinRoomPacket resultJoinRoomPacket = new JoinRoomPacket(connectedInRoom);
            client.SendPacket(resultJoinRoomPacket);
        }

        private void RequestLobbyData(Packet packet, Client client)
        {
            RequestLobbyDataPacket requestLobbyDataPacket = new RequestLobbyDataPacket(packet);

            if (!Rooms.ContainsKey(requestLobbyDataPacket.RoomID))
            {
                Debug.LogWarning("Invalid room. Reason=(Rooms.ContainsKey(requestLobbyDataPacket.RoomID) == false)");
                return;
            }

            string value = Rooms[requestLobbyDataPacket.RoomID].GetValue(requestLobbyDataPacket.Key);

            RequestLobbyDataPacket resultRequestLobbyDataPacket = new RequestLobbyDataPacket(value);

            client.SendPacket(resultRequestLobbyDataPacket);
        }

        private void SetLobbyOwner(Packet packet, Client client)
        {
            SetLobbyOwnerPacket setLobbyOwnerPacket = new SetLobbyOwnerPacket(packet);

            if (!Rooms.ContainsKey(setLobbyOwnerPacket.RoomID))
            {
                Debug.LogWarning("Invalid room. Reason=(Rooms.ContainsKey(setLobbyOwnerPacket.RoomID) == false)");
                return;
            }

            if (!_clientManager.Clients.ContainsKey(setLobbyOwnerPacket.PlayerID))
            {
                Debug.LogWarning("Invalid player. Reason=(_clientManager.Clients.ContainsKey(setLobbyOwnerPacket.PlayerID) == false)");
                return;
            }

            Client clientOwner = _clientManager.Clients[setLobbyOwnerPacket.PlayerID];

            if (!client.InRoom || client.Room.ID != setLobbyOwnerPacket.RoomID)
            {
                Debug.LogWarning("Invalid current client room. RoomManager:SetLobbyOwner");
                return;
            }

            if (!clientOwner.InRoom || clientOwner.Room.ID != setLobbyOwnerPacket.RoomID)
            {
                Debug.LogWarning("Invalid new owner client room. RoomManager:SetLobbyOwner");
                return;
            }

            Room room = Rooms[setLobbyOwnerPacket.RoomID];

            room.SetOwner(clientOwner);

            _clientManager.SendToAllPlayersChangedPlayerDataPacket(room.ID, clientOwner);
        }

        private void LeaveRoomRequest(Packet packet, Client client)
        {
            LeaveRoomPacket leaveRoomPacket = new LeaveRoomPacket(packet);

            if (!Rooms.ContainsKey(leaveRoomPacket.RoomID))
            {
                Debug.LogWarning("Invalid room. Reason=(Rooms.ContainsKey(leaveRoomPacket.RoomID) == false)");
                return;
            }

            Room room = Rooms[leaveRoomPacket.RoomID];

            room.LeavePlayer(client);
        }
#endregion PACKETS

        private Room CreateNewRoom(Client owner, JSBLobbyMode lobbyMode, int maxPlayers)
        {
            ushort roomID = GetFreeRoomID();
            Room room = new Room(roomID, maxPlayers, lobbyMode, owner);
            owner.SetRoom(room);

            Rooms.Add(roomID, room);
            ConsoleHelper.UpdateTitle(_clientManager.Clients.Count, Rooms.Count);
            return room;
        }

        private ushort GetFreeRoomID()
        {
            _nextRoomID++;
            while (Rooms.ContainsKey(_nextRoomID))
            {
                _nextRoomID++;
            }
            return _nextRoomID;
        }
    }
}
