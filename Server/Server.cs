using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ChatServer
{
    class Server
    {
        static List<TcpClient> clients = new List<TcpClient>();

        static void Main(string[] args)
        {
            TcpListener listener = new TcpListener(IPAddress.Parse("192.168.0.107"), 1234);
            listener.Start();
            Console.WriteLine("Chat server started.");

            while (true)
            {
                TcpClient client = listener.AcceptTcpClient();
                Console.WriteLine("Client connected.");

                clients.Add(client);

                Thread thread = new Thread(() => HandleClient(client));
                thread.Start();
            }
        }

        static void HandleClient(TcpClient client)
        {
            NetworkStream stream = client.GetStream();

            byte[] buffer = new byte[1024];
            int bytes;

            while (true)
            {
                bytes = stream.Read(buffer, 0, buffer.Length);
                string data = Encoding.ASCII.GetString(buffer, 0, bytes);
                Console.WriteLine("Received: {0}", data);

                if (data.StartsWith("name:"))
                {
                    string name = data.Substring(5);
                    BroadcastMessage(string.Format("{0} has joined the chat.", name));
                }
                else if (data == "exit")
                {
                    BroadcastMessage("Client has disconnected.");
                    clients.Remove(client);
                    break;
                }
                else
                {
                    BroadcastMessage(data);
                }
            }

            client.Close();
        }

        static void BroadcastMessage(string message)
        {
            byte[] msg = Encoding.ASCII.GetBytes(message);

            foreach (TcpClient client in clients)
            {
                NetworkStream stream = client.GetStream();
                stream.Write(msg, 0, msg.Length);
            }
        }
    }
}
