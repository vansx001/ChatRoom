using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TCPChatServer
{
    class Server
    {
        public static Dictionary<string, TcpClient> listOfUsers = new Dictionary<string, TcpClient>();
        public static Queue<string> messageQueue = new Queue<string>();
        public static BinaryTree treeOfUsers;
        private static TcpListener listener = null;
        private int port;
        private int bufferSize;

        public Server()
        {
            port = 13000;
            bufferSize = 1024;
            treeOfUsers = new BinaryTree();
        }

        public void StartServer()
        {
            listener = new TcpListener(IPAddress.Any, port);
            Thread listenerThread = new Thread(UserStatus);
            listenerThread.Start();

            try
            {
                StartListening();
                while (true)
                {
                    TcpClient clientSocket = listener.AcceptTcpClient();
                    Thread clientThread = new Thread(() => HandleClient(clientSocket));
                    clientThread.Start();
                }
            }
            catch (Exception error)
            {
                Console.WriteLine(error.ToString());
            }
            finally
            {
                if (listener != null)
                {
                    listener.Stop();
                }
                Console.WriteLine("Chat Server is shutting down.");
                Broadcast("Server", "Chat Server shutting down.");
                Console.ReadLine();
            }
        }

        private void StartListening()
        {
            listener.Start();
            Console.WriteLine("--- CHAT SERVER STARTED ---\n");
            Console.WriteLine("Chat server is running on port {0}.", port);
            Console.WriteLine("\nWaiting for connection...\n");
        }

        public void HandleClient(TcpClient clientSocket)
        {
            try
            {
                byte[] messageSent = new byte[bufferSize];
                string userName;
                NetworkStream networkStream = clientSocket.GetStream();
                messageSent = Encoding.ASCII.GetBytes("--- WELCOME TO SHENG'S * SUPER * AWESOME * GENERIC CHAT ROOM ---\nEnter <exit> to shut down the server at any time.\n\nEnter username:");
                networkStream.Write(messageSent, 0, messageSent.Length);

                userName = GetUserName(networkStream);
                if (userName != "exit")
                {
                    ClientStatus client = new ClientStatus();
                    client.Start(clientSocket, userName);

                    treeOfUsers.Insert(userName, clientSocket);
                    listOfUsers.Add(userName, clientSocket);

                    GiveUserNotification(userName, networkStream);
                }
            }
            catch (Exception error)
            {
                Console.WriteLine("\nError: " + error.ToString());
            }
        }

        public static void Broadcast(string userName, string message)
        {
            try
            {
                foreach (KeyValuePair<string, TcpClient> user in listOfUsers)
                {
                    if (userName != user.Key)
                    {
                        byte[] bytesOut = null;
                        bytesOut = Encoding.ASCII.GetBytes(message);
                        NetworkStream broadcast = user.Value.GetStream();
                        broadcast.Write(bytesOut, 0, bytesOut.Length);
                        broadcast.Flush();
                    }
                }
            }
            catch (Exception error)
            {
                Console.WriteLine("\nError: " + error.ToString());
            }
        }

        public static void ChatMessageQueue(string userName) 
        {
            while (messageQueue.Count > 0)
            {
                byte[] messageToAllUsers = Encoding.ASCII.GetBytes(messageQueue.Dequeue());

                foreach (KeyValuePair<string, TcpClient> user in listOfUsers)
                {
                    if (user.Key != userName)
                    {
                        NetworkStream broadcast = user.Value.GetStream();
                        broadcast.Write(messageToAllUsers, 0, messageToAllUsers.Length);
                        broadcast.Flush();
                    }
                }
            }
        }

        private void UserStatus()
        {
            while (true)
            {
                try
                {
                    if (treeOfUsers.Count() > 0)
                    {
                        foreach (Node node in treeOfUsers)
                        {
                            if (!node.tcpClient.Connected)
                            {
                                listOfUsers.Remove(node.name);
                                messageQueue.Enqueue(node.name + " has been disconnected.");
                                ChatMessageQueue(node.name);
                            }
                        }
                    }
                    treeOfUsers = new BinaryTree();
                    foreach (KeyValuePair<string, TcpClient> user in listOfUsers)
                    {
                        treeOfUsers.Insert(user.Key, user.Value);
                    }
                }
                catch (Exception error)
                {
                    Console.WriteLine("\nError: " + error.ToString());
                }
            }
        }

        private string GetUserName(NetworkStream networkStream)
        {
            byte[] bytesFrom = new byte[bufferSize];
            byte[] bytesSent = new byte[bufferSize];
            networkStream.Read(bytesFrom, 0, bytesFrom.Length);
            string userName = Encoding.ASCII.GetString(bytesFrom);
            userName = userName.Substring(0, userName.IndexOf("\0"));
            if (userName != "exit")
            {
                while (listOfUsers.ContainsKey(userName))
                {
                    bytesSent = Encoding.ASCII.GetBytes("Username is already in use. Please enter a different name.");
                    networkStream.Write(bytesSent, 0, bytesSent.Length);
                    networkStream.Read(bytesFrom, 0, bytesFrom.Length);
                    userName = Encoding.ASCII.GetString(bytesFrom);
                    userName = userName.Substring(0, userName.IndexOf("\0"));
                }
            }
            return userName;
        }

        private void GiveUserNotification(string userName, NetworkStream networkStream)
        {
            byte[] bytesSent = new byte[bufferSize];
            Console.WriteLine(userName + " has joined the chat room.");
            DisplayCurrentUsers(networkStream);

            messageQueue.Enqueue(userName + " has joined the chat room.");
            ChatMessageQueue(userName);
        }

        private void DisplayCurrentUsers(NetworkStream networkStream)
        {
            string message = "\nChat List: ";
            byte[] bytesSent = new byte[bufferSize];

            message += string.Format("{0}", string.Join(", ", listOfUsers.Keys));
            bytesSent = Encoding.ASCII.GetBytes(message);
            networkStream.Write(bytesSent, 0, bytesSent.Length);
        }
    }
}
