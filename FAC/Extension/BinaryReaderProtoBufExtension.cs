using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace FAC.Utils
{
    /// <summary>
    /// 读取protobuf数据
    /// </summary>
    public static class BinaryReaderProtoBufExtension
    {
        /// <summary>
        /// 读取BinaryReader中varint
        /// </summary>
        /// <param name="reader">数据流</param>
        /// <returns></returns>
        public static int ReadVarInt(this BinaryReader reader)
        {
            int numRead = 0;
            int result = 0;
            byte read;
            do
            {
                read = reader.ReadByte();
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
        /// 读取BinaryReader中varint
        /// </summary>
        /// <param name="reader">数据流</param>
        /// <param name="readed">已经读取的数据</param>
        /// <returns></returns>
        public static int ReadVarInt(this BinaryReader reader,List<byte> readed = null)
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
        /// 读取BinaryReader中的thisstring
        /// </summary>
        /// <param name="reader">数据流</param>
        /// <returns></returns>
        public static string ReadVarString(this BinaryReader reader)
        {
            int length = ReadVarInt(reader);
            var response = reader.ReadBytes(length);
            var result = Encoding.UTF8.GetString(response);
            return result;
        }
    }
}
