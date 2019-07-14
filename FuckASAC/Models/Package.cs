using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FuckASAC.Models
{
    /// <summary>
    /// 协议数据包
    /// </summary>
    [Serializable]
    public class Package
    {
        /// <summary>
        /// 数据包总长度
        /// </summary>
        public int PackageLength { get; set; } = 0;

        /// <summary>
        /// 协议ID
        /// </summary>
        public byte PackageId { get; set; } = 0;

        /// <summary>
        /// 数据包原始的数据(datalength + packageid + data)
        /// </summary>
        public byte[] OriginData { get; set; }   

        /// <summary>
        /// 真正数据的部分
        /// </summary>
        public byte[] Data { get; set; }
    }
}
