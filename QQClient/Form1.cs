using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
//www.srcfans.com
namespace New_QQ
{
    public partial class Form1 : Form
    {
        private static IPAddress clientIP;
        public static IPEndPoint clientIPEndPoint;
        private static UdpClient receiveUdpClient;
        private static IPEndPoint remoteIpEndPoint;
        private static IPEndPoint serverIpEndPoint;
        private static TcpClient tcpClient;
        public static UdpClient sendUdpClient;
        public static UdpClient chatClient;
        private static string userListstring;
        private static BinaryReader binaryReader;
        private static NetworkStream networkStream;
        private static Thread receiveThread;
        private static Thread sendThread;
        public Form1()
        {
            Control.CheckForIllegalCrossThreadCalls = false;
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //创建接受套接字
            clientIP = IPAddress.Parse(txtLocalIP.Text);
            clientIPEndPoint = new IPEndPoint(clientIP, int.Parse(txtlocalport.Text));
            serverIpEndPoint=new IPEndPoint(IPAddress.Parse(txtserverIP.Text),int.Parse(txtServerPort.Text) );
            receiveUdpClient = new UdpClient(clientIPEndPoint);
            //启动接受线程
            receiveThread = new Thread(ReceiveMessage);
            receiveThread.Start();
            //匿名发送
            sendUdpClient = new UdpClient(0);
            //启动发送线程
            sendThread = new Thread(SendMessage);
            sendThread.Start(string.Format("login,{0},{1}", txtusername.Text, clientIPEndPoint));
            button1.Enabled = false;
            this.Text = txtusername.Text;
            button2.Enabled = true;
            this.ControlBox = false;
        }

        private void ReceiveMessage()
        {
            remoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);
            while (true)
            {
                try
                {
                    //关闭receiveUdpClient时会产生异常
                    byte[] receiveBytes = receiveUdpClient.Receive(ref remoteIpEndPoint);
                    string message = Encoding.Unicode.GetString(receiveBytes, 0, receiveBytes.Length);
                    //处理消息
                    string[] splitstring = message.Split(',');
                    switch (splitstring[0])
                    {
                        case "Accept":
                            try
                            {
                                tcpClient = new TcpClient();
                                tcpClient.Connect(remoteIpEndPoint.Address, int.Parse(txtServerPort.Text));
                                if (tcpClient != null)
                                {
                                    //表示连接成功
                                    networkStream = tcpClient.GetStream();
                                    binaryReader = new BinaryReader(networkStream);
                                }
                            }
                            catch
                            {

                                MessageBox.Show("连接失败");
                            }
                            Thread getUserListThread = new Thread(GetUserList);
                            getUserListThread.Start();
                            break;
                        case "login":                        
                            listView1.Items.Add(splitstring[1]+","+splitstring[2]);
                            break;
                        case "logout":
                            string userItem = splitstring[1] + "," + splitstring[2];
                            foreach (ListViewItem lvItem in listView1.Items)
                            {
                                if (lvItem.Text==userItem)
                                {
                                    listView1.Items.Remove(lvItem);
                                }
                            }
                            break;
                        case "talk":
                            string message2 = splitstring[2] + "   " + splitstring[1] + '\n' + "  " + splitstring[3]+'\n';
                            richTextBox1.AppendText(message2);
                            break;
                        case "qunliao":
                            string message3 = splitstring[2] + "   " + splitstring[1] + '\n' + "  " + splitstring[3]+'\n';
                            richTextBox2.AppendText(message3);
                            break;
                    }
                }
                catch
                {

                    break;
                }
            }
        }
        //通过服务器获得在线的好友列表  Tcp协议
        private void GetUserList()
        {
            while (true)
            {
                userListstring = null;
                try
                {
                    userListstring = binaryReader.ReadString();
                    if (userListstring.EndsWith("end"))
                    {
                        string[] splitstring = userListstring.Split(';');
                        for (int i = 0; i < splitstring.Length - 1; i++)
                        {
                            listView1.Items.Add(splitstring[i]);
                        }
                        binaryReader.Close();
                        tcpClient.Close();
                        break;
                    }
                }
                catch
                {

                    break;
                }
            }
        }

        public void SendMessage(object obj)
        {
            string message = (string)obj;
            byte[] sendBytes = Encoding.Unicode.GetBytes(message);
            IPAddress remoteIp = IPAddress.Parse(txtserverIP.Text);
            IPEndPoint remoteIpEndPoint = new IPEndPoint(remoteIp, int.Parse(txtServerPort.Text));
            sendUdpClient.Send(sendBytes, sendBytes.Length, remoteIpEndPoint);
        }
        //注销按钮 通知服务器注销 Udp协议
        private void button2_Click(object sender, EventArgs e)
        {
            SendMessage(string.Format("logout,{0},{1}", txtusername.Text, clientIPEndPoint));
            tcpClient.Close();
            receiveUdpClient.Close();
            receiveThread.Abort();
            sendThread.Abort();
            button2.Enabled = false;
            button1.Enabled = true;
            Application.Exit();
            Application.ExitThread();
        }
        //双击好友列表进行p2p模式聊天  Udp协议
        private void listView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            string userinfo = listView1.FocusedItem.Text.ToString();
            string[] array = userinfo.Split(',');
            string[] array1 = array[1].Split(':');
            IPEndPoint chatobject=new IPEndPoint(IPAddress.Parse(array1[0]),int.Parse(array1[1]));
            Form2 a = new Form2(this, chatobject, txtusername.Text);
            a.Text = "与"+array[0]+"聊天中";
            a.Show();
        }
        //对当前在线好友群发消息 通过服务器转发 
        private void button3_Click(object sender, EventArgs e)
        {
            Form3 b=new Form3(serverIpEndPoint,txtusername.Text);
            b.Text = "与当前在线好友群聊中";
            b.Show();
        }
    }
}
