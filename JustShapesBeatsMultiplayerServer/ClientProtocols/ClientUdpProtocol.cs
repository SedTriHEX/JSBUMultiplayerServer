using System.Net;

namespace JustShapesBeatsMultiplayerServer.ClientProtocols
{
    class ClientUdpProtocol
    {
        private IPEndPoint _endPoint;

        public void Init(IPEndPoint clientPoint)
        {
            _endPoint = clientPoint;
        }

        public void SendBytes(byte[] bytes)
        {
            Program.ServerManagerInstance.SendBytesByUdpProtocol(_endPoint, bytes);
        }

        public void Disconnect()
        {
            _endPoint = null;
        }
    }
}
