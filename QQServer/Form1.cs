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
namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        //private static ListBox listbox1 = listbox1;
        //private static int tcpPort;
        private static UdpClient receiveUdpClient;
        private static TcpListener tcpListener;

        private static IPAddress serverIp;
        private static IPEndPoint serverIPEndPoint;
        private static IPEndPoint remoteIPEndPoint;

        Thread listenThread;
        Thread receiveThread;

        bool stopFlag_listenClientConnect;
        bool stopFlag_receiveMessage;


        public Form1()
        {
            Control.CheckForIllegalCrossThreadCalls = false;
            InitializeComponent();

            txbServerIP.Text = "127.0.0.1";
            txbServerIP.Update();

            txbServerPort.Text = "8000";
            txbServerPort.Update();

            stopFlag_listenClientConnect = false;
            stopFlag_receiveMessage = false;
        }

        public int port = 500;
        private List<User> userList = new List<User>();

        //启动服务
        private void btnStart_Click(object sender, EventArgs e)
        {
            //创建接收套接字
            serverIp = IPAddress.Parse(txbServerIP.Text);
            serverIPEndPoint = new IPEndPoint(serverIp, int.Parse(txbServerPort.Text));
            receiveUdpClient = new UdpClient(serverIPEndPoint);

            //启动接收信息的线程
            receiveThread = new Thread(ReceiveMessage);
            receiveThread.Start();
            btnStart.Enabled = false;

            //创建监听套接字
            tcpListener = new TcpListener(serverIp, int.Parse(txbServerPort.Text));
            tcpListener.Start(100);

            //启动监听线程
            listenThread = new Thread(ListenClientConnect);
            listenThread.Start();

            listBox1.Items.Add(string.Format("[1][服务端启动]服务器线程{0}启动，监听端口{1}", serverIPEndPoint, int.Parse(txbServerPort.Text)));

            stopFlag_listenClientConnect = false;
            stopFlag_receiveMessage = false;
        }

        //接收消息
        private void ReceiveMessage()
        {
            remoteIPEndPoint = new IPEndPoint(IPAddress.Any, 0);
            while (true)
            {
                try
                {
                    if(stopFlag_receiveMessage)
                        break;

                    string[] splitsubstring = null;
                    IPEndPoint clientIPEndPoint = null;
                    //关闭receiveUdpClient时下面一行代码会产生异常
                    byte[] receiveBytes = receiveUdpClient.Receive(ref remoteIPEndPoint);
                    string message = Encoding.Unicode.GetString(receiveBytes, 0, receiveBytes.Length);
                    //显示消息内容
                    listBox1.Items.Add(string.Format("[服务端接收数据]{0}:{1}", remoteIPEndPoint, message));
                    //处理消息数据
                    //服务器接受消息后做处理
                    string[] splitstring = message.Split(',');
                    //解析用户端地址
                    if (splitstring[0] != "qunliao")
                    {
                        splitsubstring = splitstring[2].Split(':');
                        clientIPEndPoint = new IPEndPoint(IPAddress.Parse(splitsubstring[0]), int.Parse(splitsubstring[1]));
                    }

                    switch (splitstring[0])
                    {
                        //若为登录信息
                        case "login":
                            User user = new User(splitstring[1], clientIPEndPoint);
                            //往在线的用户列表添加新成员
                            userList.Add(user);
                            listBox1.Items.Add(string.Format("[服务端接收数据]用户{0}{1}加入", user.GetName(), user.GetIPEndPoint()));
                            string sendString = "Accept," + txbServerPort.Text;
                            //向客户端发送应答消息
                            SendtoClient(user, sendString);
                            listBox1.Items.Add(string.Format("[服务端接收数据]向{0}{1}发出:[{2}]", user.GetName(), user.GetIPEndPoint(), sendString));
                            for (int i = 0; i < userList.Count; i++)
                            {
                                if (userList[i].GetName() != user.GetName())
                                {
                                    //给在线的其他用户发送广播消息
                                    //通知有新用户加入
                                    SendtoClient(userList[i], message);
                                }
                            }
                            listBox1.Items.Add(string.Format("[服务端接收数据]广播:[{0}]", message));
                            break;
                        case "logout":
                            for (int i = 0; i < userList.Count; i++)
                            {
                                if (userList[i].GetName() == splitstring[1])
                                {
                                    userList.RemoveAt(i); //移除用户
                                    listBox1.Items.Add(string.Format("[服务端接收数据]用户{0}({1})退出", userList[i].GetName(), userList[i].GetIPEndPoint()));
                                }
                                for (int j = 0; j < userList.Count; j++)
                                {
                                    //广播注销消息
                                    SendtoClient(userList[i], message);
                                }
                                listBox1.Items.Add(string.Format("[服务端接收数据]广播:[{0}]", message));
                                break;
                            }
                            break;
                        case "qunliao":
                            listBox1.Items.Add("[服务端接收数据]收到群发消息");
                            for (int j = 0; j < userList.Count; j++)
                            {
                                //广播注销消息
                                SendtoClient(userList[j], message);
                            }
                            listBox1.Items.Add(string.Format("[服务端接收数据]广播:[{0}]", message));
                            break;
                    }
                }
                catch (Exception)
                {
                    break;
                }
                listBox1.Items.Add(string.Format("[服务端接收数据]serverIPEndPoint={0} 接收消息   ", serverIPEndPoint));
            }
        }


        //向客户端发送消息
        private void SendtoClient(User user, string message)
        {
            //匿名方式发送
            UdpClient sendUdpClient = new UdpClient(0);
            byte[] sendBytes = Encoding.Unicode.GetBytes(message);
            remoteIPEndPoint = user.GetIPEndPoint();
            sendUdpClient.Send(sendBytes, sendBytes.Length, remoteIPEndPoint);
            sendUdpClient.Close();
        }



        //接受客户端的连接
        private void ListenClientConnect()
        {
            TcpClient newClient = null;
            while (true)
            {
                try
                {
                    newClient = tcpListener.AcceptTcpClient();
                    listBox1.Items.Add(string.Format("[监听连接]:接受客户端{0}的TCP请求", newClient.Client.RemoteEndPoint));

                    Thread sendThread = new Thread(SendData);
                    sendThread.Start(newClient);

                    if (stopFlag_listenClientConnect)
                        break;
                }
                catch (Exception)
                {
                    listBox1.Items.Add(string.Format("[监听连接(异常)]:监听线程({0}:{1})", serverIp, int.Parse(txbServerPort.Text)));
                    break;
                }

            }
        }


        // 向客户端发送在线用户列表信息
        // 服务器通过TCP连接把在线用户列表信息发送给客户端
        private void SendData(object userClient)
        {
            TcpClient newUserClient = (TcpClient)userClient;
            string userListstring = null;
            for (int i = 0; i < userList.Count; i++)
            {
                userListstring += userList[i].GetName() + "," + userList[i].GetIPEndPoint().ToString() + ";";
            }
            userListstring += "end";
            NetworkStream networkStream = newUserClient.GetStream();
            BinaryWriter binaryWriter = new BinaryWriter(networkStream);
            binaryWriter.Write(userListstring);
            binaryWriter.Flush();
            listBox1.Items.Add(string.Format("[服务端向客户端发送数据]向{0}发送[{1}]", 
                newUserClient.Client.RemoteEndPoint, userListstring));
            binaryWriter.Close();
            newUserClient.Close();
        }


        //退出服务端应用
        private void button1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }


        //停止服务---没写好。
        private void button2_Click(object sender, EventArgs e)
        {
            receiveThread.Suspend();
            receiveThread.Abort();
            listenThread.Suspend();  //();
            listenThread.Abort();

            receiveUdpClient.Close();
            tcpListener.Stop();  //100);

            stopFlag_listenClientConnect = true;
            stopFlag_receiveMessage = true;


            listBox1.Items.Add(string.Format("(没写好 )服务器线程{0}停止，监听端口{1}", serverIPEndPoint, int.Parse(txbServerPort.Text)));
        }
    }
}
