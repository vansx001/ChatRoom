using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ChatClient
{
    class Client
    {
        TcpClient clientSocket = new TcpClient();
        NetworkStream serverStream = default(NetworkStream);
        private int port;
        private int bufferSize;
        string data;

        public Client()
        {
            port = 13000;
            bufferSize = 1024;
            
        }
        public void RunChat()
        {
            try
            {
                clientSocket.Connect("localhost", port);
                while (true)
                {
                    Thread receiveMessage = new Thread(ReceiveMessage);
                    receiveMessage.Start();


                    serverStream = clientSocket.GetStream();
                    string message = Console.ReadLine();

                    byte[] outMessage = Encoding.ASCII.GetBytes(message);
                    serverStream.Write(outMessage, 0, outMessage.Length);
                    serverStream.Flush();

                    if (message.ToLower() == "exit")
                    {
                        Environment.Exit(1);
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine("\nError: " + exception);
                Console.ReadLine();
            }
            finally
            {
                if (clientSocket != null)
                {
                    clientSocket.Close();
                }

                if (serverStream != null)
                {
                    serverStream.Close();
                }
            }
        }

        private void ReceiveMessage()
        {
            try
            {
                while (true)
                {
                    byte[] bytesReceived = new byte[bufferSize];
                    serverStream.Read(bytesReceived, 0, bytesReceived.Length);
                    string message = Encoding.ASCII.GetString(bytesReceived);
                    message = message.Substring(0, message.IndexOf("\0"));
                    Console.WriteLine(message);
                }
            }
            catch (Exception exception)
            {
                Console.Write("\nError: " + exception);
            }
        }

    }
}
