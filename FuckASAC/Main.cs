using FuckASAC.Utils;
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
using FuckASAC.Models;

namespace FuckASAC
{
    public partial class Main : Form
    {
        private string serverAddress;
        private int port;
        private bool status = true;
        private TcpListener listener = null;
        private string ServerAdddress = string.Empty;
        private string localAddress = string.Empty;

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
            ASACUtil.LoadMd5ListFromFile();
            cbServer.DataSource = ASACUtil.LoadServerUrlFromFile();
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

            try
            {
                //监听器
                listener = new TcpListener(IPAddress.Any, Global.LOCAL_PORT);
                listener.Start();
            }
            catch(Exception ex)
            {
                Close("已关闭本地服务器!原因:" + ex.Message);
                return;
            }

            Global.IsVersion1_12_2 = rb1122.Checked;

            btnStart.Enabled = false;
            btnStop.Enabled = true;
            cbServer.Enabled = false;
            rb1122.Enabled = false;
            rb1710.Enabled = false;
            lblTip.Text = "请在服务器地址填写127.0.0.1:"+ Global.LOCAL_PORT + "进入游戏 ";

            status = true;
            btnCopy.Visible = true;
            localAddress = "127.0.0.1:" + Global.LOCAL_PORT;

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
                    Global.IsCompression = false;
                    Global.CompressionThreshold = 0;

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
            rb1122.Enabled = true;
            rb1710.Enabled = true;
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
            Global.ToServerWriter = write;

            while (status)
            {
                try
                {
                    Package package = reader.ReadPackage();

                    if (Plugins.Plugins.CTSHandle(package, write))
                    {
                        continue;
                    }

                    write.Write(package.OriginData);
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
            Global.ToClientWriter = write;

            while (status)
            {
                try
                {
                    Package package = reader.ReadPackage();

                    if (Plugins.Plugins.STCHandle(package, write))
                    {
                        continue;
                    }

                    // 设置压缩阈值
                    if (Global.IsVersion1_12_2 && Global.IsCompression == false && package.PackageId == 0x03)
                    {
                        int maximum = ProtoBufUtil.GetVarIntFromBytes(package.Data);
                        if (maximum >= 0)
                        {
                            Global.IsCompression = true;
                            Global.CompressionThreshold = maximum;
                        }
                    }

                    write.Write(package.OriginData);
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
    }
}
