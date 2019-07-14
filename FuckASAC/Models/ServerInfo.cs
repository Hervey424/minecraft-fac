using FuckASAC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System
{
    [Serializable]
    public class ServerInfo
    {
        /// <summary>
        /// md5列表
        /// </summary>
        public List<string> Md5List { get; set; } = new List<string>();

        /// <summary>
        /// modlist数据包内容
        /// </summary>
        public Package ModList { get; set; }
    }
}
