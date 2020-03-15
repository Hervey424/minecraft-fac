using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FuckASAC.Plugins.CatAntiCheat
{
    [Serializable]
    public class CatAntiCheatData
    {
        /// <summary>
        /// 版本
        /// </summary>
        public int Version { get; set; }

        /// <summary>
        /// hash列表
        /// </summary>
        public byte[] FileHashBytes { get; set; }
    }
}
