using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ChatClient
{
    class Client
    {
        private TcpClient client;
        private NetworkStream stream;
        private string name;

        static void Main(string[] args)
        {
            Console.Write("Enter your name: ");
            string name = Console.ReadLine();

            Console.Write("Enter the IP address of the chat server: ");
            string ipAddress = Console.ReadLine();

            Console.Write("Enter the port number of the chat server: ");
            int port = int.Parse(Console.ReadLine());

            Client client = new Client();
            client.Connect(ipAddress, port, name);

            Console.WriteLine("Connected to chat server. Type 'exit' to disconnect.");

            string message = Console.ReadLine();
            while (message.ToLower() != "exit")
            {
                client.SendMessage($"{name}: {message}");
                message = Console.ReadLine();
            }

            client.Disconnect();
        }

        public void Connect(string ipAddress, int port, string name)
        {
            this.client = new TcpClient(ipAddress, port);
            this.stream = this.client.GetStream();
            this.name = name;

            byte[] data = Encoding.ASCII.GetBytes("name:" + name);
            this.stream.Write(data, 0, data.Length);

            Thread receiveThread = new Thread(new ThreadStart(ReceiveMessages));
            receiveThread.Start();
        }

        public void Disconnect()
        {
            byte[] data = Encoding.ASCII.GetBytes("exit");
            this.stream.Write(data, 0, data.Length);

            this.client.Close();
        }

        public void SendMessage(string message)
        {
            byte[] data = Encoding.ASCII.GetBytes(message);
            this.stream.Write(data, 0, data.Length);
        }

        private void ReceiveMessages()
        {
            byte[] data = new byte[1024];

            while (true)
            {
                /*int bytes = this.stream.Read(data, 0, data.Length);
                string message = Encoding.ASCII.GetString(data, 0, bytes);
                Console.WriteLine(message);*/
                while (true)
                {
                    // Read the incoming message from the network stream
                    byte[] _data = new byte[256];
                    StringBuilder message = new StringBuilder();
                    int bytes = 0;
                    do
                    {
                        bytes = stream.Read(_data, 0, _data.Length);
                        message.Append(Encoding.ASCII.GetString(_data, 0, bytes));
                    }
                    while (stream.DataAvailable);

                    // Display the incoming message on the console if it's not the client's own message
                    if (!message.ToString().StartsWith(name + ":"))
                    {
                        Console.WriteLine(message);
                    }
                }
            }
        }
    }
}