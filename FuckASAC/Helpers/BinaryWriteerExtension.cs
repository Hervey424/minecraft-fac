using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuckASAC.Helpers
{
    public static class BinaryWriteerExtension
    {

        public static void WriteUInt16BE(this BinaryWriter writer, UInt16 data)
        {
            byte[] bytes = BitConverter.GetBytes(data).Reverse();
            writer.Write(bytes);
        }

        public static void WritePluginData(this BinaryWriter write,int packageid, string channelName, byte[] bytes)
        {
            using (MemoryStream nms = new MemoryStream())
            {
                using (BinaryWriter nsw = new BinaryWriter(nms))
                {
                    byte[] shortbytes = BitConverter.GetBytes((Int16)bytes.Length);
                    shortbytes.Reverse();

                    nsw.Write((byte)packageid);
                    nsw.Write(ProtoBufHelper.GetString(channelName));
                    nsw.Write(shortbytes);
                    nsw.Write(bytes);
                    nsw.Flush();

                    byte[] buff = nms.ToArray();
                    write.Write(ProtoBufHelper.GetVarInt(buff.Length));
                    write.Write(buff);
                    write.Flush();
                }
            }
        }
    }
}
