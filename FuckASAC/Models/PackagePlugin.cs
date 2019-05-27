using FuckASAC.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace FuckASAC.Models
{
    /// <summary>
    /// 插件消息数据包
    /// </summary>
    public class PackagePlugin : Package
    {
        /// <summary>
        /// 根据基本数据包初始化
        /// </summary>
        /// <param name="package"></param>
        public PackagePlugin(Package package)
        {
            this.PackageLength = package.PackageLength;
            this.PackageId = package.PackageId;
            this.OriginData = package.OriginData;
            this.Data = package.Data;
            using (MemoryStream ms = new MemoryStream(package.Data))
            {
                using (BinaryReader br = new BinaryReader(ms))
                {
                    this.ChannelName = br.ReadVarString();
                    int channelDataLength = 0;
                    if (Global.isCompression)
                    {
                        channelDataLength = Convert.ToInt32(br.BaseStream.Length - br.BaseStream.Position);
                    }
                    else
                    {
                        channelDataLength = br.ReadInt16BE();
                    }
                    this.ChannelData = br.ReadBytes(channelDataLength);
                }
            }
        }

        /// <summary>
        /// 根据具体内容初始化
        /// </summary>
        /// <param name="packageId">packageId</param>
        /// <param name="ChannelName">ChannelName</param>
        /// <param name="ChannelData">ChannelData</param>
        public PackagePlugin(byte packageId , string ChannelName, byte[] ChannelData, bool is112)
        {
            //成员
            this.ChannelName = ChannelName;
            this.ChannelData = ChannelData;

            byte[] channelDataLengthBytes = BitConverter.GetBytes((Int16)ChannelData.Length).Reverse();

            //Data
            List<byte> data = new List<byte>();
            data.AddRange(ProtoBufUtil.GetVarStringBytes(ChannelName));
            if(!is112)
            {
                data.AddRange(channelDataLengthBytes);
            }
            data.AddRange(ChannelData);

            //OriginData
            List<byte> originData = new List<byte>();
            List<byte> packageIdAndData = new List<byte>();
            packageIdAndData.Add(packageId);
            packageIdAndData.AddRange(data);
            if(is112)
            {
                // 如果大于阈值 , 就压缩
                if(packageIdAndData.Count > Global.compressionThreshold)
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

            //父类成员
            this.PackageId = packageId;
            this.Data = data.ToArray();
            this.OriginData = originData.ToArray();
        }

        /// <summary>
        /// 通道名称
        /// </summary>
        public string ChannelName { get; set; }

        /// <summary>
        /// 数据内容
        /// </summary>
        public byte[] ChannelData { get; set; }
    }
}
