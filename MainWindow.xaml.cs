using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace TrainStationServer
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private Socket socket, client;
        private IPEndPoint ipEnd;
        private Thread mainThread,recvThread,clientThread;
        byte[] recv;
        int i;
        public MainWindow()
        {
            InitializeComponent();
        }

        ~MainWindow()
        {
            //if (mainThread.IsAlive)
            socket.Close();
                mainThread.Abort();
                recvThread.Abort();
        }

        private void Start_Click_1(object sender, RoutedEventArgs e)
        {
            ipEnd = new IPEndPoint(IPAddress.Any, 15000);
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Bind(ipEnd);
            socket.Listen(20);
            mainThread = new Thread(Listening);
            mainThread.Start();
            Result.AppendText("Start listening...\n");
        }

        private void Listening()
        {
            this.Dispatcher.BeginInvoke(new Action(() =>  Result.AppendText("Wait for accepting...\n")));
            //socket.BeginAccept(new AsyncCallback(onConnectRequest), socket);
            while (true)
            {
                client = socket.Accept();
                this.Dispatcher.BeginInvoke(new Action(() => Result.AppendText("Accepted...\n")));
                clientThread = new Thread(ClientThread);
                clientThread.Start();
            }
        }

        private void ClientThread()
        {
            Socket temp;
            byte[] send = new byte[1024];
            temp = client;
            recv = new byte[1024];
            while (true)
            {
                try
                {
                    i = temp.Receive(recv);
                }
                catch (SocketException ex)
                {
                    MessageBox.Show(ex.Message);
                    return;
                }
                this.Dispatcher.BeginInvoke(new Action(() => Result.AppendText(Encoding.UTF8.GetString(recv, 0, i) + "\n")));
                send = Encoding.ASCII.GetBytes("Received...\n");
                temp.Send(send);
            }
        }

        private void onConnectRequest(IAsyncResult ar)
        {
            Socket Server = (Socket)ar.AsyncState;
            Socket Client = Server.EndAccept(ar);
            client = Client;
            this.Dispatcher.BeginInvoke(new Action(() => Result.AppendText("Accepted...\n")));
            string buffer = "Welcome.\n";
            Byte[] bufferbyte = System.Text.Encoding.ASCII.GetBytes(buffer);
            Client.Send(bufferbyte, bufferbyte.Length, 0);
            Server.BeginAccept(new AsyncCallback(onConnectRequest), Server);
            recvThread = new Thread(RecvThread);
            recvThread.Start();
            //Client.Close();
        }

        private void RecvThread()
        {
            Socket tempClient = client;
            recv = new byte[1024];
            while (true)
            {
                try
                {
                    i = tempClient.Receive(recv);
                }
                catch (SocketException ex)
                {
                    MessageBox.Show(ex.Message);
                    return;
                }
                this.Dispatcher.BeginInvoke(new Action(() => Result.AppendText(Encoding.UTF8.GetString(recv, 0, i) + "\n")));
            }
        }

        private void Analysis(byte[] buffer,int bufferlen)
        {
            int i,index = 0,index2 = 0;
            byte[] bufferline;
            byte[] length;
            bufferline = new byte[100];
            length = new byte[10];
            for (i = 0; i < bufferlen; i++)
            {
                bufferline[index++] = buffer[i];
                if (buffer[i+1] == ':')
                {
                    index = 0;
                    if (bufferline.Equals(Encoding.ASCII.GetBytes("Content-Length")))
                    {
                        i++;
                        do
                        {
                            length[index2] = buffer[i];
                            if((buffer[i + 1] == '\r') && (buffer[i + 2] == '\n'))
                                break;
                        }
                        while(true);
                    }
                }
            }
        }
    }
}
