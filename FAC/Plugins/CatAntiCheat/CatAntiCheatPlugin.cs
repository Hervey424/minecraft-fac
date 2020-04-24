using FAC.Models;
using FAC.Plugins.CatAntiCheat;
using FAC.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace FAC.Plugins
{
    /// <summary>
    /// CatAntiCheat处理
    /// 流程:
    /// 1. S->C:服务端发送给客户端HelloPackage, 里面包含salt
    /// 2. C->S:客户端给服务端发送salt(不做任何修改的salt)和客户端版本
    /// 3. S->C:服务端发送给客户端DataCheck
    /// 4. C->S:客户端给服务端发送结果(透视和lighting)
    /// 5. S->C:服务端发送给客户端FileCheck
    /// 6. C->S:客户端发送给服务端salt, hash列表, 签名
    /// </summary>
    public class CatAntiCheatPlugin : AbsoluteMessagePlugin
    {

        public CatAntiCheatPlugin()
        {
            if(File.Exists(fileName))
            {
                LocalData = SerializeUtil.DeserializeFromFile<CatAntiCheatData>(fileName);
            }
        }

        /// <summary>
        /// 本地保存的数据
        /// </summary>
        private CatAntiCheatData LocalData;

        /// <summary>
        /// channel列表
        /// </summary>
        public override List<string> Channels => new List<string> { "catanticheat" };

        /// <summary>
        /// 文件名称
        /// </summary>
        private readonly string fileName = "cat-hashlist.dat";

        /// <summary>
        /// salt
        /// </summary>
        private byte salt = 0;

        /// <summary>
        /// version
        /// </summary>
        private short version = 0;

        /// <summary>
        /// 客户端->服务端
        /// </summary>
        /// <param name="package"></param>
        /// <param name="write"></param>
        /// <returns></returns>
        public override bool CTSPluginMessageHandle(PackagePlugin package, BinaryWriter toClientWriter, BinaryWriter toServerWriter)
        {
            int type = package.ChannelData[0];

            // HelloReplay, 只保存version , 其他不处理
            if (type == (int)CatAntiCheatMessageType.CTSHelloReply)
            {
                version = BitConverter.ToInt16(new byte[] { package.ChannelData[2], package.ChannelData[1] }, 0);
            }
            // FileHash
            else if(type == (int)CatAntiCheatMessageType.CTSFileHash)
            {
                return FileHashHandle(package, toClientWriter, toServerWriter);
            }

            return false;
        }

        /// <summary>
        /// 服务端->客户端
        /// </summary>
        /// <param name="package"></param>
        /// <param name="write"></param>
        /// <returns></returns>
        public override bool STCPluginMessageHandle(PackagePlugin package, BinaryWriter toClientWriter, BinaryWriter toServerWriter)
        {
            int type = package.ChannelData[0];

            // hello package
            if (type == (int)CatAntiCheatMessageType.STCHello)
            {
                return HelloHandle(package, toClientWriter, toServerWriter);
            }
            // datacheck, 直接返回无问题即可
            else if (type == (int)CatAntiCheatMessageType.STCDataCheck)
            {
                return DataCheckHandle(package, toClientWriter, toServerWriter);
            }
            // filecheck
            else if (type == (int)CatAntiCheatMessageType.STCFileCheck)
            {
                return FileCheckHandle(package, toClientWriter, toServerWriter);
            }
            // classcheck
            else if (type == (int)CatAntiCheatMessageType.STCClassCheck)
            {
                return ClassCheckHandle(package, toClientWriter, toServerWriter);
            }
            // 截图
            else if (type == (int)CatAntiCheatMessageType.STCScreenShot)
            {
                return ScreenShotHandle(package, toClientWriter, toServerWriter);
            }
            return false;
        }

        /// <summary>
        /// 服务端发送截图
        /// </summary>
        /// <param name="package"></param>
        /// <param name="toClientWriter"></param>
        /// <param name="toServerWriter"></param>
        /// <returns></returns>
        private bool ScreenShotHandle(PackagePlugin package, BinaryWriter toClientWriter, BinaryWriter toServerWriter)
        {
            if(!File.Exists("screen.png"))
            {
                Console.WriteLine($"[CatAntiCheat]收到服务端截图请求, screen.png不存在, 不返回任何内容 , 建议你立即换号");
                return true;
            }
            byte[] bytes = File.ReadAllBytes("screen.png");
            using (MemoryStream ms = new MemoryStream(GzipUtil.Compress(bytes)))
            {
                using (BinaryReader br = new BinaryReader(ms))
                {
                    while(true)
                    {
                        List<byte> data = new List<byte>();

                        byte[] bs = br.ReadBytes(32763);
                        bool end = bs.Length != 32763;

                        data.Add((byte)CatAntiCheatMessageType.CTSImageData);
                        data.Add(end ? (byte)1 : (byte)0);
                        data.AddRange(bs);

                        PackagePlugin fake = new PackagePlugin(CTSPluginPackageId, package.ChannelName, data.ToArray(), Global.IsVersion1_12_2);
                        toServerWriter.Write(fake.OriginData);

                        if(end)
                        {
                            break;
                        }
                    }
                    Console.WriteLine($"[CatAntiCheat]收到服务端截图请求, 返回screen.png");
                }
            }
            return true;
        }

        /// <summary>
        /// 处理服务端classcheck
        /// 1.2.7 之后会随机发送一个类, 所以这里如果找到了就返回
        /// 如果找不到就说明是之前的版本, 就返回别的
        /// </summary>
        /// <param name="package"></param>
        /// <param name="toClientWriter"></param>
        /// <param name="toServerWriter"></param>
        /// <returns></returns>
        private bool ClassCheckHandle(PackagePlugin package, BinaryWriter toClientWriter, BinaryWriter toServerWriter)
        {
            List<string> randomClassNames = new List<string>
            {
                // v1.2.7
                "net.minecraft.launchwrapper.Launch",
                "net.minecraft.launchwrapper.LogWrapper",
                "net.minecraft.launchwrapper.AlphaVanillaTweaker",
                // v1.2.8
                "net.minecraft.launchwrapper.injector.VanillaTweakInjector", 
                "io.netty.bootstrap.Bootstrap"
            };
            using (MemoryStream ms = new MemoryStream(package.ChannelData, 1, package.ChannelData.Length - 1))
            {
                using (BinaryReader br = new BinaryReader(ms))
                {
                    Int16 size = br.ReadInt16BE();
                    List<string> classNames = new List<string>();
                    for(int i = 0;i< size;i++)
                    {
                        classNames.Add(br.ReadVarString());
                    }
                    var exist = classNames.Intersect(randomClassNames).FirstOrDefault();
                    List<byte> bytes = new List<byte>();
                    if(string.IsNullOrEmpty(exist))
                    {
                        bytes = new List<byte> { (byte)CatAntiCheatMessageType.CTSClassFound, 0, 0, salt };
                        Console.WriteLine($"[CatAntiCheat]收到服务端ClassCheck请求, 直接返回无作弊!");
                    }
                    else
                    {
                        bytes = new List<byte> { (byte)CatAntiCheatMessageType.CTSClassFound };
                        bytes.AddRange(BitConverter.GetBytes((short)1).Reverse());
                        bytes.AddRange(ProtoBufUtil.GetVarStringBytes(exist));
                        bytes.Add(salt);
                        Console.WriteLine($"[CatAntiCheat]收到服务端ClassCheck请求, 直接返回[{exist}]");
                    }

                    PackagePlugin fake = new PackagePlugin(CTSPluginPackageId, package.ChannelName, bytes.ToArray(), Global.IsVersion1_12_2);
                    toServerWriter.Write(fake.OriginData);
                    return true;
                }
            }
        }

        /// <summary>
        /// 处理filehash
        /// 保存列表文件并且发送列表
        /// </summary>
        /// <param name="package"></param>
        /// <param name="write"></param>
        /// <returns></returns>
        private bool FileHashHandle(PackagePlugin package, BinaryWriter toClientWriter, BinaryWriter toServerWriter)
        {
            if(LocalData == null)
            {
                using (MemoryStream ms = new MemoryStream(package.ChannelData, 1, package.ChannelData.Length - 1))
                {
                    using (BinaryReader br = new BinaryReader(ms))
                    {
                        // 0-2: gziplength
                        // 2-~: data
                        ushort gzipLength = br.ReadUInt16BE();
                        byte[] gzipdata = br.ReadBytes(gzipLength);
                        // 0: salt
                        // 1-~: hashlist
                        byte[] unzipdata = GzipUtil.Decompress(gzipdata);
                        byte[] hashs = new byte[unzipdata.Length - 1];
                        Array.Copy(unzipdata, 1, hashs, 0, hashs.Length);

                        CatAntiCheatData local = new CatAntiCheatData
                        {
                            Version = version,
                            FileHashBytes = hashs
                        };

                        SerializeUtil.SerializeToFile(fileName, local);
                    }
                }
                Console.WriteLine($"[CatAntiCheat]本地文件[{fileName}]不存在, 创建完成");
            }
            LocalData = SerializeUtil.DeserializeFromFile<CatAntiCheatData>(fileName);

            return false;
        }

        /// <summary>
        /// 处理filecheck
        /// </summary>
        /// <param name="package"></param>
        /// <param name="write"></param>
        /// <returns></returns>
        private bool FileCheckHandle(PackagePlugin package, BinaryWriter toClientWriter, BinaryWriter toServerWriter)
        {
            // 如果本地文件存在, 就发送本地文件
            if(LocalData != null)
            {
                List<byte> unzipedBytes = new List<byte>();
                // 添加salt和hash列表
                unzipedBytes.Add(salt);
                unzipedBytes.AddRange(LocalData.FileHashBytes);
                byte[] zipedData = GzipUtil.Compress(unzipedBytes.ToArray());

                List<byte> buffers = new List<byte>();
                //type
                buffers.Add((byte)CatAntiCheatMessageType.CTSFileHash);
                //gzip数据长度
                buffers.AddRange(BitConverter.GetBytes((short)zipedData.Length).Reverse());
                //zip数据
                buffers.AddRange(zipedData);
                //sign
                buffers.AddRange(BitConverter.GetBytes((int)hashCode(zipedData)).Reverse());

                Console.WriteLine($"[CatAntiCheat]收到服务端FileCheck请求, 直接返回本地{fileName}中的数据!");

                PackagePlugin fakerPackage = new PackagePlugin(CTSPluginPackageId, package.ChannelName, buffers.ToArray(), Global.IsVersion1_12_2);
                toServerWriter.Write(fakerPackage.OriginData);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 处理DataCheck
        /// </summary>
        /// <param name="package"></param>
        /// <param name="write"></param>
        /// <returns></returns>
        private bool DataCheckHandle(PackagePlugin package, BinaryWriter toClientWriter, BinaryWriter toServerWriter)
        {
            Console.WriteLine($"[CatAntiCheat]收到服务端DataCheck请求, 直接返回无作弊!");
            //lighting和transparentTexture
            byte[] bytes = { (byte)CatAntiCheatMessageType.CTSDataCheckReply, 0, 0 };
            PackagePlugin fakerPackage = new PackagePlugin(CTSPluginPackageId, package.ChannelName, bytes, Global.IsVersion1_12_2);
            toServerWriter.Write(fakerPackage.OriginData);
            return true;
        }

        /// <summary>
        /// 处理hellopackage
        /// </summary>
        /// <param name="package"></param>
        /// <param name="write"></param>
        /// <returns></returns>
        private bool HelloHandle(PackagePlugin package, BinaryWriter toClientWriter, BinaryWriter toServerWriter)
        {
            // 保存salt
            salt = package.ChannelData[1];

            if (LocalData != null)
            {
                byte[] versionBytes = BitConverter.GetBytes(LocalData.Version);
                // 共四位 0-type 12-version 3-salt
                byte[] bytes = { (byte)CatAntiCheatMessageType.CTSHelloReply, versionBytes[1], versionBytes[0], salt };
                PackagePlugin fakerPackage = new PackagePlugin(CTSPluginPackageId, package.ChannelName, bytes, Global.IsVersion1_12_2);
                toServerWriter.Write(fakerPackage.OriginData);
                Console.WriteLine($"[CatAntiCheat]收到服务端Hello请求 Salt={salt}, 返回Salt={salt}, Version={LocalData.Version}");
                return true;
            }

            return false;
        }

        /// <summary>
        /// hashcode
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        private int hashCode(byte[] a)
        {
            if (a == null)
                return 0;

            int result = 1;
            foreach (byte element in a)
            {
                sbyte e2 = element > 127 ? (sbyte)(element - 256) : (sbyte)element;
                result = 31 * result + e2;
            }

            return result;
        }
    }
}
