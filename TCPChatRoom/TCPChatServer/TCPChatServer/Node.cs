using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TCPChatServer
{
    class Node : IEnumerable
    {
        public string name;
        public TcpClient tcpClient;
        public Node leftNode; //first child
        public Node rightNode; //second child

        public Node(string name, TcpClient tcpClient)
        {
            this.name = name;
            this.tcpClient = tcpClient;
            this.leftNode = null;
            this.rightNode = null;
        }

        public IEnumerator GetEnumerator()
        {
            if (leftNode != null)
            {
                foreach (var node in leftNode)
                {
                    yield return node;
                }
            }
            yield return this;

            if (rightNode != null)
            {
                foreach (var node in rightNode)
                {
                    yield return node;
                }
            }
        }
    }
}
