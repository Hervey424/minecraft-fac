using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FAC.Plugins.CatAntiCheat
{
    /// <summary>
    /// 消息类别
    /// </summary>
    public enum CatAntiCheatMessageType
    {
        /// <summary>
        /// 服务端发送的hello包, 包含salt
        /// </summary>
        STCHello = 0,

        /// <summary>
        /// 服务端发送的FileCheck, 无内容
        /// </summary>
        STCFileCheck = 1,

        /// <summary>
        /// 服务端发送的classcheck, 不允许的class
        /// </summary>
        STCClassCheck = 2,

        /// <summary>
        /// 服务端发送截图命令
        /// </summary>
        STCScreenShot = 3,

        /// <summary>
        /// 客户端发送的helloreplay, 包含salt和version
        /// </summary>
        CTSHelloReply = 4,

        /// <summary>
        /// 客户端发送的filehash, 包含salt, hashlist, sign
        /// </summary>
        CTSFileHash = 5,

        /// <summary>
        /// 客户端发送类被检测到
        /// </summary>
        CTSClassFound = 6,

        /// <summary>
        /// 发送注入的类, 一般不用发送
        /// </summary>
        CTSInjectDetect = 7,

        /// <summary>
        /// 发送的图片数据
        /// </summary>
        CTSImageData = 8,

        /// <summary>
        /// 服务端发送的DataCheck包, 无内容
        /// </summary>
        STCDataCheck = 9,

        /// <summary>
        /// 客户端发送给服务端的datacheck响应包, 包含是否夜视和透视
        /// </summary>
        CTSDataCheckReply = 10
    }
}
