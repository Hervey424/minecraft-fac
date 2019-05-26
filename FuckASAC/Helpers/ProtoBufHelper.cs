using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuckASAC.Helpers
{
    /// <summary>
    /// protobuf常用方法
    /// </summary>
    public static class ProtoBufHelper
    {
        #region read
        /// <summary>
        /// 读取BinaryReader中varint
        /// </summary>
        /// <param name="reader">数据流</param>
        /// <param name="readed">已经读取的数据</param>
        /// <returns></returns>
        public static int ReadVarInt(BinaryReader reader, List<byte> readed = null)
        {
            int numRead = 0;
            int result = 0;
            byte read;
            do
            {
                read = reader.ReadByte();
                if (readed != null)
                {
                    readed.Add(read);
                }

                int value = (read & 0x7f);
                result |= (value << (7 * numRead));

                numRead++;
                if (numRead > 5)
                {
                    return 0;
                }
            } while ((read & 0x80) != 0);

            return result;
        }

        /// <summary>
        /// 读取byte[]中的varint
        /// </summary>
        /// <param name="s"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        private static int ReadVarInt(List<byte> s, out int count)
        {
            int i = 0;
            int j = 0;

            count = 0;
            while (true)
            {
                ++count;
                int k = s.First();
                s.RemoveAt(0);

                i |= (k & 0x7F) << j++ * 7;
                if (j > 5)
                {
                    return 0;
                }
                if ((k & 0x80) != 128) break;
            }
            return i;
        }

        /// <summary>
        /// 读取BinaryReader中的string
        /// </summary>
        /// <param name="reader">数据流</param>
        /// <returns></returns>
        public static string ReadString(BinaryReader reader)
        {
            int length = ReadVarInt(reader);

            var response = reader.ReadBytes(length);
            var result = Encoding.UTF8.GetString(response);

            return result;
        }
        #endregion

        #region Get
        /// <summary>
        /// 获取string
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static byte[] GetString(string content)
        {
            List<byte> output = new List<byte>();

            output.AddRange(GetVarInt(content.Length));
            output.AddRange(Encoding.UTF8.GetBytes(content));

            return output.ToArray();
        }

        /// <summary>
        /// 获取verint
        /// </summary>
        /// <param name="paramInt"></param>
        /// <returns></returns>
        public static byte[] GetVarInt(int paramInt)
        {
            List<byte> output = new List<byte>();
            while (true)
            {
                if ((paramInt & 0xFFFFFF80) == 0)
                {
                    output.Add((byte)paramInt);
                    return output.ToArray();
                }

                output.Add((byte)(paramInt & 0x7F | 0x80));
                paramInt = paramInt >> 7;
            }
        }
        #endregion
    }
}
