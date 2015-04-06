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
using System.Runtime.InteropServices;

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
        }
        private Socket socket, client, testsocket;
        //private SipSocket mainSocket, client, testsocket;
        private IPEndPoint ipEnd;
        private eXosip exosip;
        private Thread clientThread, snoopThread;
        private byte[] recv = new byte[2048], send = new byte[2048];
        private DataBase Database;
        private InterfaceC C;
        //private SocketBound bound;
        private bool timeout = false;
        public MainWindow()
        {
            InitializeComponent();
        }

        ~MainWindow()
        {
            if(socket != null)
            {
                socket.Close();
                socket.Dispose();
            }
            if(clientThread != null)
            {
                clientThread.Abort();
                clientThread.Join();
            }
        }

        private void Start_Click_1(object sender, RoutedEventArgs e)//绑定套接字等初始化
        {
            ipEnd = new IPEndPoint(IPAddress.Any, 15000);
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Bind(ipEnd);
            socket.Listen(20);
            Database = new DataBase();
            C = new InterfaceC(Database);
            stateobject mainObject = new stateobject();
            mainObject.socket = socket;
            socket.BeginAccept(new AsyncCallback(AsyncAccept), mainObject);
            Result.AppendText("Start listening...\r\n");
            exosip = new eXosip();
            snoopThread = new Thread(Snoop);
            snoopThread.IsBackground = true;
            snoopThread.Start();
            Combo.IsEnabled = true;
        }
        
        private void AsyncAccept(IAsyncResult ar)//异步Accept
        {
            stateobject mainObject = (stateobject)ar.AsyncState;
            stateobject clientObject = new stateobject();
            client = mainObject.socket.EndAccept(ar);
            mainObject.socket.BeginAccept(new AsyncCallback(AsyncAccept), mainObject);
            this.Dispatcher.BeginInvoke(new Action(() => Result.AppendText("Accepted...\r\n")));
            clientObject.socket = client;
            clientObject.recv = recv;
            clientObject.send = send;
            client.BeginReceive(clientObject.recv, 0, clientObject.BufferSize, 0, new AsyncCallback(recvProc), clientObject);
            testsocket = client;
        }

        void recvProc(IAsyncResult ar)//异步Receive
        {
            stateobject state = (stateobject)ar.AsyncState;
            XmlDocument Doc = new XmlDocument();
            SipSocket temp;
            if (state.isClosed)
                return;
            try
            {
                state.socket.BeginReceive(state.recv, 0, state.BufferSize, 0, new AsyncCallback(recvProc), state);
                int i = state.socket.EndReceive(ar);
                this.Dispatcher.BeginInvoke(new Action(() => Result.AppendText(Encoding.GetEncoding("GB2312").GetString(state.recv, 0, i))));
                SipSocket.Add(state.socket, new SIPTools(state.recv, i));
                temp = SipSocket.FindSipSocket(state.socket);
                string[] result;
                Doc = SIPTools.XmlExtract(recv, i);
                if (Doc == null)
                    return;
                if (temp == null)
                    return;
                if (InterfaceC.IsRequest(Doc))
                {
                    temp.SendResponse(InterfaceC.Request(Doc));
                }
                else
                {
                    result = InterfaceC.Response(Doc);
                    if (result != null)
                    {
                        SipSocket.InsertResult(state.socket, result);
                        for (int k = 0; k < result.Length; k++)
                            Console.WriteLine(result[k]);
                    }
                        
                }
            }
            catch(SocketException e)
            {
                state.isClosed = true;
                state.socket.Dispose();
                Console.WriteLine(e.Message);
                return;
            }
            catch(XmlException e)
            {
                Console.WriteLine(e.Message);
                return;
            }      
      
        }

        void Snoop()
        {
            eXosip.Init();
            eXosip.ListenAddr(ProtocolType.Udp, null, 5060, AddressFamily.InterNetwork, 0);
            while (true)
            {
                IntPtr je = eXosip.Event.Wait(0, 1);
                if (je == IntPtr.Zero) continue;
                eXosip.Lock();
                eXosip.AutomaticAction();
                eXosip.Unlock();
                eXosip.Event eXosipEvent = (eXosip.Event)Marshal.PtrToStructure(je, typeof(eXosip.Event));
                Console.WriteLine(eXosipEvent.textinfo);
                switch (eXosipEvent.type)
                {
                    case eXosip.EventType.EXOSIP_CALL_INVITE:
                        osipCallInvite(eXosipEvent);
                        break;
                    case eXosip.EventType.EXOSIP_CALL_MESSAGE_NEW:
                        osipCallMessage(eXosipEvent);
                        break;
                    case eXosip.EventType.EXOSIP_MESSAGE_NEW:
                        osipMessage(eXosipEvent);
                        break;
                    case eXosip.EventType.EXOSIP_CALL_CLOSED:
                        osipCallClose(eXosipEvent);
                        break;
                    default:
                        break;
                }
                eXosip.Event.Free(je);
                Thread.Sleep(100);
            }
            eXosip.Quit();

        }

        private void osipCallInvite(eXosip.Event eXosipEvent)
        {
            Socket exoSocket;
            eXosip.Lock();
            byte[] recv = new byte[2048];
            XmlDocument Request = new XmlDocument(); ;
            SipSocket temp;
            System.Timers.Timer timer = new System.Timers.Timer(2000);
            timer.Elapsed += new System.Timers.ElapsedEventHandler(Tick);
            string[] result = new string[10];
            eXosip.Call.SendAnswer(eXosipEvent.tid, 180, IntPtr.Zero);
            IntPtr sdp = eXosip.GetRemoteSdp(eXosipEvent.did);
            if (sdp == IntPtr.Zero)
            {
                eXosip.Call.SendAnswer(eXosipEvent.tid, 400, IntPtr.Zero);
                eXosip.Unlock();
                return;
            }

            osip.From pTo = osip.Message.GetTo(eXosipEvent.request);
            osip.From pFrom = osip.Message.GetFrom(eXosipEvent.request);
            osip.URI uriTo = (osip.URI)Marshal.PtrToStructure(osip.From.GetURL(pTo.url), typeof(osip.URI));
            osip.URI uriFrom = (osip.URI)Marshal.PtrToStructure(osip.From.GetURL(pFrom.url), typeof(osip.URI));
            string name = osip.URI.ToString(pTo.url);
            string name2 = osip.URI.ToString(pFrom.url);
            string resId = name.Substring(4, name.IndexOf('@') - 4);
            string userCode = name2.Substring(4, name2.IndexOf('@') - 4);
            string userId = resId.Substring(0, 10) + userCode;
            if ((exoSocket = SipSocket.FindSocket(resId.Substring(0, 6))) == null)
            {
                eXosip.Call.SendAnswer(eXosipEvent.tid, 404, IntPtr.Zero);
                eXosip.Unlock();
                return;
            }

            string sessionname = osip.SdpMessage.GetSessionName(sdp);
            //以下的接口中的数据均为伪造，未测试版本，缺失提取Xml信息的步骤
            if (sessionname == "RealTime")
            {
                Request = InterfaceC.StartMediaReq(resId, userId, "63", "1", "0", "", "", "1");
            }
            else if (sessionname == "PlayBack")
            {
                Request = InterfaceC.StartPlayBack(resId, userId, "63", "2015-03-22 22:33:22", "2015-03-22 23:44:22", 0, "192.168.1.1", "15000", 1, 0);
            }
            else if (sessionname == "DownLoad")
            {
                Request = InterfaceC.StartHisLoad(resId, userId, "63", "2015-03-22 22:33:22", "2015-03-22 23:44:22", 0, "192.168.1.1", "15000", 1, 0);//测试
            }
            else if (sessionname == "PlayBack")
            {
                Request = InterfaceC.StartPlayBack(resId, userId, "63", "2015-03-22 22:33:22", "2015-03-22 23:44:22", 0, "192.168.1.1", "15000", 1, 0);
            }
            else if (sessionname == "DownLoad")
            {
                Request = InterfaceC.StartHisLoad(resId, userId, "63", "2015-03-22 22:33:22", "2015-03-22 23:44:22", 0, "192.168.1.1", "15000", 1, 0);//测试
            }

            temp = SipSocket.FindSipSocket(exoSocket);
            SipSocket.CleanResult(exoSocket);
            temp.SendRequest(Request);
            result = WaitForResult(testsocket, timer, 2000);

            if (result != null)
                for (int k = 0; k < result.Length; k++)
                    Console.WriteLine(result[k]);
            else
            {
                eXosip.Call.SendAnswer(eXosipEvent.tid, 404, IntPtr.Zero);
                eXosip.Unlock();
                return;
            }
            string sessionId = osip.SdpMessage.GetSessionId(sdp);
            string sessionVersion = osip.SdpMessage.GetSessionVersion(sdp);
            IntPtr answer = eXosip.Call.BuildAnswer(eXosipEvent.tid, 200);
            if (answer != IntPtr.Zero)
            {
                string tmp = string.Format(
                    "v=0\r\n" +
                    "o={0} {1} {2} IN IP4 {3}\r\n" +
                    "s={5}\r\n" +
                    "c=IN IP4 {3}\r\n" +
                    "t=0 0\r\n" +
                    "a=sendonly\r\n" +
                    "m=application {4} RTP/AVP/TCP octet-stream\r\n" +
                    "a=setup:passive\r\n" +
                    "a=connection:new\r\n",
                    resId,
                    result[0],
                    sessionVersion,
                    result[1],
                    result[2],
                    sessionname);
                osip.Message.SetBody(answer, tmp);
                osip.Message.SetContentType(answer, "application/sdp");
                eXosip.Call.SendAnswer(eXosipEvent.tid, 200, answer);
                eXosip.Unlock();
            }
        }

        void osipCallMessage(eXosip.Event eXosipEvent)
        {
            IntPtr ptr;
            XmlDocument TempDoc = new XmlDocument();
            XmlDocument Request;
            Socket exoSocket;
            SipSocket temp;
            string[] result = new string[10];
            System.Timers.Timer timer = new System.Timers.Timer(2000);
            ptr = osip.Message.GetContentType(eXosipEvent.request);
            if (ptr == IntPtr.Zero) return;
            osip.ContentType content = (osip.ContentType)Marshal.PtrToStructure(ptr, typeof(osip.ContentType));
            ptr = osip.Message.GetBody(eXosipEvent.request);
            if (ptr == IntPtr.Zero) return;

            osip.From pTo = osip.Message.GetTo(eXosipEvent.request);
            osip.From pFrom = osip.Message.GetFrom(eXosipEvent.request);
            osip.URI uriTo = (osip.URI)Marshal.PtrToStructure(osip.From.GetURL(pTo.url), typeof(osip.URI));
            osip.URI uriFrom = (osip.URI)Marshal.PtrToStructure(osip.From.GetURL(pFrom.url), typeof(osip.URI));
            string name = osip.URI.ToString(pTo.url);
            string name2 = osip.URI.ToString(pFrom.url);
            string resId = name.Substring(4, name.IndexOf('@') - 4);
            string userCode = name2.Substring(4, name2.IndexOf('@') - 4);
            string userId = resId.Substring(0, 10) + userCode;

            if ((exoSocket = SipSocket.FindSocket(resId.Substring(0, 6))) == null)
            {
                eXosip.Call.SendAnswer(eXosipEvent.tid, 404, IntPtr.Zero);
                eXosip.Unlock();
                return;
            }

            osip.Body data = (osip.Body)Marshal.PtrToStructure(ptr, typeof(osip.Body));
            if (Marshal.PtrToStringAnsi(content.type) != "application" ||
                Marshal.PtrToStringAnsi(content.subtype) != "xml")
                return;
            string xml = Marshal.PtrToStringAnsi(data.body);
            Console.Write(xml);
            /*----------------------------分割线-----------------------------*/
            TempDoc.LoadXml(xml);
            temp = SipSocket.FindSipSocket(exoSocket);
            Request = InterfaceC.CallMessageTranslate(TempDoc, resId, userId);//提取参数并转为C类接口格式
            SipSocket.CleanResult(exoSocket);
            temp.SendRequest(Request);
            result = WaitForResult(testsocket, timer, 2000);

            if (result != null)
                for (int k = 0; k < result.Length; k++)
                    Console.WriteLine(result[k]);
            temp = SipSocket.FindSipSocket(exoSocket);
            temp.SendRequest(Request); 
        }

        void osipMessage(eXosip.Event eXosipEvent)
        {
            IntPtr ptr;
            XmlDocument TempDoc = new XmlDocument();
            XmlDocument Request;
            Socket exoSocket;
            SipSocket temp;
            ptr = osip.Message.GetContentType(eXosipEvent.request);
            if (ptr == IntPtr.Zero) return;
            osip.ContentType content = (osip.ContentType)Marshal.PtrToStructure(ptr, typeof(osip.ContentType));
            ptr = osip.Message.GetBody(eXosipEvent.request);
            if (ptr == IntPtr.Zero) return;

            osip.From pTo = osip.Message.GetTo(eXosipEvent.request);
            osip.From pFrom = osip.Message.GetFrom(eXosipEvent.request);
            osip.URI uriTo = (osip.URI)Marshal.PtrToStructure(osip.From.GetURL(pTo.url), typeof(osip.URI));
            osip.URI uriFrom = (osip.URI)Marshal.PtrToStructure(osip.From.GetURL(pFrom.url), typeof(osip.URI));
            string name = osip.URI.ToString(pTo.url);
            string name2 = osip.URI.ToString(pFrom.url);
            string resId = name.Substring(4, name.IndexOf('@') - 4);
            string userCode = name2.Substring(4, name2.IndexOf('@') - 4);
            string userId = resId.Substring(0, 10) + userCode;

            if ((exoSocket = SipSocket.FindSocket(resId.Substring(0, 6))) == null)
            {
                eXosip.Call.SendAnswer(eXosipEvent.tid, 404, IntPtr.Zero);
                eXosip.Unlock();
                return;
            }

            osip.Body data = (osip.Body)Marshal.PtrToStructure(ptr, typeof(osip.Body));
            if (Marshal.PtrToStringAnsi(content.type) != "application" ||
                Marshal.PtrToStringAnsi(content.subtype) != "xml")
                return;
            string xml = Marshal.PtrToStringAnsi(data.body);
            Console.Write(xml);
            /*----------------------------分割线-----------------------------*/
            TempDoc.LoadXml(xml);
            //SipSocket.CleanResult(exoSocket);
            //temp = SipSocket.FindSipSocket(exoSocket);
            //temp.Send(Request); 
        }

        void osipCallClose(eXosip.Event eXosipEvent)
        {
            IntPtr ptr;
            XmlDocument Request;
            Socket exoSocket;
            SipSocket temp;
            osip.From pFrom = osip.Message.GetFrom(eXosipEvent.request);
            osip.From pTo = osip.Message.GetTo(eXosipEvent.request);
            osip.URI uri = (osip.URI)Marshal.PtrToStructure(osip.From.GetURL(pTo.url), typeof(osip.URI));
            string name = osip.URI.ToString(pTo.url);
            string id = name.Substring(4, name.IndexOf('@') - 4);
            if ((exoSocket = SipSocket.FindSocket(id.Substring(0, 6))) == null)
            {
                eXosip.Call.SendAnswer(eXosipEvent.tid, 404, IntPtr.Zero);
                eXosip.Unlock();
                return;
            }
            /*----------------------------分割线-----------------------------*/
            Request = InterfaceC.StopMediaReq("0000000000000000", "6101010000000001", "0");//提取参数并转为C类接口格式

            temp = SipSocket.FindSipSocket(exoSocket);
            try
            {
                temp.SendRequest(Request);
            }
            catch (SocketException ex)
            {
                System.Console.WriteLine(ex);
            }

            SipSocket.CleanResult(exoSocket);
        }

        private void Test_Click_1(object sender, RoutedEventArgs e)//测试用
        {
            byte[] recv = new byte[2048];
            string[] result = new string[10];
            int sendLen = 0;
            bool test = false;
            SipSocket temp = SipSocket.FindSipSocket(testsocket);
            if (temp == null)
                return;
            System.Timers.Timer timer = new System.Timers.Timer(2000);
            SipSocket.CleanResult(testsocket);
            string[] resId = { "6101010000000001", "6101010000000002" }, name = { "01", "02" };
            string[] cameId = { "00000", "111111" }, id = { "222222", "333333" };
            string[] str = { "6100001201000101"};
            string[] type = { "666666", "4444444" };
            string[] startTime = { "2015-03-22 12:22:33", "2015-03-22 12:22:33" }, endTime = { "2015-03-22 12:42:33", "2015-03-22 12:42:33" };
            switch (Combo.SelectionBoxItem.ToString())
            {
                case "StartMediaReq":
                    test = true;
                    sendLen = temp.SendRequest(InterfaceC.StartMediaReq("6101010000000011", "6101010000000001", "63", "1", "0", "", "", "1"));
                    break;
                case "StopMediaReq":
                    sendLen = temp.SendRequest(InterfaceC.StopMediaReq("0000000000000000", "6101010000000001", "0"));
                    break;
                case "ControlPTZ":
                    sendLen = temp.SendRequest(InterfaceC.ControlPTZ("6100001201000101", "6101010000000001", "63", "RIGHT", "4", "4"));
                    break;
                case "QueryHistoryFiles":
                    sendLen = temp.SendRequest(InterfaceC.QueryHistoryFiles("6100001201000101", "6101010000000001", "0", "1111", "2015-03-22 12:22:33", "2015-03-22 12:42:33"));
                    break;
                case "StartPlayBack":
                    test = true;
                    sendLen = temp.SendRequest(InterfaceC.StartPlayBack("0000000000000000", "6101010000000001", "0", "2015-03-22 12:22:33", "2015-03-22 12:42:33", 0, "192.168.1.1", "15000", 1, 1));
                    break;
                case "ControlFileBack":
                    if(temp.sessionId != null)
                        sendLen = temp.SendRequest(InterfaceC.ControlFileBack(temp.sessionId, "6100001201000101", "RATE", 0));
                    break;
                case "StartHisLoad":
                    sendLen = temp.SendRequest(InterfaceC.StartHisLoad("0000000000000000", "6101010000000001", "0", "2015-03-22 12:22:33", "2015-03-22 12:42:33", 0, "192.168.1.1", "15000", 1, 1));
                    break;
                case "ReqCamResState":
                    sendLen = temp.SendRequest(InterfaceC.ReqCamResState("0000000000000000", str, 1));
                    break;
                case "GetUserCurState":
                    sendLen = temp.SendRequest(InterfaceC.GetUserCurState("0000000000000000", "6101010000000001"));
                    break;
                case "SetUserCamManage":
                    sendLen = temp.SendRequest(InterfaceC.SetUserCamManage("0000000000000000", "6101010000000001", 0, "2015-03-22 12:22:33", "2015-03-22 12:42:33", "2015-03-22 11:42:33", cameId, 2, id, 2));
                    break;
                case "AlarmResSubscribe":
                    sendLen = temp.SendRequest(InterfaceC.AlarmResSubscribe("0000000000000000", "6101010000000001", 0, id, type, 2));
                    break;
                case "QueryAlarmRes":
                    sendLen = temp.SendRequest(InterfaceC.QueryAlarmRes("6101010000000001", "saName", id, type, 2));
                    break;
                case "ResTransOrder":
                    sendLen = temp.SendRequest(InterfaceC.ResTransOrder("6101010000000001", "1", "1", resId, name, null, null, null, 2));
                    break;
                case "ResChangeOrder":
                    sendLen = temp.SendRequest(InterfaceC.ResChangeOrder("6101010000000001", "cmd", resId, name, null, null, null, 2));
                    break;
                case "ReportAlarmInfo":
                    sendLen = temp.SendRequest(InterfaceC.ReportAlarmInfo("6101010000000001", "muName", resId, type, startTime, endTime, 2));
                    break;
                default:
                    break;
            }

            result = WaitForResult(testsocket, timer, 2000);
            Console.WriteLine("Send Length: " + sendLen.ToString());
            if (result != null)
            {
                for (int k = 0; k < result.Length; k++)
                    Console.WriteLine(result[k]);
                if (test)
                {
                    temp.sessionId = result[0];
                }
            }
                
            //XXX.Send(InterfaceC.StartMediaReq("127.0.0.1", "12000", "6100011201000102", "6100011201000102", "1", "1", "0", "", "", "1"));
        }

        private string[] WaitForResult(Socket socket, System.Timers.Timer timer,int ms)
        {
            timer.Interval = ms;
            timer.Elapsed += new System.Timers.ElapsedEventHandler(Tick);
            string[] result = new string[10];
            timer.Enabled = true;
            while (true)
            {
                if (timeout)
                {
                    timeout = false;
                    result = null;
                    break;
                }
                if ((result = SipSocket.GetResult(socket)) != null)
                    break;
                Thread.Sleep(100);
            }
            timer.Enabled = false;
            return result;
        }

        private void Tick(object source, System.Timers.ElapsedEventArgs e)
        {
            timeout = true;
        }

        private void Combo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //SipSocket temp = SipSocket.FindSipSocket(testsocket);
            //string seleRequest;
            //int sendLen = 0;
            //seleRequest = Combo.SelectedValue.ToString().Substring(38);
            //switch (seleRequest)
            //{
            //    case "StartMediaReq":
            //        sendLen = temp.SendRequest(InterfaceC.StartMediaReq("6101010000000000", "6101010000000001", "63", "1", "0", "", "", "1"));
            //        break;
            //    case "StopMediaReq":
            //        sendLen = temp.SendRequest(InterfaceC.StopMediaReq("0000000000000000", "6101010000000001", "0"));
            //        break;
            //    case "QueryHistoryFiles":
            //        sendLen = temp.SendRequest(InterfaceC.QueryHistoryFiles("6100001201000101", "6101010000000001", "0", "1111", "2015-03-22 12:22:33", "2015-03-22 12:42:33"));
            //        break;
            //    case "StartPlayBack":
            //        sendLen = temp.SendRequest(InterfaceC.StartPlayBack("0000000000000000", "6101010000000001", "0", "2015-03-22 12:22:33", "2015-03-22 12:42:33", 0, "192.168.1.1", "15000", 1, 1));
            //        break;
            //    case "ControlFileBack":
            //        sendLen = temp.SendRequest(InterfaceC.ControlFileBack("0000000000000000", "6100001201000101", "PLAY", 0));
            //        break;
            //    case "StartHisLoad":
            //        sendLen = temp.SendRequest(InterfaceC.StartHisLoad("0000000000000000", "6101010000000001", "0", "2015-03-22 12:22:33", "2015-03-22 12:42:33", 0, "192.168.1.1", "15000", 1, 1));
            //        break;
            //    case "ReqCamResState":
            //        string[] str = { "000000", "1111111", "222222" };
            //        sendLen = temp.SendRequest(InterfaceC.ReqCamResState("0000000000000000", str, 1));
            //        break;
            //    case "GetUserCurState":
            //        sendLen = temp.SendRequest(InterfaceC.GetUserCurState("0000000000000000", "6101010000000001"));
            //        break;
            //    case "SetUserCamManage":
            //        string[] cameId = { "00000", "111111" }, id = { "222222", "333333" };
            //        sendLen = temp.SendRequest(InterfaceC.SetUserCamManage("0000000000000000", "6101010000000001", 0, "2015-03-22 12:22:33", "2015-03-22 12:42:33", "2015-03-22 11:42:33", cameId, 2, id, 2));
            //        break;
            //    case "AlarmResSubscribe":
            //        string[] type = { "666666", "4444444" }, id_0 = { "00000", "11111" };
            //        sendLen = temp.SendRequest(InterfaceC.AlarmResSubscribe("0000000000000000", "6101010000000001", 0, id_0, type, 2));
            //        break;
            //    default:
            //        break;
            //}

            //Console.WriteLine("Send Length: " + sendLen.ToString());
        }
    }
}
