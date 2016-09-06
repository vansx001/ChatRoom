using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TCPChatServer
{
    class User
    {
        private TcpClient tcpClient;
        private string userName;

        public User(TcpClient tcpClient, string userName)
        {
            this.tcpClient = tcpClient;
            this.userName = userName;
        }

        public TcpClient TcpClient
        {
            get { return tcpClient; }
            set { tcpClient = value; }
        }

        public string UserName
        {
            get { return userName; }
            set { userName = value; }
        }
    }
}
