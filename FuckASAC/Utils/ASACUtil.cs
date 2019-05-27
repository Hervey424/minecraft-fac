using FuckASAC.NBT;
using FuckASAC.NBT.IO;
using Org.BouncyCastle.Crypto.Encodings;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace FuckASAC.Utils
{
    public static class ASACUtil
    {
        private static byte[] removeLength(byte[] data, bool is112)
        {
            int offset = is112 ? 0 : 2;
            return data.Where((b, index) => index > offset).ToArray();
        }

        /// <summary>
        /// 判断是否需要rsa加密
        /// 只有1.0.6及以下需要
        /// </summary>
        /// <param name="md5Data"></param>
        /// <returns></returns>
        public static bool IsNeedRSA(byte[] md5Data ,bool is112)
        {
            bool isRsa = false;
            MemoryStream ms = new MemoryStream(removeLength(md5Data, is112));
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

        /// <summary>
        /// 从nbt数据中获取salt
        /// </summary>
        /// <param name="data"></param>
        /// <param name="is112">是否是1.12版本, 如果是, 那么只需要处理一位数字</param>
        /// <returns></returns>
        public static string GetSaltFromNBTByteArray(byte[] data, bool is112)
        {
            MemoryStream ms = new MemoryStream(removeLength(data, is112));
            TagCompound tag = NBTFile.FromStream(ms);
            byte[] saltarr = tag.GetByteArray("salt");
            string salt = System.Text.Encoding.UTF8.GetString(saltarr);
            if (saltarr.Length > 100)
            {
                salt = ASACUtil.RSADecodeSalt(saltarr);
            }
            return salt;
        }

        /// <summary>
        /// 根绝salt生成nbt字节数组
        /// </summary>
        /// <param name="salt"></param>
        /// <returns></returns>
        public static byte[] GetSaltNBTDataArrayFromSaltString(string salt, bool is112)
        {
            MemoryStream ms = new MemoryStream();
            TagCompound tag = new TagCompound();
            tag.Add("salt", Encoding.UTF8.GetBytes(salt));
            NBTFile.ToStream(ms, tag, !is112);
            byte[] data = ms.ToArray();

            MemoryStream ms2 = new MemoryStream();
            using (BinaryWriter bw = new BinaryWriter(ms2))
            {
                bw.Write((byte)0);
                if(!is112)
                {
                    bw.WriteUInt16BE((UInt16)data.Length);
                }
                bw.Write(data);
            }
            return ms2.ToArray();
        }

        /// <summary>
        /// 从nbt数据中心获取md5字符串列表
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static List<string> GetMd5ListFromNBTByteArray(byte[] data, bool is112)
        {
            List<string> md5s = new List<string>();
            MemoryStream ms = new MemoryStream(removeLength(data, is112));
            TagCompound tag = NBTFile.FromStream(ms);
            TagList taglist = (TagList)tag["md5s"];
            foreach (TagByteArray br in taglist)
            {
                byte[] bytes = br.Value;
                if (bytes.Length > 100)
                {
                    string md5 = ASACUtil.RSADecodeMD5(bytes);
                    md5s.Add(md5);
                }
                else
                {
                    md5s.Add(Encoding.UTF8.GetString(bytes));
                }
            }
            return md5s;
        }

        /// <summary>
        /// RSA解密salt(只针对1.0.6,1.0.5,1.0.4)
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string RSADecodeSalt(byte[] data)
        {
            BigInteger modulus = new BigInteger("127165929499203230494093636558638013965252017663799535492473366241186172657381802456786953683177089298103209968185180374096740166047543803456852621212768600619629127825926162262624471403179175000577485553838478368190967564483813134073944752700839742123715548482599351441718070230200126591331603170595424433351");
            BigInteger privateExponent = new BigInteger("8120442115967552979504430611683477858989268564673406717365778685618263462946775764555188689810276923151226539464042905009305546407509816095746345114598417659887966619863710400187548253486545871530930302536230539029867970428580758154100440676071461522806034959078299053007522099777875429363283152166104624633");

            RsaKeyParameters publicParameters = new RsaKeyParameters(true, modulus, privateExponent);
            Pkcs1Encoding eng = new Pkcs1Encoding(new RsaEngine());
            eng.Init(false, publicParameters);

            return Encoding.UTF8.GetString(eng.ProcessBlock(data, 0, data.Length));
        }

        /// <summary>
        /// RSA解密md5(只针对1.0.6,1.0.5,1.0.4)
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string RSADecodeMD5(byte[] data)
        {
            BigInteger modulus = new BigInteger("110765265706288445432931740098429930486184776709780238438557625017629729661573053311960037088088056476891441153774532896215697933861615265976216025080531157954939381061122847093245480153835410088489980899310444547515616362801564379991216339336084947840837937083577860481298666622413144703510357744423856873247");
            BigInteger privateExponent = new BigInteger("46811199235043884723986609175064677734346396089701745030024727102450381043328026268845951862745851965156510759358732282931568208403881136178696846768321267356928789780189985031058525539943424151785807761491334305713351706700232920994479762308513198807509163912459260953727448797718901389753582140608347129153");

            RsaKeyParameters publicParameters = new RsaKeyParameters(true, modulus, privateExponent);
            Pkcs1Encoding eng = new Pkcs1Encoding(new RsaEngine());
            eng.Init(false, publicParameters);

            return Encoding.UTF8.GetString(eng.ProcessBlock(data, 0, data.Length));
        }

        /// <summary>
        /// RSA加密MD5(只针对1.0.6,1.0.5,1.0.4)
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static byte[] RSAEncodeMD5(byte[] data)
        {
            BigInteger modulus = new BigInteger("110765265706288445432931740098429930486184776709780238438557625017629729661573053311960037088088056476891441153774532896215697933861615265976216025080531157954939381061122847093245480153835410088489980899310444547515616362801564379991216339336084947840837937083577860481298666622413144703510357744423856873247");
            BigInteger privateExponent = new BigInteger("65537");

            RsaKeyParameters publicParameters = new RsaKeyParameters(false, modulus, privateExponent);
            Pkcs1Encoding eng = new Pkcs1Encoding(new RsaEngine());
            eng.Init(true, publicParameters);

            return eng.ProcessBlock(data, 0, data.Length);
        }

        /// <summary>
        /// 从本地加载md5列表
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static void LoadMd5ListFromFile()
        {
            if (File.Exists(Global.MD5_FILE_PATH))
            {
                Global.hasMd5File = true;
                Global.md5List = SerializeUtil.DeserializeFromFile<List<string>>(Global.MD5_FILE_PATH);
            }
            else
            {
                Global.hasMd5File = false;
            }
        }

        /// <summary>
        /// 从文件Ian
        /// </summary>
        /// <returns></returns>
        public static List<string> LoadServerUrlFromFile()
        {
            List<string> urls = new List<string>();

            //获取newgui中的地址
            try
            {
                var newGuiFile = ".minecraft/config/New Gui.cfg";
                if (File.Exists(newGuiFile))
                {
                    using (FileStream fs = new FileStream(newGuiFile, FileMode.Open))
                    {
                        using (StreamReader sr = new StreamReader(fs))
                        {
                            string line = sr.ReadLine();
                            while (line != null)
                            {
                                if (line.Contains("S:Address1="))
                                {
                                    urls.Add(line.Replace("S:Address1=", "").Trim());
                                }
                                if (line.Contains("S:Address2="))
                                {
                                    urls.Add(line.Replace("S:Address2=", "").Trim());
                                }
                                line = sr.ReadLine();
                            }
                        }
                    }
                }
            }
            catch { }

            //获取server.data文件中的地址
            try
            {
                string serverFilePath = ".minecraft/servers.dat";
                if (File.Exists(serverFilePath))
                {
                    TagCompound tags = NBTFile.FromFile(serverFilePath);
                    TagList servers = (TagList)tags["servers"];
                    foreach (TagCompound server in servers)
                    {
                        urls.Add(((TagString)server["ip"]).Value);
                    }
                }
            }
            catch { }

            return urls;
        }

        /// <summary>
        /// 获取nbt数据从md5和salt
        /// </summary>
        public static byte[] GetMd5NBTByteArray(bool isUseRSA, List<string> md5s, string salt, bool is112)
        {
            TagCompound tagCompound = new TagCompound();
            TagList tagList = new TagList();
            foreach (string md5 in md5s)
            {
                string newMd5 = EncryptionUtil.MD5(md5 + salt);
                byte[] md5bytes = Encoding.UTF8.GetBytes(newMd5);
                if (isUseRSA)
                {
                    md5bytes = ASACUtil.RSAEncodeMD5(md5bytes);
                }
                TagByteArray byteArray = new TagByteArray(md5bytes);
                tagList.Add(byteArray);
            }
            tagCompound.Add("md5s", tagList);

            MemoryStream tagCompoundMS = new MemoryStream();
            NBTFile.ToStream(tagCompoundMS, tagCompound, !is112);
            byte[] tagCompoundByteArray = tagCompoundMS.ToArray();
            return tagCompoundByteArray;
        }
    }
}
