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
            byte pluginPackageId = (byte)(Global.isVersion1_12_2 ? 0x09 : 0x17);
            if (package.PackageId == pluginPackageId)
            {
                PackagePlugin plugin = new PackagePlugin(package);

                if (Global.hasMd5File)
                {
                    if (Global.ASAC_CHANNEL_NAMES.Contains(plugin.ChannelName))
                    {
                        bool isUseRSA = ASACUtil.IsNeedRSA(plugin.ChannelData, Global.isVersion1_12_2);
                        byte[] md5sNBTByteArray = ASACUtil.GetMd5NBTByteArray(isUseRSA, Global.ServerInfo.Md5List, Global.salt, Global.isVersion1_12_2);

                        MemoryStream channelDataStream = new MemoryStream();
                        using (BinaryWriter bw = new BinaryWriter(channelDataStream))
                        {
                            bw.Write((byte)plugin.ChannelData[0]);
                            // 如果是1.12, 这里不需要写入长度
                            if (!Global.isVersion1_12_2)
                            {
                                bw.WriteUInt16BE((UInt16)md5sNBTByteArray.Length);
                            }
                            bw.Write(md5sNBTByteArray);
                        }
                        byte[] channelData = channelDataStream.ToArray();
                        PackagePlugin newPackage = new PackagePlugin(pluginPackageId, plugin.ChannelName, channelData, Global.isVersion1_12_2);
                        write.Write(newPackage.OriginData);
                        return true;
                    }
                }
                else
                {
                    if (Global.ASAC_CHANNEL_NAMES.Contains(plugin.ChannelName))
                    {
                        // 读取modlist
                        //if (plugin.ChannelName == "FML|HS" && plugin.ChannelData.Length > 0 && plugin.ChannelData[0] == 2)
                        //{
                        //    Global.ServerInfo.ModList = plugin;
                        //}

                        // 读取md5list
                        Global.ServerInfo.Md5List = ASACUtil.GetMd5ListFromNBTByteArray(plugin.ChannelData, Global.isVersion1_12_2);
                        SerializeUtil.SerializeToFile("md5list.dat", Global.ServerInfo);
                        Global.hasMd5File = true;
                    }
                }
            }
            return false;
        }

        public bool STCHandle(Package package, BinaryWriter write)
        {
            // 插件消息
            byte pluginPackageId = (byte)(Global.isVersion1_12_2 ? 0x18 : 0x3f);
            if (package.PackageId == pluginPackageId)
            {
                PackagePlugin plugin = new PackagePlugin(package);
                if (Global.ASAC_CHANNEL_NAMES.Contains(plugin.ChannelName))
                {
                    if (Global.hasMd5File)
                    {
                        Global.salt = ASACUtil.GetSaltFromNBTByteArray(plugin.ChannelData, Global.isVersion1_12_2);
                    }
                    else
                    {
                        byte[] channelData = ASACUtil.GetSaltNBTDataArrayFromSaltString(string.Empty, Global.isVersion1_12_2);
                        PackagePlugin newPackage = new PackagePlugin(pluginPackageId, plugin.ChannelName, channelData, Global.isVersion1_12_2);
                        write.Write(newPackage.OriginData);
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
