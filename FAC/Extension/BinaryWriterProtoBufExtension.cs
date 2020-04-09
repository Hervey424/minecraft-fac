using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FAC.Utils
{
    /// <summary>
    /// 写入protobuf数据
    /// </summary>
    public static class BinaryWriterProtoBufExtension
    {
        /// <summary>
        /// 写入varstring的数据
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static void GetString(this BinaryWriter writer, string content)
        {
            byte[] output = ProtoBufUtil.GetVarStringBytes(content);
            writer.Write(output);
        }

        /// <summary>
        /// 写入varint的数据
        /// </summary>
        /// <param name="paramInt"></param>
        /// <returns></returns>
        public static void GetVarInt(this BinaryWriter writer, int paramInt)
        {
            byte[] output = ProtoBufUtil.GetVarIntBytes(paramInt);
            writer.Write(output);
        }
    }
}
