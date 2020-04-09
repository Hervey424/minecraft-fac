using FAC.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FAC.Utils
{
    public static class BinaryReaderExtension
    {
        #region 读取big-endian数据

        public static UInt16 ReadUInt16BE(this BinaryReader binRdr)
        {
            return BitConverter.ToUInt16(binRdr.ReadBytesRequired(sizeof(UInt16)).Reverse(), 0);
        }

        public static Int16 ReadInt16BE(this BinaryReader binRdr)
        {
            return BitConverter.ToInt16(binRdr.ReadBytesRequired(sizeof(Int16)).Reverse(), 0);
        }

        public static UInt32 ReadUInt32BE(this BinaryReader binRdr)
        {
            return BitConverter.ToUInt32(binRdr.ReadBytesRequired(sizeof(UInt32)).Reverse(), 0);
        }

        public static Int32 ReadInt32BE(this BinaryReader binRdr)
        {
            return BitConverter.ToInt32(binRdr.ReadBytesRequired(sizeof(Int32)).Reverse(), 0);
        }

        public static Int64 ReadInt64BE(this BinaryReader binRdr)
        {
            return BitConverter.ToInt64(binRdr.ReadBytesRequired(sizeof(Int64)).Reverse(), 0);
        }

        public static float ReadSingleBE(this BinaryReader binRdr)
        {
            return BitConverter.ToSingle(binRdr.ReadBytesRequired(sizeof(float)).Reverse(), 0);
        }

        public static byte[] ReadBytesRequired(this BinaryReader binRdr, int byteCount)
        {
            var result = binRdr.ReadBytes(byteCount);

            if (result.Length != byteCount)
                throw new EndOfStreamException(string.Format("{0} bytes required from stream, but only {1} returned.", byteCount, result.Length));
            return result;
        }

        #endregion

        /// <summary>
        /// 读取一个package
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public static Package ReadPackage(this BinaryReader reader)
        {
            List<byte> packageLengthBytes = new List<byte>();
            int packageLength = reader.ReadVarInt(packageLengthBytes);
            //byte[] packageLengthBytes = ProtoBufUtil.GetVarIntBytes(packageLength);
            byte[] packageData = reader.ReadBytes(packageLength);

            List<byte> originData = new List<byte>();
            originData.AddRange(packageLengthBytes);
            originData.AddRange(packageData);

            //如果压缩了, 就获取压缩后的数据
            if (Global.IsCompression)
            {
                using (MemoryStream ms = new MemoryStream(packageData))
                {
                    using (BinaryReader br = new BinaryReader(ms))
                    {
                        int uncompressionDataLength = br.ReadVarInt();
                        packageData = br.ReadBytes(Convert.ToInt32(br.BaseStream.Length - br.BaseStream.Position));
                        if (uncompressionDataLength != 0)
                        {
                            packageData = ZLibUtil.Decompress(packageData);
                        }
                    }
                }
            }

            List<byte> packageIdBytes = new List<byte>();
            int packageId = ProtoBufUtil.GetVarIntFromBytes(packageData, packageIdBytes);

            return new Package
            {
                PackageLength = packageLength,
                PackageId = (byte)packageId,
                OriginData = originData.ToArray(),
                Data = packageData.Where((x, i) => i >= packageIdBytes.Count).ToArray()
            };
        }
    }
}
