using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using FuckASAC.Models;
using FuckASAC.Utils;

namespace FuckASAC.Plugins
{
    /// <summary>
    /// AnotherAntiCheat 处理
    /// </summary>
    public class ASACPlugin : IPlugin
    {
        public bool CTSHandle(Package package, BinaryWriter write)
        {
            byte pluginPackageId = (byte)(Global.IsVersion1_12_2 ? 0x09 : 0x17);
            if (package.PackageId == pluginPackageId)
            {
                PackagePlugin plugin = new PackagePlugin(package);

                // 读取modlist
                if (plugin.ChannelName == "FML|HS" && plugin.ChannelData.Length > 0 && plugin.ChannelData[0] == 2)
                {
                    if (Global.HasMd5File)
                    {
                        write.Write(Global.ServerInfo.ModList.OriginData);
                        return false;
                    }
                    else
                    {
                        Global.ServerInfo.ModList = package;
                    }
                }

                // 读取md5list
                if (Global.ASAC_CHANNEL_NAMES.Contains(plugin.ChannelName))
                {
                    Global.ServerInfo.Md5List = ASACUtil.GetMd5ListFromNBTByteArray(plugin.ChannelData, Global.IsVersion1_12_2);
                    SerializeUtil.SerializeToFile(Global.MD5_FILE_PATH, Global.ServerInfo);
                    Global.HasMd5File = true;
                }
            }
            return false;
        }

        public bool STCHandle(Package package, BinaryWriter write)
        {
            // 插件消息
            byte pluginPackageId = (byte)(Global.IsVersion1_12_2 ? 0x18 : 0x3f);
            if (package.PackageId == pluginPackageId)
            {
                PackagePlugin plugin = new PackagePlugin(package);
                if (Global.ASAC_CHANNEL_NAMES.Contains(plugin.ChannelName))
                {
                    if (Global.HasMd5File)
                    {
                        Global.Salt = ASACUtil.GetSaltFromNBTByteArray(plugin.ChannelData, Global.IsVersion1_12_2);
                        SendMd5List(plugin.ChannelName);
                        return true;
                    }
                    else
                    {
                        byte[] channelData = ASACUtil.GetSaltNBTDataArrayFromSaltString(string.Empty, Global.IsVersion1_12_2);
                        PackagePlugin newPackage = new PackagePlugin(pluginPackageId, plugin.ChannelName, channelData, Global.IsVersion1_12_2);
                        write.Write(newPackage.OriginData);
                        return true;
                    }
                }
            }
            return false;
        }

        private void SendMd5List(string channelName)
        {
            byte pluginPackageId = (byte)(Global.IsVersion1_12_2 ? 0x09 : 0x17);
            byte[] md5sNBTByteArray = ASACUtil.GetMd5NBTByteArray(false, Global.ServerInfo.Md5List, Global.Salt, Global.IsVersion1_12_2);

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
            PackagePlugin newPackage = new PackagePlugin(pluginPackageId, channelName, channelData, Global.IsVersion1_12_2);
            Global.ToServerWriter.Write(newPackage.OriginData);
        }
    }
}
