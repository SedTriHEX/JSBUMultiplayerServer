using JustShapesBeatsMultiplayerServer.Data;
using JustShapesBeatsMultiplayerServer.Packets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace JustShapesBeatsMultiplayerServer.Managers
{
    class ClientManager
    {
        public const ushort InvalidPlayerID = 0;

        public Dictionary<ushort, Client> Clients { private set; get; }

        private ushort _nextPlayerID;

        private RoomManager _roomManager;

        public ClientManager()
        {
            Clients = new Dictionary<ushort, Client>();

            PacketHandler.AddPacketHandler(PacketEnum.ConnectedClient, ConnectedClient);
            PacketHandler.AddPacketHandler(PacketEnum.SetMemberData, SetMemberData);
            PacketHandler.AddPacketHandler(PacketEnum.RequestFriendPersonaName, RequestFriendPersonaName);
            PacketHandler.AddPacketHandler(PacketEnum.PingPong, PongClient);
        }

        public void StartPingClients()
        {
            Task.Run(PingClients);
        }

        public void StartCheckPlayersAndRooms()
        {
            Task.Run(CheckPlayersAndRooms);
        }

        public void SetRoomManager(ref RoomManager roomManager)
        {
            _roomManager = roomManager;
        }

        public void AddClient(Client client)
        {
            if (Clients.ContainsKey(client.PlayerID))
            {
                Debug.LogError("Failed add client. Clients.ContainsKey(client.PlayerID) == true");
                return;
            }

            if (!client.InitializedTcpClient)
                client.InitializeTcp();

            Clients.Add(client.PlayerID, client);

            Debug.Log($"New connection! IP = {client.GetPlayerIP()} PlayerID = {client.PlayerID}");
            Debug.Log($"Current count clients: {Clients.Count}");
        }

        public ushort GetFreePlayerID()
        {
            if (Clients.Count > Constants.MaxPlayers)
                return InvalidPlayerID;

            do
            {
                _nextPlayerID++;
            } 
            while (Clients.ContainsKey(_nextPlayerID) || _nextPlayerID == InvalidPlayerID);

            return _nextPlayerID;
        }

        public bool CheckContainsPlayerID(ushort playerID)
        {
            return Clients.ContainsKey(playerID);
        }

        public void HandleP2PPacket(byte[] bytes, bool fromTCP)
        {
            P2PPacket packet = new P2PPacket(bytes);

            if (!Clients.ContainsKey(packet.SenderPlayerID))
            {
                Debug.LogError($"Failed p2p packet. Sender player invalid. (P2P_ER1)");
                return;
            }

            if (!Clients.ContainsKey(packet.ReceiverPlayerID))
            {
                Debug.LogError("Failed p2p packet. Receiver player invalid. (P2P_ER2)");

                if (!Clients.ContainsKey(packet.SenderPlayerID))
                {
                    Debug.LogError($"Failed p2p packet. Sender player invalid. (P2P_ER3)");
                    return;
                }

                Client senderPlayer = Clients[packet.SenderPlayerID];
                if (!senderPlayer.InRoom)
                {
                    Debug.LogError($"Failed p2p packet. Sender player invalid. (P2P_ER4)");
                    return;
                }
                if (!senderPlayer.Room.LeavePlayer(packet.ReceiverPlayerID))
                {
                    Debug.LogError($"Failed leave receiver player. Send leave packet... (P2P_ER5)");
                    senderPlayer.Room.SendLeavePlayerPacket(packet.ReceiverPlayerID);
                }
                else
                {
                    Debug.LogWarning("TODO: ClearRoom() maybe?");
                    //client.ClearRoom();
                }
                return;
            }

            Client receiverClient = Clients[packet.ReceiverPlayerID];

            List<byte> p2pBytes = new List<byte>();
            p2pBytes.Add(PacketType.P2P);
            p2pBytes.AddRange(BitConverter.GetBytes(packet.SenderPlayerID));
            p2pBytes.AddRange(packet.Data);
            if (fromTCP)
            {
                AnalyzeP2PPacket(packet.Data, packet.SenderPlayerID, packet.ReceiverPlayerID);
                receiverClient.SendBytesTcp(p2pBytes.ToArray());
                return;
            }
            if (!receiverClient.ConnectedByUdp)
            {
                Debug.LogError($"Failed send p2p packet by udp. Reason=(NoConnectedByUdp == false) (P2P_ER6)");
                return;
            }
            receiverClient.SendBytesUdp(p2pBytes.ToArray());
        }

        public void SetUdpClient(ushort playerID, IPEndPoint clientPoint)
        {
            Clients[playerID].SetUdpClient(clientPoint);
        }

        public void DisconnectClient(Client client)
        {
            if (!client.Disconnected)
            {
                client.Disconnect(false);
            }

            if (Clients.ContainsKey(client.PlayerID))
            {
                Clients.Remove(client.PlayerID);
                Debug.Log($"Disconnected client. PlayerID={client.PlayerID} IP={client.GetPlayerIP()}");
                Debug.Log($"Current count clients: {Clients.Count}");

                if (client.InRoom)
                {
                    client.Room.LeavePlayer(client);
                    client.ClearRoom();
                }
            }
            else
            {
                Debug.LogWarning($"Failed remove client. PlayerID={client.PlayerID}");
            }
        }

        #region PACKETS
        private void ConnectedClient(Packet packet, Client client)
        {
            ConnectedClientPacket connectedClientPacket = new ConnectedClientPacket(packet);

            string nickName = connectedClientPacket.NickName;

            if (nickName == null || nickName.Length == 0)
                nickName = "Player " + Helper.Random.Next(1, 99999999);

            Debug.Log($"New connected client! PlayerID={client.PlayerID}; NickName={nickName}; GameVersion={connectedClientPacket.GameVersion};");

            client.SetNickName(nickName);

            ConnectedClientPacket resultConnectedPacket = new ConnectedClientPacket(ConnectedClientPacket.OK, client.PlayerID);
            client.SendPacket(resultConnectedPacket.Packet);
        }

        private void SetMemberData(Packet packet, Client client)
        {
            SetMemberDataPacket setMemberDataPacket = new SetMemberDataPacket(packet);

            if (!_roomManager.Rooms.ContainsKey(setMemberDataPacket.RoomID))
            {
                Debug.LogWarning($"Invalid room. Reason=(_roomManager.Rooms.ContainsKey(setMemberDataPacket.RoomID) == false)");
                return;
            }

            if (!client.InRoom)
            {
                Debug.LogWarning($"Invalid player room. Reason=(client.InRoom == false)");
                return;
            }

            if (client.Room.ID != setMemberDataPacket.RoomID)
            {
                Debug.LogWarning($"Invalid player room. Reason=(client.Room.ID != setMemberDataPacket.RoomID)");
                return;
            }

            client.SetValue(setMemberDataPacket.Key, setMemberDataPacket.Value);

            SendToAllPlayersChangedPlayerDataPacket(setMemberDataPacket.RoomID, client);
        }

        public void SendToAllPlayersChangedPlayerDataPacket(ushort roomID, Client client)
        {
            ChangedPlayerDataPacket changedPlayerDataPacket = new ChangedPlayerDataPacket(roomID, client.PlayerID);

            foreach (KeyValuePair<ushort, Client> player in Clients)
            {
                if (player.Value.PlayerID == client.PlayerID)
                    continue;
                player.Value.SendPacket(changedPlayerDataPacket);
            }
        }

        private void RequestFriendPersonaName(Packet packet, Client client)
        {
            RequestFriendPersonaNamePacket requestFriendPersonaNamePacket = new RequestFriendPersonaNamePacket(packet);

            if (!Clients.ContainsKey(requestFriendPersonaNamePacket.PlayerID))
            {
                Debug.Log("Invalid player. Reason=(!_clientManager.Clients.ContainsKey(requestFriendPersonaNamePacket.PlayerID))");
                return;
            }

            string nickName = Clients[requestFriendPersonaNamePacket.PlayerID].NickName;

            RequestFriendPersonaNamePacket resultRequestFriendPersonaNamePacket = new RequestFriendPersonaNamePacket(nickName);

            client.SendPacket(resultRequestFriendPersonaNamePacket);
        }

        private void PongClient(Packet packet, Client client)
        {
            client.UpdatePongDateTime(DateTime.Now);

            PingPongPacket pingPongPacket = new PingPongPacket(packet);

            if (!pingPongPacket.AvailableRequestData) return;

            if (pingPongPacket.PlayerID != client.PlayerID)
            {
                Debug.LogError("Invalid player id. Reason=(pingPongPacket.PlayerID != client.PlayerID). Disconnect this client...");
                DisconnectClient(client);
                return;
            }

            if (pingPongPacket.InRoom != client.InRoom)
            {
                Debug.LogWarning("Invalid player room. Reason=(pingPongPacket.InRoom != client.InRoom)");
                return;
            }

            if (!client.InRoom) return;

            if (pingPongPacket.RoomID != client.Room.ID)
            {
                Debug.LogWarning("Invalid player room. Reason=(pingPongPacket.RoomID != client.Room.ID)");
                return;
            }

            if (pingPongPacket.PlayerIDOwnerOfRoom != client.Room.Owner.PlayerID)
            {
                Debug.LogWarning("Invalid player owner room. Reason=(pingPongPacket.PlayerIDOwnerOfRoom != client.Room.Owner.PlayerID)");
                return;
            }
        }

        #endregion

        private void AnalyzeP2PPacket(byte[] bytes, ushort senderPlayerID, ushort receiverPlayerID)
        {
            // TODO: AnalyzeP2PPacket
            BinaryReader binaryReader = new BinaryReader(new MemoryStream(bytes));
            ushort packetID = binaryReader.ReadUInt16();
            ulong clientID = binaryReader.ReadUInt64();
            //Debug.Log($"[P2P] PacketID = {packetID}; ClientID = {clientID}");
        }

        private void PingClients()
        {
            Debug.Log("Start ping clients...");
            HashSet<Client> clients = new HashSet<Client>();
            while (Program.ServerManagerInstance.IsListening)
            {
                Thread.Sleep(Constants.CountDownPingClients);
                if (!Program.ServerManagerInstance.IsListening)
                    break;

                foreach (KeyValuePair<ushort, Client> client in Clients)
                    clients.Add(client.Value);

                foreach (KeyValuePair<ushort, Room> roomKeyValue in _roomManager.Rooms)
                {
                    Room room = roomKeyValue.Value;

                    foreach (KeyValuePair<ushort, Client> player in room.Players)
                        clients.Add(player.Value);
                }

                foreach (Client client in clients)
                {
                    if (client.Disconnected)
                    {
                        if (!CheckContainsPlayerID(client.PlayerID) && client.InRoom)
                        {
                            bool statusLeavePlayer = client.Room.LeavePlayer(client);
                            if (!statusLeavePlayer)
                                Debug.LogWarning($"Failed leave disconnected player. PlayerID={client.PlayerID}");
                            continue;
                        }
                        DisconnectClient(client);
                        continue;
                    }
                    if ((DateTime.Now - client.LastTimePong).TotalSeconds > Constants.TotalSecondsForDisconnectClient)
                    {
                        DisconnectClient(client);
                        continue;
                    }

                    PingPongPacket packet = new PingPongPacket();
                    client.SendPacket(packet);
                }

                clients.Clear();
            }
        }

        private void CheckPlayersAndRooms()
        {
            foreach (KeyValuePair<ushort, Client> clientKeyValue in Clients)
            {
                Client client = clientKeyValue.Value;
                if (client.Disconnected)
                {
                    DisconnectClient(client);
                    return;
                }
                if (client.InRoom)
                {
                    Room room = client.Room;
                }
            }

            // TODO: CheckPlayersAndRooms
        }
    }
}
