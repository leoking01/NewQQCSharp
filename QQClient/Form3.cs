using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace New_QQ
{
    public partial class Form3 : Form
    {
        private byte[] SendBytes;
        public IPEndPoint ServerIpEndPoint;
        public string user;
        public UdpClient Udp;
        public Form3(IPEndPoint ipEndPoint,string str)
        {
            InitializeComponent();
            ServerIpEndPoint = ipEndPoint;
            user = str;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Udp=new UdpClient(0);
            string message = "qunliao" + "," + DateTime.Now + "," + user + "," + richTextBox1.Text;
            SendBytes = Encoding.Unicode.GetBytes(message);
            Udp.Send(SendBytes, SendBytes.Length, ServerIpEndPoint);
            richTextBox1.ResetText();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
