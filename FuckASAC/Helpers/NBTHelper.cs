using FuckASAC.Helper;
using FuckASAC.NBT;
using FuckASAC.NBT.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace FuckASAC.Helpers
{
    public class NBTHelper
    {
        private static byte[] removeLength(byte[] data)
        {
            return data.Where((b, index) => index > 2).ToArray();
        }

        public static string GetSalt(byte[] data)
        {
            MemoryStream ms = new MemoryStream(removeLength(data));
            TagCompound tag = NBTFile.FromStream(ms);
            byte[] saltarr = tag.GetByteArray("salt");
            string salt = System.Text.Encoding.UTF8.GetString(saltarr);
            if(saltarr.Length > 100)
            {
                salt = RSAHelper.GetSalt(saltarr);
            }
            return salt;
        }

        public static byte[] getSaltByteArray(string salt)
        {
            MemoryStream ms = new MemoryStream();
            TagCompound tag = new TagCompound();
            tag.Add("salt", Encoding.UTF8.GetBytes(salt));
            NBTFile.ToStream(ms, tag, true);
            byte[] data = ms.ToArray();

            MemoryStream ms2 = new MemoryStream();
            using (BinaryWriter bw = new BinaryWriter(ms2))
            {
                bw.Write((byte)0);
                bw.WriteUInt16BE((UInt16)data.Length);
                bw.Write(data);
            }
            return ms2.ToArray();
        }

        public static bool GetIsUseRSA(byte[] data)
        {
            bool isRsa = false;
            MemoryStream ms = new MemoryStream(removeLength(data));
            TagCompound tag = NBTFile.FromStream(ms);
            TagList taglist = (TagList)tag["md5s"];
            foreach (TagByteArray br in taglist)
            {
                byte[] bytes = br.Value;
                if (bytes.Length > 100)
                {
                    isRsa = true;
                    break;
                }
            }
            return isRsa;
        }

        public static List<string> GetMd5List(byte[] data)
        {
            List<string> md5s = new List<string>();
            MemoryStream ms = new MemoryStream(removeLength(data));
            TagCompound tag = NBTFile.FromStream(ms);
            TagList taglist = (TagList)tag["md5s"];
            foreach (TagByteArray br in taglist)
            {
                byte[] bytes = br.Value;
                if(bytes.Length > 100)
                {
                    string md5 = RSAHelper.GetMD5(bytes);
                    md5s.Add(md5);
                }
                else
                {
                    md5s.Add(Encoding.UTF8.GetString(bytes));
                }
            }
            return md5s;
        }
    }
}
