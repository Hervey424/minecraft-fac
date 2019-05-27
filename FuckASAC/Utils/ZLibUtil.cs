using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using zlib;

namespace FuckASAC.Utils
{
    public class ZLibUtil
    {
        private static void CopyStream(Stream input, Stream output, byte[] data)
        {
            byte[] buffer = new byte[2000];
            int len;
            while ((len = input.Read(buffer, 0, 2000)) > 0)
            {
                output.Write(buffer, 0, len);
            }
            output.Flush();
        }

        public static byte[] Decompress(byte[] data)
        {
            MemoryStream ms = new MemoryStream();
            ZOutputStream outZStream = new ZOutputStream(ms);
            MemoryStream inStream = new MemoryStream(data);

            try
            {
                CopyStream(inStream, outZStream, data);
            }
            finally
            {
                outZStream.Close();
                ms.Close();
                inStream.Close();
            }

            return ms.ToArray();
        }

        /// <summary>  
        /// zlib.net 压缩函数  
        /// </summary>  
        /// <param name="strSource">待压缩数据</param>  
        /// <returns>压缩后数据</returns>  
        public static byte[] Compress(byte[] bytes)
        {
            MemoryStream outms = new MemoryStream();
            MemoryStream inms = new MemoryStream(bytes);
            zlib.ZOutputStream outZStream = new zlib.ZOutputStream(outms, zlib.zlibConst.Z_DEFAULT_COMPRESSION);
            try
            {
                CopyStream(inms, outZStream, bytes);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                outZStream.Close();
            }
            return outms.ToArray();
        }
    }
}
