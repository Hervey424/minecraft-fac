using System;
using System.Text;
using System.IO;

namespace FuckASAC.NBT.Helper
{
    public static class StreamHelper
    {

        public static int ReadInt32Helper(this FileStream br)
        {
            return BitConverter.ToInt32(ReadBytes(br, 4), 0);
        }

        public static short ReadInt16Helper(this FileStream br)
        {
            return BitConverter.ToInt16(ReadBytes(br, 2), 0);
        }

        public static long ReadInt64Helper(this FileStream br)
        {
            return BitConverter.ToInt64(ReadBytes(br, 8), 0);
        }

        public static double ReadDoubleHelper(this FileStream br)
        {
            return BitConverter.ToDouble(ReadBytes(br, 8), 0);
        }

        public static float ReadSingleHelper(this FileStream br)
        {
            return BitConverter.ToSingle(ReadBytes(br, 4), 0);
        }

        public static byte ReadByteHelper(this FileStream br)
        {
            return (byte)br.ReadByte();
        }

        public static string ReadStringHelper(this FileStream br)
        {
            short length = br.ReadInt16Helper();
            return Encoding.UTF8.GetString(ReadBytes(br, length, false));
        }


        private static byte[] ReadBytes(FileStream br, int length, bool reverse = true)
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


        private static byte[] ReadBytes(BinaryReader br, int length, bool reverse = true)
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

        public static void WriteHelper(this FileStream bw, byte value)
        {
            bw.WriteByte(value);
        }

        public static void WriteHelper(this FileStream bw, int value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            Array.Reverse(bytes);
            bw.Write(bytes, 0, (int)bytes.Length);
        }

        public static void WriteHelper(this FileStream bw, long value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            Array.Reverse(bytes);
            bw.Write(bytes, 0, (int)bytes.Length);
        }

    }
}
