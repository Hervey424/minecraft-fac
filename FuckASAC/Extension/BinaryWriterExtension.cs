using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuckASAC.Utils
{
    /// <summary>
    /// 写入big-endian
    /// </summary>
    public static class BinaryWriterExtension
    {
        public static void WriteUInt16BE(this BinaryWriter writer, UInt16 data)
        {
            byte[] bytes = BitConverter.GetBytes(data).Reverse();
            writer.Write(bytes);
        }
    }
}
