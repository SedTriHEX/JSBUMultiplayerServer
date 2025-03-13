using JustShapesBeatsMultiplayerServer.Data;
using JustShapesBeatsMultiplayerServer.Managers;
using JustShapesBeatsMultiplayerServer.Packets;
using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace JustShapesBeatsMultiplayerServer.ClientProtocols
{
    class ClientTcpProtocol
    {
        public TcpClient TcpClient { private set; get; }

        public Client Client { private set; get; }

        private NetworkStream _stream;
        private byte[] _receiveBuffer;

        public bool Disconnected { private set; get; }

        public void Init(Client client, TcpClient tcpClient)
        {
            TcpClient = tcpClient;
            Client = client;

            InitConnect();
        }

        public void SendBytes(byte[] bytes)
        {
            try
            {
                if (Disconnected || Client.Disconnected)
                {
                    if (Client == null) return;
                    ClientManager clientManager = Program.ServerManagerInstance.GetClientManager();
                    if (clientManager.Clients.ContainsKey(Client.PlayerID))
                    {
                        if (Client.Disconnected) Client.Disconnect();
                        else clientManager.DisconnectClient(Client);
                    }
                    return;
                }
                if (!TcpClient.Connected)
                {
                    Debug.LogError($"Failed send bytes by tcp protocol. Reason: (!TcpClient.Connected) == true");
                    Client.Disconnect();
                    return;
                }

                int length = bytes.Length;

                List<byte> bytesToSend = new List<byte>();
                bytesToSend.AddRange(BitConverter.GetBytes((ushort)length));
                bytesToSend.AddRange(bytes);

                _stream.BeginWrite(bytesToSend.ToArray(), 0, bytesToSend.Count, null, null);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed send bytes by tcp protocol. Reason: {ex}");
                Client.Disconnect();
            }
        }

        private void InitConnect()
        {
            try
            {
                TcpClient.ReceiveBufferSize = Constants.TcpDataBufferSize;
                TcpClient.SendBufferSize = Constants.TcpDataBufferSize;
                _stream = TcpClient.GetStream();

                _receiveBuffer = new byte[Constants.TcpDataBufferSize];
                _stream.BeginRead(_receiveBuffer, 0, Constants.TcpDataBufferSize, ReceiveCallback, null);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed init tcp connect. Reason: {ex}");
                Client.Disconnect();
            }
        }

        private void ReceiveCallback(IAsyncResult result)
        {
            try
            {
                if (_stream == null || !_stream.CanRead || !_stream.CanWrite || !TcpClient.Connected)
                {
                    //Debug.Log($"Invalid receive tcp data. (_stream == null || !_stream.CanRead || !_stream.CanWrite || !TcpClient.Connected) == true");
                    return;
                }

                int bufferLength = _stream.EndRead(result);
                if (bufferLength <= 0)
                {
                    Client.Disconnect();
                    return;
                }

                if (_receiveBuffer == null)
                    return;

                ProcessingData(_receiveBuffer, bufferLength, 0);

                _stream.BeginRead(_receiveBuffer, 0, Constants.TcpDataBufferSize, ReceiveCallback, null);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Invalid receive tcp data. {ex}");
                Client.Disconnect();
            }
        }

        private void ProcessingData(byte[] input, int receivedBytes, int index)
        {
            if (receivedBytes - index == 1)
            {
                byte[] buffer = new byte[2] { input[index], 0 };
                _stream.Read(buffer, 1, 1);

                ProcessingData(buffer, 2, 0);
                return;
            }

            ushort packetLength = BitConverter.ToUInt16(input, index);
            int count = receivedBytes - index - 2;

            if (count >= packetLength)
            {
                ReceviedPacket(input, index + 2, packetLength);

                if (count > packetLength)
                    ProcessingData(input, receivedBytes, index + packetLength + 2);
            }
            else
            {
                byte[] buffer = new byte[packetLength];

                _stream.Read(buffer, count, packetLength - count);

                Array.Copy(_receiveBuffer, index + 2, buffer, 0, count);

                ReceviedPacket(buffer, 0, packetLength);
            }
        }

        private void ReceviedPacket(byte[] data, int index, int length)
        {
            byte packetType = data[index];

            if (packetType == PacketType.Default)
            {
                Packet packet = new Packet(data, index);
                PacketHandler.Invoke(packet.PacketId, packet, Client);
                return;
            }
            else if (packetType == PacketType.P2P)
            {
                byte[] bufferPacket = new byte[length];
                Array.Copy(data, index, bufferPacket, 0, length);
                Program.ServerManagerInstance.GetClientManager().HandleP2PPacket(bufferPacket, true);
            }
        }

        public void Disconnect()
        {
            if (Disconnected)
                return;

            try
            {
                _stream.Close();
                _stream.Dispose();
            }
            catch { }

            try
            {
                TcpClient.Close();
                TcpClient.Dispose();
            }
            catch { }

            _stream = null;
            TcpClient = null;
            Client = null;
            _receiveBuffer = null;

            Disconnected = true;
        }
    }
}
