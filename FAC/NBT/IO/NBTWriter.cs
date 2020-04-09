using System;
using System.Text;
using System.IO;

namespace FAC.NBT.IO
{
    public class NBTBinaryWriter : IDisposable
    {
        private Stream _baseStream;

        public NBTBinaryWriter(Stream baseStream)
        {
            _baseStream = baseStream;
        }

        public void Write(byte value)
        {
            _baseStream.Write(new byte[] { value }, 0, 1);
        }

        public void Write(byte[] value)
        {
            _baseStream.Write(value, 0, value.Length);
        }

        public void Write(int value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            Array.Reverse(bytes);
            _baseStream.Write(bytes, 0, bytes.Length);
        }

        public void Write(long value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            Array.Reverse(bytes);
            _baseStream.Write(bytes, 0, bytes.Length);
        }

        public void Write(short value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            Array.Reverse(bytes);
            _baseStream.Write(bytes, 0, bytes.Length);
        }

        public void Write(float value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            Array.Reverse(bytes);
            _baseStream.Write(bytes, 0, bytes.Length);
        }

        public void Write(double value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            Array.Reverse(bytes);
            _baseStream.Write(bytes, 0, bytes.Length);
        }

        public void Write(string value)
        {
            if (!(Encoding.UTF8.GetByteCount(value) > 32767))
            {
                byte[] bytes = Encoding.UTF8.GetBytes(value);
                Write((short)bytes.Length);
                _baseStream.Write(bytes, 0, bytes.Length);
            }
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
