using JustShapesBeatsMultiplayerServer.Data;
using JustShapesBeatsMultiplayerServer.Managers;
using System;
using System.Net;
using System.Net.Sockets;

namespace JustShapesBeatsMultiplayerServer.Controllers
{
    class TcpController
    {
        public bool IsListening { private set; get; }

        public TcpListener Listener;

        private ushort _port;
        private ClientManager _clientManager;

        public TcpController(ushort port, ref ClientManager clientManager)
        {
            _port = port;
            _clientManager = clientManager;
        }

        public void Start()
        {
            Listener = new TcpListener(IPAddress.Any, _port);
            Listener.Start();
            Listener.BeginAcceptTcpClient(ConnectCallback, null);
            IsListening = true;
        }

        public void Stop()
        {
            Listener.Stop();
            IsListening = false;
        }

        private void ConnectCallback(IAsyncResult result)
        {
            TcpClient client = Listener.EndAcceptTcpClient(result);
            Listener.BeginAcceptTcpClient(ConnectCallback, null);

            ushort playerID = _clientManager.GetFreePlayerID();
            if (!CheckLimitPlayers() || playerID == ClientManager.InvalidPlayerID)
            {
                client.Close();
                return;
            }
            _clientManager.AddClient(new Client(playerID, client));
        }

        private bool CheckLimitPlayers()
        {
            return _clientManager.Clients.Count < Constants.MaxPlayers;
        }
    }
}
