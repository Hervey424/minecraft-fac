﻿using FuckASAC.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace FuckASAC
{
    /// <summary>
    /// 全局变量
    /// </summary>
    public static class Global
    {
        /// <summary>
        /// 本地端口
        /// </summary>
        public static int LOCAL_PORT { get;  } = 25555;

        /// <summary>
        /// md5列表文件路径
        /// </summary>
        public static string MD5_FILE_PATH { get;  } = "md5list.dat";

        /// <summary>
        /// 写入到服务端的流
        /// </summary>
        public static BinaryWriter ToServerWriter;

        /// <summary>
        /// 写入到客户端的流
        /// </summary>
        public static BinaryWriter ToClientWriter;

        /// <summary>
        /// 是否加密
        /// </summary>
        public static bool IsCompression { get; set; } = false;

        /// <summary>
        /// 加密阈值
        /// </summary>
        public static int CompressionThreshold { get; set; } = 0;

        /// <summary>
        /// 是否为1.12.2版本
        /// </summary>
        public static bool IsVersion1_12_2 { get; set; } = false;
    }
}
