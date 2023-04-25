using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ChatTCP
{
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
            //string Name = NameText.Text;
            //int Ip = IPText.Text;
            //string Port = PortText.Text;

        }

        //MainWindow client = new MainWindow();

        private TcpClient client;
        private NetworkStream stream;
        private string name;



        private void Button_Connect(object sender, RoutedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                Connect(IPText.Text.ToString(), int.Parse(PortText.Text), NameText.Text.ToString());
            });
        }

        private void Button_Exit(object sender, RoutedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                Disconnect(this.client, this.stream);
            });
        }
        private void Button_SendMessage(object sender, RoutedEventArgs e)
        {
            string message = MessageInput.Text;
            SendMessage($"{name}: {message}", stream);
            MessageInput.Clear();
            MessageInput.Focus();
        }

        public void Connect(string Ip, int Port, string Name)
        {
            client = new TcpClient(Ip, Port);
            stream = this.client.GetStream();
            name = Name;

            byte[] data = Encoding.ASCII.GetBytes("name:" + name);
            stream.Write(data, 0, data.Length);

            //Thread receiveThread = new Thread(new ThreadStart(ReceiveMessages));
            Thread receiveThread = new Thread(new ThreadStart(() => ReceiveMessages(client, stream, name)));
            receiveThread.Start();
        }

        public void Disconnect(TcpClient client, NetworkStream stream)
        {
            byte[] data = Encoding.ASCII.GetBytes("exit");
            this.stream.Write(data, 0, data.Length);
            client.Close();
        }

        private void ReceiveMessages(TcpClient client, NetworkStream stream, string name)
        {
            byte[] data = new byte[1024];

            while (true)
            {
                while (true)
                {
                    byte[] _data = new byte[256];
                    StringBuilder message = new StringBuilder();
                    int bytes = 0;
                        do
                        {
                            bytes = stream.Read(_data, 0, _data.Length);
                            message.Append(Encoding.ASCII.GetString(_data, 0, bytes));
                        }
                        while (stream.DataAvailable);
                    

                    if (!message.ToString().StartsWith(name + ":"))
                    {
                        Dispatcher.Invoke(() =>
                        {
                            MessageText.Text += message.ToString() + "\n";
                        });
                    }
                }
            }
        }
        
        public void SendMessage(string message, NetworkStream stream)
        {
            byte[] data = Encoding.ASCII.GetBytes(message);
            stream.Write(data, 0, data.Length);
            MessageText.Text += message + "\r\n";
        }
    }
}
