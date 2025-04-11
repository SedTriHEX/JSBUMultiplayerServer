using JustShapesBeatsMultiplayerServer.Data;
using JustShapesBeatsMultiplayerServer.Managers;
using JustShapesBeatsMultiplayerServer.Packets;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Reflection;
using System.Threading.Tasks;

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
                Task.Run(BeginReceive);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed init tcp connect. Reason: {ex}");
                Client.Disconnect();
            }
        }

        private async void BeginReceive()
        {
            while (_stream != null && TcpClient.Connected)
            {
                (bool successReadPacketSizeBuffer, byte[] packetSizeBuffer) = await TryReadBytesAsync(sizeof(ushort));

                if (!successReadPacketSizeBuffer)
                {
                    Client.Disconnect();
                    break; 
                }

                ushort length = BitConverter.ToUInt16(packetSizeBuffer);

                (bool successReadBuffer, byte[] buffer) = await TryReadBytesAsync(length);

                if (!successReadBuffer)
                {
                    Client.Disconnect();
                    break;
                }

                ReceviedPacket(buffer, 0, length);
            }
        }

        private async ValueTask<(bool, byte[])> TryReadBytesAsync(int count)
        {
            try
            {
                byte[] buffer = new byte[count];
                int totalReaded = 0;
                while (totalReaded < count)
                {
                    int readed = await _stream.ReadAsync(buffer, totalReaded, count - totalReaded);

                    if (readed == 0)
                        return (false, new byte[0]);

                    totalReaded += readed;
                }
                return (true, buffer);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed read bytes from NetworkBuffer. {ex}");
                return (false, new byte[0]);
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
