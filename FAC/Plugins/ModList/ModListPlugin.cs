using FAC.Models;
using FAC.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace FAC.Plugins
{
    /// <summary>
    /// 保存玩家正常的modlist用来躲避检测
    /// </summary>
    public class ModListPlugin : AbsolutePlugin
    {
        /// <summary>
        /// 保存的文件名
        /// </summary>
        private readonly string fileName = "modlist.dat";

        /// <summary>
        /// 需要匹配的Channel列表
        /// </summary>
        public override List<string> Channels
        {
            get
            {
                return new List<string> { "FML|HS" };
            }
        }

        /// <summary>
        /// 如果本地存在modlist文件, 就发送本地文件的, 如果不存在就创建文件
        /// </summary>
        /// <param name="package"></param>
        /// <param name="write"></param>
        /// <returns></returns>
        public override bool CTSPluginMessageHandle(PackagePlugin package, BinaryWriter toClientWriter, BinaryWriter toServerWriter)
        {
            // 确定是modlist
            if(package.ChannelData.Length > 0 && package.ChannelData[0] == 2)
            {
                // 如果不存在, 就创建并保存文件
                if (!File.Exists(fileName))
                {
                    Console.WriteLine($"[公共][{fileName}]不存在, 创建文件成功!");
                    SerializeUtil.SerializeToFile(fileName, package);
                }

                // 读取文件内容并发送给服务端
                PackagePlugin local = SerializeUtil.DeserializeFromFile<PackagePlugin>(fileName);
                toServerWriter.Write(local.OriginData);

                // 丢掉客户端发来的数据
                return true;
            }
            return false;
        }
    }
}
