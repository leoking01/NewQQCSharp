using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace New_QQ
{
    public partial class Form2 : Form
    {
        public RichTextBox ChatTextBox;
        public RichTextBox MessageTextBox;
        //实名发送
        public IPEndPoint RemoteIpEndPoint;
        byte[] sendBytes = new byte[1024];
        public Form1 form1;
        public IPEndPoint IpEndPoint;
        public UdpClient chatUdpClient;
        private string user;
        public Form2(Form1 f,IPEndPoint ipEnd1,string s)
        {
            Control.CheckForIllegalCrossThreadCalls = false;
            InitializeComponent();
            ChatTextBox = richTextBox1;
            form1 = f;
            RemoteIpEndPoint = ipEnd1;
            user = s;
        }
        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            chatUdpClient=new UdpClient(0);
            string message1 = "talk" + "," + DateTime.Now +","+user+","+richTextBox1.Text;
            sendBytes = Encoding.Unicode.GetBytes(message1);
            chatUdpClient.Send(sendBytes, sendBytes.Length, RemoteIpEndPoint);
            richTextBox1.ResetText();
        }
    }
}
