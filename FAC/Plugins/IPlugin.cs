using FAC.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace FAC.Plugins
{
    public interface IPlugin
    {
        /// <summary>
        /// channel列表
        /// </summary>
        List<string> Channels { get; }

        /// <summary>
        /// 客户端到服务端的数据包
        /// </summary>
        /// <param name="package">数据包</param>
        /// <param name="write">写入流</param>
        /// <returns>是否已经处理</returns>
        bool CTSHandle(Package package, BinaryWriter toClientWriter, BinaryWriter toServerWriter);

        /// <summary>
        /// 服务端到客户端的数据包
        /// </summary>
        /// <param name="package">数据包</param>
        /// <param name="write">写入流</param>
        /// <returns>是否已经处理</returns>
        bool STCHandle(Package package, BinaryWriter toClientWriter, BinaryWriter toServerWriter);
    }
}
