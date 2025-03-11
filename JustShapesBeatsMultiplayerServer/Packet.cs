using JustShapesBeatsMultiplayerServer.Packets;
using System;
using System.Collections.Generic;
using System.Text;

namespace JustShapesBeatsMultiplayerServer
{
    public class Packet
    {
        public byte PacketId { private set; get; }

        private List<byte> _bytes;
        private byte[] _readableBuffer;
        private int _pos;

        public Packet(byte id)
        {
            PacketId = id;

            _bytes = new List<byte>
            {
                PacketType.Default
            };

            Add(PacketId);
        }

        public Packet(byte[] bytes, int position = 0)
        {
            _pos = position;
            _readableBuffer = bytes;
            _pos += 1;
            PacketId = _readableBuffer[_pos++];
        }

        public byte[] ToBytes() => _bytes.ToArray();

        public void Add(bool value) => _bytes.AddRange(BitConverter.GetBytes(value));

        public void Add(byte value) => _bytes.Add(value);
        public void Add(int value) => _bytes.AddRange(BitConverter.GetBytes(value));

        public void Add(float value) => _bytes.AddRange(BitConverter.GetBytes(value));

        public void Add(ushort value) => _bytes.AddRange(BitConverter.GetBytes(value));

        public void Add(ulong value) => _bytes.AddRange(BitConverter.GetBytes(value));

        public void Add(long value) => _bytes.AddRange(BitConverter.GetBytes(value));

        public void Add(string value)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(value);
            Add((ushort)bytes.Length);
            _bytes.AddRange(bytes);
        }

        public int GetInt()
        {
            int value = BitConverter.ToInt32(_readableBuffer, _pos);
            _pos += 4;
            return value;
        }

        public float GetFloat()
        {
            float value = BitConverter.ToSingle(_readableBuffer, _pos);
            _pos += 4;
            return value;
        }

        public bool GetBool()
        {
            bool value = BitConverter.ToBoolean(_readableBuffer, _pos++);
            return value;
        }

        public bool CheckBytes(int count)
        {
            if (_readableBuffer != null)
                return _readableBuffer.Length >= _pos + count;
            return _bytes.Count >= _pos + count;
        }

        public byte GetByte()
        {
            return _readableBuffer[_pos++];
        }

        public ushort GetUshort()
        {
            ushort value = BitConverter.ToUInt16(_readableBuffer, _pos);
            _pos += 2;
            return value;
        }

        public long GetLong()
        {
            long value = BitConverter.ToInt64(_readableBuffer, _pos);
            _pos += 8;
            return value;
        }

        public ulong GetULong()
        {
            ulong value = BitConverter.ToUInt64(_readableBuffer, _pos);
            _pos += 8;
            return value;
        }

        public string GetString()
        {
            int length = GetUshort();
            string value = Encoding.UTF8.GetString(_readableBuffer, _pos, length);
            _pos += length;
            return value;
        }
    }
}
