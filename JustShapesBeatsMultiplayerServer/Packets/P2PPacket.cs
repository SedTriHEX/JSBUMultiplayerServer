using System;

namespace JustShapesBeatsMultiplayerServer.Packets
{
    class P2PPacket
    {
        public ushort SenderPlayerID { private set; get; }

        public ushort ReceiverPlayerID { private set; get; }

        public byte[] Data { private set; get; }

        public P2PPacket(byte[] bytes)
        {
            SenderPlayerID = BitConverter.ToUInt16(bytes, 1);
            ReceiverPlayerID = BitConverter.ToUInt16(bytes, 3);

            Data = new byte[bytes.Length - 5];
            Array.Copy(bytes, 5, Data, 0, bytes.Length - 5);
        }
    }
}
