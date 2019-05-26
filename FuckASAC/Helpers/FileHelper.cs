using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace FuckASAC.Helpers
{
    public class FileHelper
    {
        public static void SaveMd5List(string file, List<string> md5s)
        {
            IFormatter formatter = new BinaryFormatter();
            using (Stream stream = new FileStream(file, FileMode.Create, FileAccess.Write))
            {
                formatter.Serialize(stream, md5s);
            }
        }

        public static List<string> ReadMd5List(string file)
        {
            IFormatter formatter = new BinaryFormatter();
            List<string> data = new List<string>();
            using (Stream stream = new FileStream(file, FileMode.Open, FileAccess.ReadWrite))
            {
                data = formatter.Deserialize(stream) as List<string>;
            }
            return data;
        }
    }
}
