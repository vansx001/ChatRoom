using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TCPChatServer
{
    class ClientStatus
    {
        TcpClient clientSocket;
        string userName;
        private int bufferSize = 1024;

        public void Start(TcpClient clientSocket, string userName)
        {
            this.clientSocket = clientSocket;
            this.userName = userName;

            Thread clientThread = new Thread(Communicate);
            clientThread.Start();
        }

        private void Communicate()
        {
            try
            {
                while (clientSocket.Connected)
                {
                    byte[] bytesFrom = new byte[bufferSize];
                    string userInput = null;
                    NetworkStream networkStream = clientSocket.GetStream();
                    networkStream.Read(bytesFrom, 0, bytesFrom.Length);

                    userInput = Encoding.ASCII.GetString(bytesFrom);

                    userInput = userInput.Substring(0, userInput.IndexOf("\0"));

                    if (userInput.ToLower() == "exit")
                    {
                        ProcessExitingClient(userName);
                        break;
                    }
                    else
                    {
                        userInput = "<" + userName + "> " + userInput;
                        Server.messageQueue.Enqueue(userInput);
                        Server.ChatMessageQueue(userName);
                    }
                }
            }
            catch (Exception error)
            {
                Console.WriteLine("\nError: " + error.ToString());
            }
            finally
            {
                if (clientSocket != null)
                {
                    clientSocket.Close();
                }
            }
        }

        private void ProcessExitingClient(string userName)
        {
            string message = userName + " has left the chat room.";

            Server.treeOfUsers.Delete(userName);
            Server.listOfUsers.Remove(userName);

            Console.WriteLine(message);
            Server.messageQueue.Enqueue(message);
            Server.ChatMessageQueue(userName);
            //Server.Broadcast(userName, message);
        }
    }
}
