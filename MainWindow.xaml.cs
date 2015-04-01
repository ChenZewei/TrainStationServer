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
        private SocketBound bound;
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
            //client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Bind(ipEnd);
            socket.Listen(20);
            //test = new SipSocket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //test.Bind(ipEnd);
            //test.Listen(50);
            Database = new DataBase();
            C = new InterfaceC(Database);
            stateobject mainObject = new stateobject();
            //mainSocket = new SipSocket(socket);
            mainObject.socket = socket;
            socket.BeginAccept(new AsyncCallback(AsyncAccept), mainObject);//开启异步监听委托，仍然是一个委托程序只处理一个连接请求
            Result.AppendText("Start listening...\r\n");
            exosip = new eXosip();
            snoopThread = new Thread(Snoop);
            snoopThread.IsBackground = true;
            snoopThread.Start();
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
            string sendbuffer;
            if (state.isClosed)
                return;
            try
            {
                state.socket.BeginReceive(state.recv, 0, state.BufferSize, 0, new AsyncCallback(recvProc), state);
                int i = state.socket.EndReceive(ar);
                this.Dispatcher.BeginInvoke(new Action(() => Result.AppendText(Encoding.GetEncoding("GB2312").GetString(state.recv, 0, i))));
                SocketBound.Add(state.socket, new SIPTools(state.recv, i));//将新连接的套接字放入SocketBound对象的sipsocket列表中
                string[] result;
                Doc = SIPTools.XmlExtract(recv, i);//去Sip头
                if (Doc == null)
                    return;
                if (InterfaceC.IsRequest(Doc))
                {
                    sendbuffer = SocketBound.FindSip(testsocket).SIPRequest(InterfaceC.Request(Doc));//处理请求消息并返回string格式的响应消息(Sip+Xml)
                    state.send = Encoding.GetEncoding("GB2312").GetBytes(sendbuffer);
                    state.socket.Send(state.send);
                }
                else
                {
                    result = InterfaceC.Response(state.recv, i);
                    if (result != null)
                    {
                        SocketBound.InsertResult(state.socket, result);
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
            if ((exoSocket = SocketBound.FindSocket(id.Substring(0, 6))) == null)
            {
                eXosip.Call.SendAnswer(eXosipEvent.tid, 404, IntPtr.Zero);
                eXosip.Unlock();
                return;
            }
            /*----------------------------分割线-----------------------------*/
            Request = InterfaceC.StopMediaReq("aaaaaaa","11111111","sad");//提取参数并转为C类接口格式

            temp = SocketBound.FindSipSocket(exoSocket);
            try
            {
                temp.Send(Request);
            }
            catch (SocketException ex)
            {
                System.Console.WriteLine(ex);
            }
            
            SocketBound.CleanResult(exoSocket);
        }

        private void osipCallInvite(eXosip.Event eXosipEvent)
        {
            Socket exoSocket;
            eXosip.Lock();
            int len;
            byte[] recv = new byte[2048];
            XmlDocument Request;
            SipSocket temp;
            //string tcpIp, tcpPort;
            string[] result = new string[10];
            eXosip.Call.SendAnswer(eXosipEvent.tid, 180, IntPtr.Zero);
            IntPtr sdp = eXosip.GetRemoteSdp(eXosipEvent.did);
            if (sdp == IntPtr.Zero)
            {
                eXosip.Call.SendAnswer(eXosipEvent.tid, 400, IntPtr.Zero);
                eXosip.Unlock();
                return;
            }
            osip.From pTo = osip.Message.GetTo(eXosipEvent.response);
            osip.URI uri = (osip.URI)Marshal.PtrToStructure(osip.From.GetURL(pTo.url), typeof(osip.URI));
            string name = osip.URI.ToString(pTo.url);
            string id = name.Substring(4, name.IndexOf('@') - 4);//仅需判断前六位号
            //goto loop;
            if ((exoSocket = SocketBound.FindSocket(id.Substring(0,6))) == null)//*问题*不应该执行if里面的语句。此处之所以找不到对应的套接字，主要是因为CU这边连接的套接字没有加入SocketBound对象的表中
            {
                eXosip.Call.SendAnswer(eXosipEvent.tid, 404, IntPtr.Zero);
                eXosip.Unlock();
                return;
            }
            string sessionname = osip.SdpMessage.GetSessionName(sdp);
            //以下的接口中的数据均为伪造，未测试版本，缺失提取Xml信息的步骤
            if (sessionname == "RealTime")
            {
                Request = InterfaceC.StartMediaReq("", "", "", "1", "0", "", "", "1");
            }
            else if (sessionname == "PlayBack")
            {
                Request = InterfaceC.StartPlayBack("1111111", "1111111", "1111111", "1990-03-22 22:33:22", "1990-03-22 23:33:22", 0, "192.168.1.1", "15000", 1, 0);
            }
            else //if (sessionname == "DownLoad")
            {
                Request = InterfaceC.StartHisLoad("1111111", "1111111", "1111111", "1990-03-22 22:33:22", "1990-03-22 23:33:22", 0, "192.168.1.1", "15000", 1, 0);//测试
            }
            temp = SocketBound.FindSipSocket(exoSocket);
            temp.Send(Request);
            System.Timers.Timer timer = new System.Timers.Timer(5000);
            timer.Elapsed += new System.Timers.ElapsedEventHandler(Tick);
            timer.Enabled = true;
            while (true)
            {
                if (timeout)
                {
                    timeout = false;
                    result = null;
                    break;
                }
                if ((result = SocketBound.GetResult(exoSocket)) != null)
                    break;
                Thread.Sleep(100);
            }
            timer.Enabled = false;
            if (result == null)
            {
                eXosip.Call.SendAnswer(eXosipEvent.tid, 404, IntPtr.Zero);
                eXosip.Unlock();
                return;
            }
            //loop:
            string sessionId = osip.SdpMessage.GetSessionId(sdp);
            string sessionVersion = osip.SdpMessage.GetSessionVersion(sdp);
            IntPtr answer = eXosip.Call.BuildAnswer(eXosipEvent.tid, 200);
            if (answer != IntPtr.Zero)
            {
                string tmp;
                if (sessionname == "RealTime")
                {
                    tmp = string.Format(
                       "v=0\r\n" +
                       "o={0} {1} {2} IN IP4 {3}\r\n" +
                       "s=RealTime\r\n" +
                       "c=IN IP4 {3}\r\n" +
                       "t=0 0\r\n" +
                       "a=sendonly\r\n" +
                       "m=video {4} TCP H264\r\n" +
                       "a=setup:passive\r\n" +
                       "a=connection:new\r\n" +
                       "m=audio {4} TCP PCMA\r\n" +
                       "a=setup:passive\r\n" +
                       "a=connection:new\r\n",
                       id,
                       sessionId,
                       sessionVersion,
                       "192.168.1.100",
                       "8888");
                }
                else if (sessionname == "PlayBack")
                {
                    tmp = string.Format(
                       "v=0\r\n" +
                       "o={0} {1} {2} IN IP4 {3}\r\n" +
                       "s=PlayBack\r\n" +
                       "c=IN IP4 {3}\r\n" +
                       "t=0 0\r\n" +
                       "a=sendonly\r\n" +
                       "m=video {4} TCP H264\r\n" +
                       "a=setup:passive\r\n" +
                       "a=connection:new\r\n" +
                       "m=audio {4} TCP PCMA\r\n" +
                       "a=setup:passive\r\n" +
                       "a=connection:new\r\n",
                       id,
                       sessionId,
                       sessionVersion,
                       "192.168.1.100",
                       "8888");
                }
                else
                {
                    tmp = string.Format(
                       "v=0\r\n" +
                       "o={0} {1} {2} IN IP4 {3}\r\n" +
                       "s=DownLoad\r\n" +
                       "c=IN IP4 {3}\r\n" +
                       "t=0 0\r\n" +
                       "a=sendonly\r\n" +
                       "m=video {4} TCP H264\r\n" +
                       "a=setup:passive\r\n" +
                       "a=connection:new\r\n" +
                       "m=audio {4} TCP PCMA\r\n" +
                       "a=setup:passive\r\n" +
                       "a=connection:new\r\n",
                       id,
                       sessionId,
                       sessionVersion,
                       "192.168.1.100",
                       "8888");
                }
                
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
            ptr = osip.Message.GetContentType(eXosipEvent.request);
            if (ptr == IntPtr.Zero) return;
            osip.ContentType content = (osip.ContentType)Marshal.PtrToStructure(ptr, typeof(osip.ContentType));
            ptr = osip.Message.GetBody(eXosipEvent.request);
            if (ptr == IntPtr.Zero) return;

            osip.From pTo = osip.Message.GetTo(eXosipEvent.request);
            osip.URI uri = (osip.URI)Marshal.PtrToStructure(osip.From.GetURL(pTo.url), typeof(osip.URI));
            string name = osip.URI.ToString(pTo.url);
            string id = name.Substring(4, name.IndexOf('@') - 4);
            //id = "6100002008000001";
            if ((exoSocket = SocketBound.FindSocket(id.Substring(0, 6))) == null)
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
            Request = InterfaceC.Translate(TempDoc);//提取参数并转为C类接口格式
            
            temp = SocketBound.FindSipSocket(exoSocket);
            temp.Send(Request);
            SocketBound.CleanResult(exoSocket);
        }

        private void Test_Click_1(object sender, RoutedEventArgs e)//测试用
        {
            byte[] recv = new byte[2048];
            string[] result = new string[10];
            stateobject temp = new stateobject();
            System.Timers.Timer timer = new System.Timers.Timer(5000);
            timer.Elapsed += new System.Timers.ElapsedEventHandler(Tick);
            SocketBound.CleanResult(testsocket);
            testsocket.Send(Encoding.GetEncoding("GB2312").GetBytes(SocketBound.FindSip(testsocket).SIPRequest(InterfaceC.StartMediaReq("", "", "", "1", "0", "", "", "1"))));
            timer.Enabled = true;
            while (true)
            {
                if(timeout)
                {
                    timeout = false;
                    result = null;
                    break;
                }
                if ((result = SocketBound.GetResult(testsocket)) != null)
                    break;
                Thread.Sleep(100);
            }
            timer.Enabled = false;
            if(result != null)
                for (int k = 0; k < result.Length; k++)
                    Console.WriteLine(result[k]);
            //XXX.Send(InterfaceC.StartMediaReq("127.0.0.1", "12000", "6100011201000102", "6100011201000102", "1", "1", "0", "", "", "1"));
        }

        private void Tick(object source, System.Timers.ElapsedEventArgs e)
        {
            timeout = true;
        }
    }
}
