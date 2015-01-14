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
        private Thread mainThread;
        public MainWindow()
        {
            InitializeComponent();
        }

        ~MainWindow()
        {
            //if (mainThread.IsAlive)
                mainThread.Abort();
        }

        private void Start_Click_1(object sender, RoutedEventArgs e)
        {
            ipEnd = new IPEndPoint(IPAddress.Any, 8888);
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Bind(ipEnd);
            socket.Listen(20);
            mainThread = new Thread(Listening);
            mainThread.Start();
            Result.AppendText("Start listening...\n");
        }

        private void Listening()
        {
            Thread clientThread;
            while (true)
            {
                this.Dispatcher.BeginInvoke(new Action(() =>  Result.AppendText("Wait for accepting...\n")));
                client = socket.Accept();
                clientThread = new Thread(Proc);
                this.Dispatcher.BeginInvoke(new Action(() => Result.AppendText("Accepted!\n")));
            }
        }

        private void Proc()
        {

        }

        private void onConnectRequest(IAsyncResult ar)
        {
            Socket Server = (Socket)ar.AsyncState;
            Socket Client = Server.EndAccept(ar);
        }

        private void Analysis()
        {

        }
    }
}
