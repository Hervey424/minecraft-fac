using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using FAC.Models;
using FAC.Utils;

namespace FAC.Plugins
{
    /// <summary>
    /// AnotherAntiCheat处理
    /// 流程: 
    /// 1. 服务端发送salt给客户端
    /// 2. 程序把salt清空, 然后发给客户端(因为salt为空的时候才能获取到没有hash过的文件列表)
    /// 3. 客户端把没有hash的md5列表发送过来
    /// 4. 程序保存文件, 以后所有请求会发送给客户端本文件
    /// </summary>
    public class ASACPlugin : AbsoluteMessagePlugin
    {
        /// <summary>
        /// 加密md5的salt
        /// </summary>
        string Salt { get; set; } = string.Empty;

        /// <summary>
        /// 保存的文件名
        /// </summary>
        private readonly string fileName = "aac-md5list.dat";

        /// <summary>
        /// channel列表
        /// </summary>
        public override List<string> Channels
        {
            get
            {
                return new List<string> { "anotheranticheat", "anotherstaranticheat", "AnotherStarAntiCheat" };
            }
        }

        /// <summary>
        /// 客户端发送到服务端
        /// 作用: 保存客户端发过来的md5列表
        /// </summary>
        /// </summary>
        /// <param name="package"></param>
        /// <param name="write"></param>
        /// <returns></returns>
        public override bool CTSPluginMessageHandle(PackagePlugin package, BinaryWriter toClientWriter, BinaryWriter toServerWriter)
        {
            List<string> md5list = ASACUtil.GetMd5ListFromNBTByteArray(package.ChannelData, Global.IsVersion1_12_2);
            SerializeUtil.SerializeToFile(fileName, md5list);

            Console.WriteLine($"[AnotherAntiCheat]本地文件{fileName}不存在, 创建文件成功");
            // 继续发送原数据包, 然后会被T出服务器
            return false;
        }

        /// <summary>
        /// 服务端发送给客户端
        /// </summary>
        /// <param name="package"></param>
        /// <param name="write"></param>
        /// <returns></returns>
        public override bool STCPluginMessageHandle(PackagePlugin package, BinaryWriter toClientWriter, BinaryWriter toServerWriter)
        {
            // 如果存在md5文件, 就把请求拦截下, 并发送假的过去
            if (File.Exists(fileName))
            {
                // 获取到服务端发送来的salt
                Salt = ASACUtil.GetSaltFromNBTByteArray(package.ChannelData, Global.IsVersion1_12_2);
                // 把列表发送过去
                SendMd5List(package.ChannelName, toServerWriter);

                Console.WriteLine($"[AnotherAntiCheat]收到服务端检测MD5请求, Salt={Salt}, 返回{fileName}中的列表");

                return true;
            }
            else
            {
                byte[] channelData = ASACUtil.GetSaltNBTDataArrayFromSaltString(string.Empty, Global.IsVersion1_12_2);
                PackagePlugin newPackage = new PackagePlugin(STCPluginPackageId, package.ChannelName, channelData, Global.IsVersion1_12_2);
                toClientWriter.Write(newPackage.OriginData);

                Console.WriteLine($"[AnotherAntiCheat]收到服务端检测MD5请求, 本地文件不存在, 获取MD5列表");

                return true;
            }
        }

        /// <summary>
        /// 发送假数据给服务端
        /// </summary>
        /// <param name="channelName"></param>
        private void SendMd5List(string channelName, BinaryWriter toServerWriter)
        {
            List<string> localMd5List = SerializeUtil.DeserializeFromFile<List<string>>(fileName);
            byte[] md5sNBTByteArray = ASACUtil.GetMd5NBTByteArray(false, localMd5List, Salt, Global.IsVersion1_12_2);

            MemoryStream channelDataStream = new MemoryStream();
            using (BinaryWriter bw = new BinaryWriter(channelDataStream))
            {
                bw.Write((byte)1);
                //如果是1.12, 这里不需要写入长度
                if (!Global.IsVersion1_12_2)
                {
                    bw.WriteUInt16BE((UInt16)md5sNBTByteArray.Length);
                }
                bw.Write(md5sNBTByteArray);
            }
            byte[] channelData = channelDataStream.ToArray();
            PackagePlugin newPackage = new PackagePlugin(CTSPluginPackageId, channelName, channelData, Global.IsVersion1_12_2);
            toServerWriter.Write(newPackage.OriginData);
        }
    }
}
