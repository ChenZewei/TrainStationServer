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
using System.IO;

namespace TrainStationServer
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        class stateobject
        {
            public Socket socket;
            public int BufferSize = 2048;
            public byte[] recv;
            public byte[] send;
            public int recvLen;
            public bool isClosed = false;
            public StringBuilder sb = new StringBuilder();
        }
        private Socket socket, client, testSever, testClient, test;
        private Socket server;
        private IPEndPoint ipEnd, testIpEnd;
        private Thread mainThread, recvThread, clientThread, testThread, testClientThread;
        //byte[] recv;
        int i;
        DataBase Database;
        InterfaceC C;
        public MainWindow()
        {
            InitializeComponent();
        }

        ~MainWindow()
        {
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
            if(clientThread != null)
            {
                clientThread.Abort();
                clientThread.Join();
            }
            if (testThread != null)
            {
                testThread.Abort();
                testThread.Join();
            }
            if (testClientThread != null)
            {
                testClientThread.Abort();
                testClientThread.Join();
            }
        }

        private void Start_Click_1(object sender, RoutedEventArgs e)
        {
            ipEnd = new IPEndPoint(IPAddress.Any, 15000);
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Bind(ipEnd);
            socket.Listen(20);
            Database = new DataBase();
            C = new InterfaceC(Database);
            mainThread = new Thread(Listening);
            mainThread.IsBackground = true;
            mainThread.Start();
            Result.AppendText("Start listening...\r\n");
        }

        private void Listening()
        {
            this.Dispatcher.BeginInvoke(new Action(() => Result.AppendText("Wait for accepting...\r\n")));
            while (true)
            {
                client = socket.Accept();
                this.Dispatcher.BeginInvoke(new Action(() => Result.AppendText("Accepted...\r\n")));
                clientThread = new Thread(ClientThread);
                clientThread.IsBackground = true;
                clientThread.Start();
            }
        }

        void recvProc(IAsyncResult ar)
        {
            stateobject state = (stateobject)ar.AsyncState;
            if (state.isClosed)
                return;
            try
            {
                int i = state.socket.EndReceive(ar);
                this.Dispatcher.BeginInvoke(new Action(() => Result.AppendText(Encoding.GetEncoding("GB2312").GetString(state.recv, 0, i))));
                string[] result;
                if (InterfaceC.IsRequest(state.recv, i))
                {
                    state.send = InterfaceC.Request(state.recv, i);
                    state.socket.Send(state.send);
                }
                else
                {
                    result = InterfaceC.Response(state.recv, i);
                    if (result != null)
                        for (int k = 0; k < result.Length; k++)
                            Console.WriteLine(result[k]);
                }
            }
            catch(SocketException e)
            {
                state.isClosed = true;
                Console.WriteLine(e.Message);
                return;
            }
            catch(XmlException e)
            {
                Console.WriteLine(e.Message);
                return;
            }            
        }

        private void ClientThread()//多线程法
        {
            Socket temp;
            temp = client;
            byte[] send = new byte[2048];
            byte[] recv = new byte[2048];
            string[] result = new string[10];
            stateobject so = new stateobject();
            so.socket = temp;
            so.recv = recv;
            so.send = send;
            while(true)
            {
                
                try
                {
                    //i = temp.Receive(recv);
                    temp.BeginReceive(so.recv, 0, so.BufferSize, 0, new AsyncCallback(recvProc), so);
                    if (so.isClosed)
                        return;
                }
                catch (SocketException ex)
                {
                    MessageBox.Show(ex.Message);
                    return;
                }
                
                Thread.Sleep(10);
                //FileStream sendbuf = new FileStream("D://Response.txt", FileMode.OpenOrCreate, FileAccess.Write);
                //sendbuf.Close();
                //sendbuf = new FileStream("D://Response.txt", FileMode.Append, FileAccess.Write);
                //sendbuf.Write(recv, 0, recv.Length);
                //sendbuf.Close();

                
   
            }
        }

        private void Test_Click_1(object sender, RoutedEventArgs e)
        {
            test.Send(InterfaceC.StartMediaReq("127.0.0.1", "12000", "6100011201000102", "6100011201000102", "1", "1", "0", "", "", "1"));
        }
    }
}
