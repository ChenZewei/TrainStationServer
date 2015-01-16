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
using System.Xml;

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
            if(socket != null)
                socket.Close();
            if (mainThread != null)
            {
                mainThread.Abort();
                mainThread.Join();
            }
            if (recvThread != null)
            {
                recvThread.Abort();
                recvThread.Join();
            }
        }

        private void Start_Click_1(object sender, RoutedEventArgs e)
        {
            ipEnd = new IPEndPoint(IPAddress.Any, 15000);
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Bind(ipEnd);
            socket.Listen(20);
            mainThread = new Thread(Listening);
            mainThread.IsBackground = true;
            mainThread.Start();
            Result.AppendText("Start listening...\r\n");
        }

        private void Listening()
        {
            this.Dispatcher.BeginInvoke(new Action(() => Result.AppendText("Wait for accepting...\r\n")));
            //socket.BeginAccept(new AsyncCallback(onConnectRequest), socket);
            while (true)
            {
                client = socket.Accept();
                this.Dispatcher.BeginInvoke(new Action(() => Result.AppendText("Accepted...\r\n")));
                clientThread = new Thread(ClientThread);
                clientThread.IsBackground = true;
                clientThread.Start();
            }
        }

        private void ClientThread()
        {
            Socket temp;
            XmlDocument doc;
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
                this.Dispatcher.BeginInvoke(new Action(() => Result.AppendText(Encoding.UTF8.GetString(recv, 0, i))));
                doc = Analysis(recv, i);
                doc.Save("D://test.xml");
                send = Encoding.ASCII.GetBytes("Received...\r\n");
                temp.Send(send);
            }
        }

        private void onConnectRequest(IAsyncResult ar)
        {
            Socket Server = (Socket)ar.AsyncState;
            Socket Client = Server.EndAccept(ar);
            client = Client;
            this.Dispatcher.BeginInvoke(new Action(() => Result.AppendText("Accepted...\r\n")));
            string buffer = "Welcome.\r\n";
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
                Analysis(recv, i);
            }
        }

        private XmlDocument Analysis(byte[] buffer,int bufferlen)
        {
            XmlDocument xmlDoc;
            int i = 0,index = 0;
            byte[] bufferline;
            byte[] length;
            string strBuffer;
            bufferline = new byte[100];
            length = new byte[10];
            if ((index = IndexOf(buffer, Encoding.ASCII.GetBytes("Content-Length"))) != -1)
            {
                index++;
                while ((index + i)<bufferlen)
                {
                    length[i] = buffer[index + i];
                    i++;
                    if ((buffer[index + i] == '\r') && (buffer[index + i + 1] == '\n'))
                        break;
                }
            }
            Console.Write(Encoding.UTF8.GetString(length,0,length.Length));
            if((index = IndexOf(buffer, Encoding.ASCII.GetBytes("\r\n\r\n"))) != -1)
            {
                xmlDoc = new XmlDocument();
                strBuffer = Encoding.UTF8.GetString(buffer, index, (bufferlen - index));
                xmlDoc.LoadXml(strBuffer);
                return xmlDoc;
            }
            return null;
        }

        private int IndexOf(byte[] srcBytes, byte[] searchBytes)
        {
            if (srcBytes == null) { return -1; }
            if (searchBytes == null) { return -1; }
            if (srcBytes.Length == 0) { return -1; }
            if (searchBytes.Length == 0) { return -1; }
            if (srcBytes.Length < searchBytes.Length) { return -1; }
            for (int i = 0; i < srcBytes.Length - searchBytes.Length; i++)
            {
                if (srcBytes[i] == searchBytes[0])
                {
                    if (searchBytes.Length == 1) { return i; }
                    bool flag = true;
                    for (int j = 1; j < searchBytes.Length; j++)
                    {
                        if (srcBytes[i + j] != searchBytes[j])
                        {
                            flag = false;
                            break;
                        }
                    }
                    if (flag)
                        return i+searchBytes.Length;
                }
            }
            return -1;
        }

    }
}
