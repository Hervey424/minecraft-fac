using FAC.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace FAC.Plugins
{
    /// <summary>
    /// 处理pluginmessage
    /// 自动根据channel匹配
    /// </summary>
    public abstract class MessagePlugin : IPlugin
    {
        /// <summary>
        /// 客户端->服务端 plugin的PackageId
        /// </summary>
        public byte CTSPluginPackageId => (byte)(Global.IsVersion1_12_2 ? 0x09 : 0x17);

        /// <summary>
        /// 服务端->客户端 plugin的PackageId
        /// </summary>
        public byte STCPluginPackageId => (byte)(Global.IsVersion1_12_2 ? 0x18 : 0x3f);

        /// <summary>
        /// 要处理的channel列表
        /// </summary>
        public abstract List<string> Channels { get; }

        /// <summary>
        /// 客户端发送给服务端的消息处理
        /// </summary>
        /// <param name="package"></param>
        /// <param name="write"></param>
        /// <returns>true表示这条消息已经处理, 不要在发送给服务端了</returns>
        public virtual bool CTSHandle(Package package, BinaryWriter toClientWriter, BinaryWriter toServerWriter)
        {
            if(package.PackageId == CTSPluginPackageId)
            {
                PackagePlugin plugin = new PackagePlugin(package);
                if(Channels.Contains(plugin.ChannelName, StringComparer.CurrentCultureIgnoreCase))
                {
                    return CTSPluginMessageHandle(plugin, toClientWriter, toServerWriter);
                }
            }
            return false;
        }

        /// <summary>
        /// 服务端发送给客户端的消息处理
        /// </summary>
        /// <param name="package"></param>
        /// <param name="write"></param>
        /// <returns>true表示这条消息已经处理, 不要在发送给客户端了</returns>
        public virtual bool STCHandle(Package package, BinaryWriter toClientWriter, BinaryWriter toServerWriter)
        {
            if (package.PackageId == STCPluginPackageId)
            {
                PackagePlugin plugin = new PackagePlugin(package);
                if (Channels.Contains(plugin.ChannelName, StringComparer.CurrentCultureIgnoreCase))
                {
                    return STCPluginMessageHandle(plugin, toClientWriter, toServerWriter);
                }
            }
            return false;
        }

        /// <summary>
        /// 客户端发送给服务端的pluginmessage
        /// </summary>
        /// <param name="package"></param>
        /// <param name="write"></param>
        /// <returns></returns>
        public abstract bool CTSPluginMessageHandle(PackagePlugin package, BinaryWriter toClientWriter, BinaryWriter toServerWriter);

        /// <summary>
        /// 服务端发送给客户端的pluginmessage
        /// </summary>
        /// <param name="package"></param>
        /// <param name="write"></param>
        /// <returns></returns>
        public abstract bool STCPluginMessageHandle(PackagePlugin package, BinaryWriter toClientWriter, BinaryWriter toServerWriter);
    }
}
