﻿using System;
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
        private Socket socket, client, testSever, testClient;
        private Socket server;
        private IPEndPoint ipEnd, testIpEnd;
        private Thread mainThread, recvThread, clientThread, testThread, testClientThread;
        byte[] recv;
        int i;
        DataBase Database;
        InterfaceC C;
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
            testIpEnd = new IPEndPoint(IPAddress.Any, 10000);
            testSever = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            testClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            testSever.Bind(testIpEnd);
            testSever.Listen(20);
            Database = new DataBase();
            C = new InterfaceC(Database);
            mainThread = new Thread(Listening);
            mainThread.IsBackground = true;
            mainThread.Start();
            Result.AppendText("Start listening...\r\n");
            testThread = new Thread(testListening);
            testThread.IsBackground = true;
            testThread.Start();
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

        private void testListening()
        {
            this.Dispatcher.BeginInvoke(new Action(() => Result.AppendText("Wait for accepting...\r\n")));
            //socket.BeginAccept(new AsyncCallback(onConnectRequest), socket);
            while (true)
            {
                testClient = testSever.Accept();
                this.Dispatcher.BeginInvoke(new Action(() => Result.AppendText("Accepted...\r\n")));
                testClientThread = new Thread(TestClientThread);
                testClientThread.IsBackground = true;
                testClientThread.Start();
            }
        }

        private void ClientThread()//多线程法
        {
            Socket temp;
            //SIPTools sipTools;
            //XmlDocument doc,sendxml;

            //FileStream sendbuf = new FileStream("D://Response.txt", FileMode.OpenOrCreate, FileAccess.Write);
            //sendbuf.Close();

            byte[] send = new byte[2048];
            temp = client;
            recv = new byte[2048];
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
            //sipTools = new SIPTools(recv, i);
            //try
            //{
            //    doc = SIPTools.XmlExtract(recv, i);
            //    sendxml = FuncDistribution(doc);
            //    send = Encoding.UTF8.GetBytes(sipTools.SIPResponse(sendxml));        
            //}
            //catch(XmlException e)
            //{ 
            //    Console.WriteLine(e.Message); 
            //}

            send = InterfaceC.Response(recv, i);

            //sendbuf = new FileStream("D://Response.txt", FileMode.Append, FileAccess.Write);
            //sendbuf.Write(send, 0, send.Length);
            //sendbuf.Close();

            try
            {
                temp.Send(send);
                temp.Close();
            }
            catch(SocketException ex)
            {
                Console.Write(ex.Message);
            }
        }

        private void TestClientThread()//多线程法
        {
            Socket temp;

            byte[] send = new byte[2048];
            temp = testClient;
            recv = new byte[2048];
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

            send = InterfaceC.test(recv, i);

            try
            {
                temp.Send(send);
                temp.Close();
            }
            catch (SocketException ex)
            {
                Console.Write(ex.Message);
            }
        }

        /*
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
        }

        private void RecvThread()
        {
            Socket tempClient = client;
            SIPTools sipTools;
            XmlDocument doc, sendxml;
            recv = new byte[1024];
            byte[] send = new byte[2048];
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
            sipTools = new SIPTools(recv, i);
            doc = SIPTools.XmlExtract(recv, i);
            sendxml = FuncDistribution(doc);
            send = Encoding.ASCII.GetBytes(sipTools.SIPResponse(sendxml));

            try
            {
                tempClient.Send(send);
            }
            catch (SocketException ex)
            {
                Console.Write(ex.Message);
            }
            tempClient.Close();
        }
        */
        private XmlDocument FuncDistribution(XmlDocument doc)
        {
            InterfaceB B = new InterfaceB();
            InterfaceC C = new InterfaceC(Database);
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
                    response = InterfaceC.ResReport(doc);
                    break;
                case "ResChange":
                    this.Dispatcher.BeginInvoke(new Action(() => Result.AppendText("ResChange\n")));
                    break;
                case "QueryHistoryFiles":
                    this.Dispatcher.BeginInvoke(new Action(() => Result.AppendText("QueryHistoryFiles\n")));
                    response = B.QueryHistoryFiles(doc);
                    break;
                case "MURegister":
                    this.Dispatcher.BeginInvoke(new Action(() => Result.AppendText("MURegister\n")));
                    break;
                case "MUKeepAlive":
                    this.Dispatcher.BeginInvoke(new Action(() => Result.AppendText("MUKeepAlive\n")));
                    break;
                case "StartMediaReq":
                    this.Dispatcher.BeginInvoke(new Action(() => Result.AppendText("StartMediaReq\n")));
                    response = B.StartMediaReq(doc);
                    break;

                case "INFO"://DDU->DDU
                    this.Dispatcher.BeginInvoke(new Action(() => Result.AppendText("INFO\n")));
                    response = B.INFO(doc);
                    break;

                case "StopMediaReq":
                    this.Dispatcher.BeginInvoke(new Action(() => Result.AppendText("StopMediaReq\n")));
                    response = B.StopMediaReq(doc);
                    break;
                case "StartPlayBack":
                    this.Dispatcher.BeginInvoke(new Action(() => Result.AppendText("StartPlayBack\n")));
                    response = B.StartPlayBack(doc);
                    break;

                case "HisInfo"://DDU->DDU
                    this.Dispatcher.BeginInvoke(new Action(() => Result.AppendText("HisInfo\n")));
                    response = B.HisInfo(doc);
                    break;

                case "ControlFileBack":
                    this.Dispatcher.BeginInvoke(new Action(() => Result.AppendText("ControlFileBack\n")));
                    response = B.ControlFileBack(doc);
                    break;
                case "StartHisLoad":
                    this.Dispatcher.BeginInvoke(new Action(() => Result.AppendText("StartHisLoad\n")));
                    response = B.StartHisLoad(doc);
                    break;

                case "HisLoadInfo"://DDU->DDU
                    this.Dispatcher.BeginInvoke(new Action(() => Result.AppendText("HisLoadInfo\n")));
                    response = B.HisLoadInfo(doc);
                    break;

                case "ReportCamResState":
                    this.Dispatcher.BeginInvoke(new Action(() => Result.AppendText("ReportCamResState\n")));
                    break;
                case "ReqCamResState":
                    this.Dispatcher.BeginInvoke(new Action(() => Result.AppendText("ReqCamResState\n")));
                    response = B.ReqCamResState(doc);
                    break;
                case "UserResReport":
                    this.Dispatcher.BeginInvoke(new Action(() => Result.AppendText("UserResReport\n")));
                    break;
                case "GetUserCurState":
                    this.Dispatcher.BeginInvoke(new Action(() => Result.AppendText("GetUserCurState\n")));
                    response = B.GetUserCurState(doc);
                    break;
                case "UserResChange":
                    this.Dispatcher.BeginInvoke(new Action(() => Result.AppendText("UserResChange\n")));
                    break;
                case "SetUserCamManage":
                    this.Dispatcher.BeginInvoke(new Action(() => Result.AppendText("SetUserCamManage\n")));
                    response = B.SetUserCamManage(doc);
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
                    response = B.QueryAlarmRes(doc);
                    break;
                case "ReportAlarmInfo":
                    this.Dispatcher.BeginInvoke(new Action(() => Result.AppendText("ReportAlarmInfo\n")));
                    response = B.ReportAlarmInfo(doc);
                    break;
                case "ControlPTZ":
                    this.Dispatcher.BeginInvoke(new Action(() => Result.AppendText("ControlPTZ\n")));
                    response = B.ControlPTZ(doc);
                    break;
                case "ResTransOrder":
                    this.Dispatcher.BeginInvoke(new Action(() => Result.AppendText("ResTransOrder\n")));
                    response = B.ResTransOrder(doc);
                    break;
                case "ResChangeOrder":
                    this.Dispatcher.BeginInvoke(new Action(() => Result.AppendText("ResChangeOrder\n")));
                    response = B.ResChangeOrder(doc);
                    break;
                default:
                    response = new XmlDocument();
                    break;
            }
            return response;
        }

        private void Test_Click_1(object sender, RoutedEventArgs e)
        {
            ipEnd = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 10000);
            server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            Thread testThread;
            try
            {
                server.Connect(ipEnd);
            }
            catch (SocketException ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
            server.Send(InterfaceC.StartMediaReq("127.0.0.1", "12000", "6100011201000102", "6100011201000102", "1", "1", "0", "", "", "1"));
            testThread = new Thread(TestThread);
            testThread.IsBackground = true;
            testThread.Start();
        }

        private void TestThread()
        {
            byte[] recv;
            string[] result = new string[3];
            SIPTools sipTools;
            XmlDocument doc = new XmlDocument();
            FileStream sendbuf = new FileStream("D://Response.txt", FileMode.OpenOrCreate, FileAccess.Write);
            sendbuf.Close();
            recv = new byte[2048];
            try
            {
                i = server.Receive(recv);
            }
            catch (SocketException ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
            sipTools = new SIPTools(recv, i);
            try
            {
                doc = SIPTools.XmlExtract(recv, i);
            }
            catch (XmlException e)
            {
                Console.WriteLine(e.Message);
            }
            sendbuf = new FileStream("D://test.txt", FileMode.Append, FileAccess.Write);
            sendbuf.Write(Encoding.UTF8.GetBytes(doc.OuterXml), 0, Encoding.UTF8.GetBytes(doc.OuterXml).Length);
            sendbuf.Close();
            result = InterfaceC.StartMediaResponse(doc);
            Console.WriteLine("sessionId:" + result[0] + ";tcpIp:" + result[1] + ";tcpPort: " + result[2]);
            this.Dispatcher.BeginInvoke(new Action(() => Result.AppendText(Encoding.UTF8.GetString(recv, 0, i))));
        }
        /*
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
                        return i + searchBytes.Length;
                }
            }
            return -1;
        }
        */
    }
}
