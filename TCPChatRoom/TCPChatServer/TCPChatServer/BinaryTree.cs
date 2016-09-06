using System;
using System.Collections;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TCPChatServer
{
    class BinaryTree : IEnumerable
    {
        private Node root; //top
        private int count;
        public bool isEmpty {
            get { return root == null; }
        }

        public BinaryTree()
        {
            root = null;
            count = 0;
        }


        public int Count()
        {
            return count;
        }

        public Node Search(string name)
        {
            Node searchNode = root;
            while (searchNode != null)
            {
                if (String.Compare(name, searchNode.name) == 0)
                {
                    return searchNode;
                }
                if (String.Compare(name, searchNode.name) < 0)
                {
                    searchNode = searchNode.leftNode;
                }
                else
                {
                    searchNode = searchNode.rightNode;
                }
            }
            return null;
        }

        private void Add(Node node, ref Node tree)
        {
            if (tree == null)
                tree = node;
            else
            {
                if (String.Compare(node.name, tree.name) == 0)
                    throw new Exception();

                if (String.Compare(node.name, tree.name) < 0)
                {
                    Add(node, ref tree.leftNode);
                }
                else
                {
                    Add(node, ref tree.rightNode);
                }
            }
        }

        public Node Insert(string name, TcpClient client)
        {
            Node nodeToAdd = new Node(name, client);
            if (root == null)
            {
                root = nodeToAdd;
            }
            else
            {
                Add(nodeToAdd, ref root);
            }
            count++;
            return nodeToAdd;
        }

        private Node LocateParent(string name, ref Node parent)
        {
            Node searchNode = root;
            parent = null;

            while (searchNode != null)
            {
                if (String.Compare(name, searchNode.name) == 0)
                    return searchNode;

                if (String.Compare(name, searchNode.name) < 0)
                {
                    parent = searchNode;
                    searchNode = searchNode.leftNode;
                }
                else
                {
                    parent = searchNode;
                    searchNode = searchNode.rightNode;
                }
            }
            return null;
        }

        public Node LocateChild(Node startNode, ref Node parent)
        {
            parent = startNode;
            startNode = startNode.rightNode;
            while (startNode.leftNode != null)
            {
                parent = startNode;
                startNode = startNode.leftNode;
            }
            return startNode;
        }

        public void Delete(string key)
        {
            Node parent = null;
            // find the key and parent
            Node nodeToDelete = LocateParent(key, ref parent);
            if (nodeToDelete == null)
                throw new Exception("Unable to delete: " + key.ToString());

            // no children
            if ((nodeToDelete.leftNode == null) && (nodeToDelete.rightNode == null))
            {
                if (parent == null)
                {
                    root = null;
                    return;
                }

                if (parent.leftNode == nodeToDelete)
                    parent.leftNode = null;
                else
                    parent.rightNode = null;
                count--;
                return;
            }

            //check right side, move child node up
            if (nodeToDelete.leftNode == null)
            {
                if (parent == null)
                {
                    root = nodeToDelete.rightNode;
                    return;
                }
                if (parent.leftNode == nodeToDelete)
                    parent.rightNode = nodeToDelete.rightNode;
                else
                    parent.leftNode = nodeToDelete.rightNode;
                nodeToDelete = null;
                count--;
                return;
            }
            if (nodeToDelete.rightNode == null)
            {	
                if (parent == null)
                {
                    root = nodeToDelete.leftNode;
                    return;
                }
                if (parent.leftNode == nodeToDelete)
                {
                    parent.leftNode = nodeToDelete.leftNode;
                }
                else
                {
                    parent.rightNode = nodeToDelete.leftNode;
                }

                nodeToDelete = null;
                count--;
                return;
            }
            Node child = LocateChild(nodeToDelete, ref parent);
            Node childCopy = new Node(child.name, child.tcpClient);
            if (parent.leftNode == child)
            {
                parent.leftNode = null;
            }
            else
            {
                parent.rightNode = null;
            }
            nodeToDelete.name = childCopy.name;
            nodeToDelete.tcpClient = childCopy.tcpClient;
            count--;
        }
        public IEnumerator GetEnumerator()
        {
            return root.GetEnumerator();
        }
    }
}
