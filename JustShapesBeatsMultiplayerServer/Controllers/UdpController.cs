using JustShapesBeatsMultiplayerServer.Managers;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace JustShapesBeatsMultiplayerServer.Controllers
{
    class UdpController
    {
        private const int LengthMinimumUdpPacket = 3;
        private const byte IsConnectUdpClientPacket = 22;

        private static UdpClient _listener;

        private ushort _port;
        private ClientManager _clientManager;

        public UdpController(ushort port, ref ClientManager clientManager)
        {
            _port = port;
            _clientManager = clientManager;
        }

        public void Start()
        {
            _listener = new UdpClient(_port);
            _listener.BeginReceive(ReceiveCallback, null);
        }

        public void Stop()
        {
            _listener.Close();
            _listener.Dispose();
        }

        private void ReceiveCallback(IAsyncResult result)
        {
            try
            {
                IPEndPoint clientPoint = new IPEndPoint(IPAddress.Any, 0);

                byte[] bytes = _listener.EndReceive(result, ref clientPoint);

                _listener.BeginReceive(ReceiveCallback, null);

                if (bytes.Length < LengthMinimumUdpPacket) return;

                if (!IsConnectPacket(bytes))
                {
                    _clientManager.HandleP2PPacket(bytes, false);
                    return;
                }

                ushort playerID = BitConverter.ToUInt16(bytes, 1);
                if (!_clientManager.CheckContainsPlayerID(playerID))
                {
                    Debug.LogError($"Invalid playerID for connect udp. CheckContainsPlayerID(playerID) == false, Received ID: {playerID}");
                    return;
                }
                _clientManager.SetUdpClient(playerID, clientPoint);
            }
            catch (Exception ex)
            {
                Debug.LogError($"(UDP) Failed receive bytes. Reason: {ex}");
            }
        }

        public void SendBytes(IPEndPoint endPoint, byte[] bytes)
        {
            try
            {
                if (endPoint != null)
                    _listener.BeginSend(bytes, bytes.Length, endPoint, null, null);
            }
            catch (Exception ex)
            {
                Debug.LogError($"(UDP) Failed send bytes. Reason: {ex}");
            }
        }

        private bool IsConnectPacket(byte[] bytes)
        {
            // мне лень переписывать клиентскую часть мультиплеера
            return bytes[0] == IsConnectUdpClientPacket && bytes.Length == 3;
        }
    }
}
