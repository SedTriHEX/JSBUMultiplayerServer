using JustShapesBeatsMultiplayerServer.ClientProtocols;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace JustShapesBeatsMultiplayerServer.Data
{
    public class Client
    {
        public ushort PlayerID { private set; get; }

        public bool InitializedTcpClient { private set; get; }

        public string Guid { private set; get; }

        public string NickName { private set; get; }

        public bool Disconnected { private set; get; }

        public DateTime LastTimePong { private set; get; }

        public bool InRoom => Room != null;

        public bool ConnectedByUdp { get => _udp != null; }

        public Room Room { private set; get; }

        public Dictionary<string, string> ValuesInRoom { private set; get; }


        private TcpClient _tcpClient;

        private string _cachedPlaeyrIP;

        private ClientTcpProtocol _tcp;
        private ClientUdpProtocol _udp;

        public Client(ushort playerID, TcpClient tcpClient)
        {
            PlayerID = playerID;
            _tcpClient = tcpClient;

            _cachedPlaeyrIP = _tcpClient.Client.RemoteEndPoint.ToString();
            Guid = Helper.GenerateGuid();

            LastTimePong = DateTime.Now;
        }

        public string GetPlayerIP()
        {
            return _cachedPlaeyrIP;
        }

        public void InitializeTcp()
        {
            _tcp = new ClientTcpProtocol();
            _tcp.Init(this, _tcpClient);

            InitializedTcpClient = true;
        }

        public void SetUdpClient(IPEndPoint clientPoint)
        {
            _udp = new ClientUdpProtocol();
            _udp.Init(clientPoint);
        }

        public void SetNickName(string name)
        {
            NickName = name;
        }

        public void SendPacket(Packet packet)
        {
            if (Disconnected)
                return;
            _tcp.SendBytes(packet.ToBytes());
        }

        public void SendPacket(IPacket packet)
        {
            if (Disconnected)
                return;
            _tcp.SendBytes(packet.Packet.ToBytes());
        }

        public void SendBytesTcp(byte[] bytes)
        {
            if (Disconnected)
                return;
            _tcp.SendBytes(bytes);
        }

        public void SendBytesUdp(byte[] bytes)
        {
            if (Disconnected)
                return;
            _udp.SendBytes(bytes);
        }

        public void UpdatePongDateTime(DateTime dateTime)
        {
            LastTimePong = dateTime;
        }

        public void Disconnect(bool invokeDisconnectClientInClientManager = true)
        {
            if (Disconnected)
                return;

            _tcp.Disconnect();
            if(_udp != null)
                _udp.Disconnect();

            SetTrueDisconnected();

            if(invokeDisconnectClientInClientManager)
                Program.ServerManagerInstance.GetClientManager().DisconnectClient(this);
        }

        public void SetTrueDisconnected()
        {
            Disconnected = true;
        }

        public void SetRoom(Room room)
        {
            Room = room;
            ValuesInRoom = new Dictionary<string, string>();
        }

        public void SetValue(string key, string value)
        {
            if (ValuesInRoom == null)
            {
                Debug.LogWarning("Failed client set value. Room == null");
                return;
            }
            ValuesInRoom[key] = value;
        }

        public string GetValue(string key)
        {
            if (ValuesInRoom == null)
            {
                Debug.LogWarning("Failed client get value. Room == null");
                return "";
            }
            if (!ValuesInRoom.ContainsKey(key))
                return "";
            return ValuesInRoom[key];
        }

        public void ClearRoom()
        {
            Room = null;
            ValuesInRoom.Clear();
            ValuesInRoom = null;
        }
    }
}
