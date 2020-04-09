using FAC.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FAC.Models
{
    /// <summary>
    /// 协议数据包
    /// </summary>
    [Serializable]
    public class Package
    {
        public static Package Create(byte packageId, byte[] data, bool is112)
        {
            Package package = new Package();
            package.PackageId = packageId;
            package.Data = data;

            List<byte> packageIdAndData = new List<byte>();
            List<byte> originData = new List<byte>();
            packageIdAndData.Add(packageId);
            packageIdAndData.AddRange(data);
            if (is112)
            {
                // 如果大于阈值 , 就压缩
                if(packageIdAndData.Count > Global.CompressionThreshold)
                {
                    byte[] compressData = ZLibUtil.Compress(packageIdAndData.ToArray());
                    byte[] compressDataLengthBytes = ProtoBufUtil.GetVarIntBytes(packageIdAndData.Count);
                    byte[] lengthBytes = ProtoBufUtil.GetVarIntBytes(compressDataLengthBytes.Length + compressData.Length);

                    originData.AddRange(lengthBytes);
                    originData.AddRange(compressDataLengthBytes);
                    originData.AddRange(compressData);
                }
                else
                {
                    byte[] lengthBytes = ProtoBufUtil.GetVarIntBytes(packageIdAndData.Count+1);
                    originData.AddRange(lengthBytes);
                    originData.Add(0);
                    originData.AddRange(packageIdAndData);
                }
            }
            else
            {
                byte[] lengthBytes = ProtoBufUtil.GetVarIntBytes(packageIdAndData.Count);
                originData.AddRange(lengthBytes);
                originData.AddRange(packageIdAndData);
            }
            package.OriginData = originData.ToArray();
            return package;
        }

        /// <summary>
        /// 数据包总长度
        /// </summary>
        public int PackageLength { get; set; } = 0;

        /// <summary>
        /// 协议ID
        /// </summary>
        public byte PackageId { get; set; } = 0;

        /// <summary>
        /// 数据包原始的数据(datalength + packageid + data)
        /// </summary>
        public byte[] OriginData { get; set; }   

        /// <summary>
        /// 真正数据的部分
        /// </summary>
        public byte[] Data { get; set; }
    }
}
