using System;
using System.Text;
using System.IO;
namespace FAC.NBT.IO
{
    public class NBTBinaryReader : IDisposable
    {
        private Stream _baseStream;

        public Stream BaseStream => _baseStream;

        public NBTBinaryReader(Stream baseStream)
        {
            _baseStream = baseStream;
        }

        public int ReadInt32()
        {
            return BitConverter.ToInt32(ReadBytes(_baseStream, 4), 0);
        }

        public short ReadInt16()
        {
            return BitConverter.ToInt16(ReadBytes(_baseStream, 2), 0);
        }

        public long ReadInt64()
        {
            return BitConverter.ToInt64(ReadBytes(_baseStream, 8), 0);
        }

        public double ReadDouble()
        {
            return BitConverter.ToDouble(ReadBytes(_baseStream, 8), 0);
        }

        public float ReadSingle()
        {
            return BitConverter.ToSingle(ReadBytes(_baseStream, 4), 0);
        }

        public byte ReadByte()
        {
            byte[] r = ReadBytes(_baseStream, 1);
            return r[0];
        }

        public string ReadString()
        {
            short length = ReadInt16();
            return Encoding.UTF8.GetString(ReadBytes(_baseStream, length, false), 0, length);
        }

        public byte[] ReadBytes(int length, bool reverse = true)
        {
            return ReadBytes(_baseStream, length, reverse);
        }

        private byte[] ReadBytes(Stream br, int length, bool reverse = true)
        {
            byte[] buf = new byte[length];
            if (br.Read(buf, 0, buf.Length) != buf.Length)
            {
                throw new EndOfStreamException();
            }
            if (reverse)
            {
                Array.Reverse(buf);
            }
            return buf;
        }

        public void Dispose()
        {
            if (_baseStream == null)
                return;
            _baseStream.Dispose();
            _baseStream = null;
        }
    }
}
