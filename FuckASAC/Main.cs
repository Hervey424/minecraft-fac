using FuckASAC.Helpers;
using FuckASAC.NBT;
using FuckASAC.NBT.IO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FuckASAC
{
    public partial class Main : Form
    {
        private string serverAddress;
        private int port;
        private int localPort;
        private bool status = true;
        private TcpListener listener = null;
        private string ServerAdddress = string.Empty;
        private string localAddress = string.Empty;
        private List<string> md5List = new List<string>();
        private bool hasMd5File = false;
        private string salt = string.Empty;

        public Main()
        {
            InitializeComponent();
            Control.CheckForIllegalCrossThreadCalls = false;
            lblTip.Text = "本地服务器未开启!";
        }

        private void Main_Load(object sender, EventArgs e)
        {
            if (!Directory.Exists(".minecraft"))
            {
                MessageBox.Show("请把软件放到客户端目录再运行!", "错误");
            }

            LoadMd5File();
            LoadServerUrl();
        }

        private void LoadMd5File()
        {
            string file = "md5list.dat";
            if(File.Exists(file))
            {
                hasMd5File = true;
                md5List = FileHelper.ReadMd5List(file);
            }
            else
            {
                hasMd5File = false;
            }
            Message();
        }

        private void Message()
        {
            if(hasMd5File)
            {
                lbMsg.Text = "当前目录存在md5list.data, 请直接加入作弊mod进入游戏!";
            }
            else
            {
                lbMsg.Text = "当前目录不存在md5list.data, 请先不要修改任何mod进入游戏!";
            }
        }


        private void btnStart_Click(object sender, EventArgs e)
        {
            string address = cbServer.Text;
            serverAddress = string.Empty;
            port = 25565;
            string[] arr = address.Split(':');
            if(string.IsNullOrEmpty(address))
            {
                MessageBox.Show("服务器地址填写不正确!", "错误");
                return;
            }
            if(arr.Length != 1 && arr.Length !=2)
            {
                MessageBox.Show("服务器地址填写不正确!","错误");
                return;
            }
            else
            {
                serverAddress = arr[0];
                if (arr.Length == 2)
                {
                    int.TryParse(arr[1], out port);
                }
            }

            Random r = new Random();
            localPort = 25565;
            if(!int.TryParse(txtport.Text,out localPort))
            {
                localPort = r.Next(50000, 60000);
            }
            try
            {
                //监听器
                listener = new TcpListener(IPAddress.Any, localPort);
                listener.Start();
            }
            catch(Exception ex)
            {
                Close("已关闭本地服务器!原因:" + ex.Message);
                return;
            }

            btnStart.Enabled = false;
            btnStop.Enabled = true;
            cbServer.Enabled = false;
            lblTip.Text = "请在服务器地址填写127.0.0.1:"+ localPort + "进入游戏 ";
            txtport.Enabled = false;

            status = true;
            btnCopy.Visible = true;
            localAddress = "127.0.0.1:" + localPort;

            //开启转发服务器线程
            Thread thread = new Thread(new ParameterizedThreadStart(ReceiveClient));
            thread.IsBackground = true;
            thread.Start(listener);

            ServerAdddress = address;
        }

        /// <summary>
        /// 接收请求
        /// </summary>
        /// <param name="obj"></param>
        private void ReceiveClient(object obj)
        {
            TcpListener listener = obj as TcpListener;
            while (true)
            {
                try
                {
                    //接收客户端请求
                    TcpClient client = listener.AcceptTcpClient();

                    //设定超时，否则端口将一直被占用，即使失去连接
                    client.SendTimeout = 300000;
                    client.ReceiveTimeout = 300000;

                    //转发客户端
                    TcpClient transClient = new TcpClient(serverAddress, port);
                    transClient.SendTimeout = 300000;
                    transClient.ReceiveTimeout = 300000;

                    //开启两个线程来
                    dynamic obj1 = new { tc1 = client, tc2 = transClient };
                    dynamic obj2 = new { tc1 = transClient, tc2 = client };
                    Thread ctos = new Thread(new ParameterizedThreadStart(ClientToServer));
                    Thread stoc = new Thread(new ParameterizedThreadStart(ServerToClient));
                    ctos.IsBackground = true;
                    stoc.IsBackground = true;

                    ctos.Start(obj1);
                    stoc.Start(obj2);
                }
                catch(Exception ex)
                {
                    Close("已关闭本地服务器!原因:"+ex.Message);
                    return;
                }
            }
        }

        private void Close(string msg)
        {
            status = false;
            cbServer.Enabled = true;
            btnStart.Enabled = true;
            btnStop.Enabled = false;
            lblTip.Text = msg;
            btnCopy.Visible = false;
            txtport.Enabled = true;
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            Close("已关闭本地服务器");
            try
            {
                listener.Stop();
            }
            catch { };
        }

        /// <summary>
        /// 客户端往服务器端发包
        /// </summary>
        private void ClientToServer(dynamic obj)
        {
            TcpClient tc1 = obj.tc1;
            TcpClient tc2 = obj.tc2;
            NetworkStream ns1 = tc1.GetStream();
            NetworkStream ns2 = tc2.GetStream();
            BinaryReader reader = new BinaryReader(ns1);
            BinaryWriter write = new BinaryWriter(ns2);

            while (status)
            {
                try
                {
                    //读取出来package长度
                    List<byte> readed = new List<byte>();
                    int packageLength = ProtoBufHelper.ReadVarInt(reader, readed);
                    //读取出来package的内容
                    byte[] package = reader.ReadBytes(packageLength);
                    readed.AddRange(package.ToList());
                    byte[] trans = readed.ToArray();

                    using (MemoryStream ms = new MemoryStream(package))
                    {
                        using (BinaryReader br = new BinaryReader(ms))
                        {
                            int packageId = ProtoBufHelper.ReadVarInt(br);

                            //插件消息
                            if (packageId == 0x17)
                            {
                                string channelName = ProtoBufHelper.ReadString(br);
                                int datalength = br.ReadInt16BE();
                                if (channelName.Contains("anotheranticheat") 
                                    || channelName.Contains("anotherstaranticheat")
                                    || channelName.Contains("AnotherStarAntiCheat"))
                                {
                                    var data = br.ReadBytes(datalength);
                                    if(hasMd5File)
                                    {
                                        bool isUseRSA = NBTHelper.GetIsUseRSA(data);
                                        TagCompound tagCompound = new TagCompound();
                                        TagList tagList = new TagList();
                                        foreach(string md5 in md5List)
                                        {
                                            string newMd5 = Crypto.Md5(md5 + salt);
                                            byte[] md5bytes = Encoding.UTF8.GetBytes(newMd5);
                                            if(isUseRSA)
                                            {
                                                md5bytes = RSAHelper.GetMD5Byte(md5bytes);
                                            }
                                            TagByteArray byteArray = new TagByteArray(md5bytes);
                                            tagList.Add(byteArray);
                                        }
                                        tagCompound.Add("md5s", tagList);

                                        MemoryStream tagCompoundMS = new MemoryStream();
                                        NBTFile.ToStream(tagCompoundMS, tagCompound, true);
                                        byte[] tagCompoundByteArray = tagCompoundMS.ToArray();

                                        MemoryStream ms2 = new MemoryStream();
                                        using (BinaryWriter bw = new BinaryWriter(ms2))
                                        {
                                            bw.Write((byte)data[0]);
                                            bw.WriteUInt16BE((UInt16)tagCompoundByteArray.Length);
                                            bw.Write(tagCompoundByteArray);
                                        }
                                        byte[] bytes = ms2.ToArray();
                                        write.WritePluginData(0x17, channelName, bytes);
                                        continue;
                                    }
                                    else
                                    {
                                        md5List = NBTHelper.GetMd5List(data);
                                        FileHelper.SaveMd5List("md5list.dat", md5List);
                                        lbMsg.Text = "当前目录存在md5list.data, 请直接加入作弊mod进入游戏!";
                                        hasMd5File = true;
                                    }
                                }
                            }
                        }
                    }

                    write.Write(trans);
                }
                catch
                {
                    break;
                }
            }

            write.Close();
            reader.Close();
            ns1.Dispose();
            ns2.Dispose();
            tc1.Close();
            tc2.Close();
        }

        /// <summary>
        /// 客户端往服务端发包
        /// </summary>
        /// <param name="obj"></param>
        private void ServerToClient(dynamic obj)
        {
            TcpClient tc1 = obj.tc1;
            TcpClient tc2 = obj.tc2;
            NetworkStream ns1 = tc1.GetStream();
            NetworkStream ns2 = tc2.GetStream();
            BinaryReader reader = new BinaryReader(ns1);
            BinaryWriter write = new BinaryWriter(ns2);
            while (status)
            {
                try
                {
                    //读取出来package长度
                    List<byte> readed = new List<byte>();
                    int packageLength = ProtoBufHelper.ReadVarInt(reader, readed);
                    //读取出来package的内容
                    byte[] package = reader.ReadBytes(packageLength);
                    readed.AddRange(package.ToList());
                    byte[] trans = readed.ToArray();

                    try
                    {
                        using (MemoryStream ms = new MemoryStream(package))
                        {
                            using (BinaryReader br = new BinaryReader(ms))
                            {
                                int packageId = ProtoBufHelper.ReadVarInt(br);
                                if (packageId == 0x3F)
                                {
                                    string channelName = ProtoBufHelper.ReadString(br);
                                    if (channelName == "anotheranticheat" 
                                        || channelName.Contains("anotherstaranticheat")
                                        || channelName.Contains("AnotherStarAntiCheat"))
                                    {
                                        int datalength = br.ReadInt16BE();
                                        byte[] data = br.ReadBytes(datalength);
                                        if(hasMd5File)
                                        {
                                            salt = NBTHelper.GetSalt(data);
                                        }
                                        else
                                        {
                                            byte[] emptySaltArray = NBTHelper.getSaltByteArray(string.Empty);
                                            write.WritePluginData(0x3f, channelName, emptySaltArray);
                                            continue;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch { }


                    write.Write(trans);
                }
                catch
                {
                    break;
                }
            }

            write.Close();
            reader.Close();
            ns1.Dispose();
            ns2.Dispose();
            tc1.Close();
            tc2.Close();
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            try
            {
                Clipboard.SetDataObject(localAddress);
            }
            catch
            {
                MessageBox.Show("复制到剪切板失败,请手动添加" + localAddress + "到服务器列表");
            }
        }

        private void LoadServerUrl()
        {
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
                                    cbServer.Items.Add(line.Replace("S:Address1=", "").Trim());
                                }
                                if (line.Contains("S:Address2="))
                                {
                                    cbServer.Items.Add(line.Replace("S:Address2=", "").Trim());
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
                        cbServer.Items.Add(((TagString)server["ip"]).Value);
                    }
                }
            }
            catch { }

            if(cbServer.Items.Count > 0)
            {
                cbServer.SelectedIndex = 0;
            }
        }
    }
}
