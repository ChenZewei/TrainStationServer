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

        private void ClientThread()//多线程法
        {
            Socket temp;
            XmlDocument doc,sendxml;
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
                doc = XmlExtract(recv, i);
                //doc.Save("D://test.xml");
                sendxml = FuncDistribution(doc);
                send = Encoding.ASCII.GetBytes("Received...\r\n");
                temp.Send(send);
                send = Encoding.ASCII.GetBytes(sendxml.OuterXml);
                temp.Send(send);
            }
            //temp.Close();
        }

        private void onConnectRequest(IAsyncResult ar)//异步调用法
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
                XmlExtract(recv, i);
            }
        }

        private XmlDocument XmlExtract(byte[] buffer,int bufferlen)//Xml提取
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

        private XmlDocument FuncDistribution(XmlDocument doc)
        {
            InterfaceB B = new InterfaceB();
            XmlElement root;
            XmlNodeList nodeList;
            XmlNode node;
            XmlDocument response = new XmlDocument() ;
            root = doc.DocumentElement;
            nodeList = root.SelectNodes("/request/@command");
            node = nodeList.Item(0);
            switch(node.InnerText)
            {
                case "ResReport":
                    this.Dispatcher.BeginInvoke(new Action(() => Result.AppendText("ResReport\n")));
                    break;
                case "ResChange":
                    this.Dispatcher.BeginInvoke(new Action(() => Result.AppendText("ResChange\n")));
                    break;
                case "QueryHistoryFiles":
                    this.Dispatcher.BeginInvoke(new Action(() => Result.AppendText("QueryHistoryFiles\n")));
                    break;
                case "MURegister":
                    this.Dispatcher.BeginInvoke(new Action(() => Result.AppendText("MURegister\n")));
                    break;
                case "MUKeepAlive":
                    this.Dispatcher.BeginInvoke(new Action(() => Result.AppendText("MUKeepAlive\n")));
                    break;
                case "StartMediaReq":
                    this.Dispatcher.BeginInvoke(new Action(() => Result.AppendText("StartMediaReq\n")));
                    break;

                case "INFO"://DDU->DDU
                    this.Dispatcher.BeginInvoke(new Action(() => Result.AppendText("INFO\n")));
                    break;

                case "StopMediaReq":
                    this.Dispatcher.BeginInvoke(new Action(() => Result.AppendText("StopMediaReq\n")));
                    break;
                case "StartPlayBack":
                    this.Dispatcher.BeginInvoke(new Action(() => Result.AppendText("StartPlayBack\n")));
                    break;

                case "HisInfo"://DDU->DDU
                    this.Dispatcher.BeginInvoke(new Action(() => Result.AppendText("HisInfo\n")));
                    B.HisInfo(doc);
                    break;

                case "ControlFileBack":
                    this.Dispatcher.BeginInvoke(new Action(() => Result.AppendText("ControlFileBack\n")));
                    response = B.ControlFileBack(doc);
                    break;
                case "StartHisLoad":
                    this.Dispatcher.BeginInvoke(new Action(() => Result.AppendText("StartHisLoad\n")));
                    break;

                case "HisLoadInfo"://DDU->DDU
                    this.Dispatcher.BeginInvoke(new Action(() => Result.AppendText("HisLoadInfo\n")));
                    B.HisLoadInfo(doc);
                    break;

                case "ReportCamResState":
                    this.Dispatcher.BeginInvoke(new Action(() => Result.AppendText("ReportCamResState\n")));
                    break;
                case "ReqCamResState":
                    this.Dispatcher.BeginInvoke(new Action(() => Result.AppendText("ReqCamResState\n")));
                    break;
                case "UserResReport":
                    this.Dispatcher.BeginInvoke(new Action(() => Result.AppendText("UserResReport\n")));
                    break;
                case "GetUserCurState":
                    this.Dispatcher.BeginInvoke(new Action(() => Result.AppendText("GetUserCurState\n")));
                    B.GetUserCurState(doc);
                    break;
                case "UserResChange":
                    this.Dispatcher.BeginInvoke(new Action(() => Result.AppendText("UserResChange\n")));
                    break;
                case "SetUserCamManage":
                    this.Dispatcher.BeginInvoke(new Action(() => Result.AppendText("SetUserCamManage\n")));
                    break;
                case "AlarmResListReport":
                    this.Dispatcher.BeginInvoke(new Action(() => Result.AppendText("AlarmResListReport\n")));
                    break;
                case "AlarmResListChange":
                    this.Dispatcher.BeginInvoke(new Action(() => Result.AppendText("AlarmResListChange\n")));
                    break;
                case "AlarmResSubscribe":
                    this.Dispatcher.BeginInvoke(new Action(() => Result.AppendText("AlarmResSubscribe\n")));
                    response = B.AlarmResSubscribe(doc);
                    break;
                case "ReportAlarmRes":
                    this.Dispatcher.BeginInvoke(new Action(() => Result.AppendText("ReportAlarmRes\n")));
                    break;
                case "QueryAlarmRes":
                    this.Dispatcher.BeginInvoke(new Action(() => Result.AppendText("QueryAlarmRes\n")));
                    break;
                case "ReportAlarmInfo":
                    this.Dispatcher.BeginInvoke(new Action(() => Result.AppendText("ReportAlarmInfo\n")));
                    break;
                case "ControlPTZ":
                    this.Dispatcher.BeginInvoke(new Action(() => Result.AppendText("ControlPTZ\n")));
                    response = B.ControlPTZ(doc);
                    break;
                case "ResTransOrder":
                    this.Dispatcher.BeginInvoke(new Action(() => Result.AppendText("ResTransOrder\n")));
                    break;
                case "ResChangeOrder":
                    this.Dispatcher.BeginInvoke(new Action(() => Result.AppendText("ResChangeOrder\n")));
                    break;
                default:
                    response = new XmlDocument();
                    break;
            }
            return response;
        }

        private int IndexOf(byte[] srcBytes, byte[] searchBytes)//搜索byte数组，返回-1即未找到，返回值不为-1则为搜索字串后一个字节的序号
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
