using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace FAC.Utils
{
    public static class SerializeUtil
    {
        /// <summary>
        /// 序列化到文件
        /// </summary>
        public static void SerializeToFile<T>(string path , T data)
        {
            IFormatter formatter = new BinaryFormatter();
            using (Stream stream = new FileStream(path, FileMode.Create, FileAccess.Write))
            {
                formatter.Serialize(stream, data);
            }
        }

        /// <summary>
        /// 从文件反序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <returns></returns>
        public static T DeserializeFromFile<T>(string path) where T: class
        {
            IFormatter formatter = new BinaryFormatter();
            T data;
            using (Stream stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite))
            {
                data = formatter.Deserialize(stream) as T;
            }
            return data;
        }
    }
}
