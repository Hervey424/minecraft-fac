using FuckASAC.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuckASAC.Utils
{
    /// <summary>
    /// protobuf常用方法
    /// </summary>
    public static class ProtoBufUtil
    {
        /// <summary>
        /// 获取varstring的字节数组
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static byte[] GetVarStringBytes(string content)
        {
            List<byte> output = new List<byte>();

            output.AddRange(GetVarIntBytes(content.Length));
            output.AddRange(Encoding.UTF8.GetBytes(content));

            return output.ToArray();
        }

        /// <summary>
        /// 获取varint的字节数组
        /// </summary>
        /// <param name="paramInt"></param>
        /// <returns></returns>
        public static byte[] GetVarIntBytes(int paramInt)
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


        /// <summary>
        /// 从字节数组中获取varint
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static int GetVarIntFromBytes(byte[] data)
        {
            int numRead = 0;
            int result = 0;
            byte read;
            int index = 0;
            do
            {
                read = data[index++];
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

        public static int GetVarIntFromBytes(byte[] data, List<byte> readed)
        {
            List<byte> s = data.ToList();
            int i = 0;
            int j = 0;

            while (true)
            {
                int k = s.First();
                readed.Add((byte)k);
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
    }
}
