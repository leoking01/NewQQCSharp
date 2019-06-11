using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApplication1
{
    class User
    {
        public string Name;
        public IPEndPoint ipend;
        public string GetName()
        {
            return this.Name;
        }

        public IPEndPoint GetIPEndPoint()
        {
            return this.ipend;
        }
        public User(string s,IPEndPoint ip)
        {
            this.Name = s;
            this.ipend = ip;
        }
    }
}
