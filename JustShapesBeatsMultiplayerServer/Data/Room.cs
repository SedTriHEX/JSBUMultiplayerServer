using JustShapesBeatsMultiplayerServer.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JustShapesBeatsMultiplayerServer.Data
{
    public class Room
    {
        public ushort ID { private set; get; }

        public JSBLobbyMode Mode { private set; get; }

        public JSBDifficulty Difficulty 
        {
            get
            {
                if (Values == null || !Values.ContainsKey("JSB_Diff"))
                    return JSBDifficulty.Normal;
                return (JSBDifficulty)int.Parse(GetValue("JSB_Diff"));
            }
        }

        public Client Owner { private set; get; }

        public Dictionary<ushort, Client> Players { private set; get; }

        public int MaxPlayers { private set; get; }

        public int PlayerCount 
        {
            get => 0;
        }

        public Dictionary<string, string> Values { private set; get; }

        public Room(ushort id, int maxPlayers, JSBLobbyMode mode, Client owner)
        {
            ID = id;
            Mode = mode;
            Owner = owner;
            MaxPlayers = maxPlayers;


            Values = new Dictionary<string, string>();
            Players = new Dictionary<ushort, Client>();
            Players.Add(owner.PlayerID, owner);
        }

        public string GetValue(string key)
        {
            if (!Values.ContainsKey(key))
                return "";
            return Values[key];
        }

        public void SetValue(string key, string value)
        {
            if (key == "JSB_Mode")
            {
                Mode = (JSBLobbyMode)int.Parse(value);
            }

            if (!Values.ContainsKey(key))
            {
                Values.Add(key, value);
                return;
            }
            Values[key] = value;
        }

        public bool CheckHasPlayer(Client client)
        {
            return Players.ContainsKey(client.PlayerID);
        }

        public bool LeavePlayer(ushort playerID)
        {
            if (!Players.ContainsKey(playerID))
            {
                Debug.LogWarning($"Failed to leave the player. Reason=([L1] Unknown client.) PlayerID={playerID}");
                return false;
            }
            else if (Program.ServerManagerInstance.GetClientManager().CheckContainsPlayerID(playerID))
            {
                Client client = Program.ServerManagerInstance.GetClientManager().Clients[playerID];
                if (client.InRoom && client.Room.ID == ID)
                {
                    Debug.LogWarning($"Bug? Client.Room.ID == (Room[{ID}]).ID, but player is not in the list of players.");
                }
                return false;
            }

            return TryLeavePlayer(Players[playerID]);
        }

        public bool LeavePlayer(Client client)
        {
            if (!Players.ContainsKey(client.PlayerID))
            {
                Debug.LogWarning($"Failed to leave the player. Reason=([L2] Unknown client.) PlayerID={client.PlayerID}");
                return false;
            }

            return TryLeavePlayer(client);
        }

        private bool TryLeavePlayer(Client client)
        {
            Players.Remove(client.PlayerID);
            client.ClearRoom();

            Debug.Log($"Player[{client.PlayerID}] leaved froom Room[{ID}]");

            if (Players.Count == 0)
            {
                Program.ServerManagerInstance.GetRoomManager().RemoveRoom(this);
                return true;
            }

            if (client.PlayerID == Owner.PlayerID)
            {
                ushort[] players = Players.Keys.ToArray();
                int index = Helper.Random.Next(0, players.Length);
                Owner = Players[players[index]];
            }

            SendLeavePlayerPacket(client.PlayerID);
            return true;
        }

        public void SendLeavePlayerPacket(ushort leavedPlayerID)
        {
            PlayerHasLeftRoomPacket playerHasLeftRoomPacket = new PlayerHasLeftRoomPacket(ID, leavedPlayerID);

            foreach (KeyValuePair<ushort, Client> player in Players)
            {
                if (player.Key == leavedPlayerID)
                    continue;
                player.Value.SendPacket(playerHasLeftRoomPacket);
            }
        }

        public bool TryAddPlayer(Client client)
        {
            if (Players.Count == MaxPlayers) return false;

            if (Players.ContainsKey(client.PlayerID))
            {
                LeavePlayer(client);
                return false;
            }

            Players.Add(client.PlayerID, client);
            client.SetRoom(this);

            Debug.Log($"Player (NickName={client.NickName}; ID={client.PlayerID}) joined to room! (RoomID={ID})");

            SendJoinedPlayerPacket(client);

            return true;
        }

        private void SendJoinedPlayerPacket(Client client)
        {
            JoinedPlayerPacket joinedPlayerPacket = new JoinedPlayerPacket(ID, client.PlayerID);

            foreach (KeyValuePair<ushort, Client> player in Players)
            {
                if (player.Key == client.PlayerID)
                    continue;
                player.Value.SendPacket(joinedPlayerPacket);
            }
        }
    }

    public enum JSBLobbyMode : byte
    {
        Public,
        Private,
        Community
    }

    public enum JSBDifficulty : byte
    {
        Easy,
        Normal,
        Hardcore
    }
}
