using FuckASAC.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace FuckASAC.Utils
{
    public class ClientChatUtil
    {
        public static void SendMessage(BinaryWriter toClientWriter, string message)
        {
            var text = new
            {
                text = "[反作弊破解]",
                bold = true,
                color = "green",
                extra = new List<object>
                {
                    new { text = message, bold = true, color = "green" }
                }
            };
            string json = JsonConvert.SerializeObject(text);
            Package package = Package.Create((byte)0x02, ProtoBufUtil.GetVarStringBytes(json), Global.IsVersion1_12_2);
            toClientWriter.Write(package.OriginData);
        }
    }
}
