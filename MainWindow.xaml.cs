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
            socket.BeginAccept(new AsyncCallback(AsyncAccept), mainObject);
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
                SocketBound.Add(state.socket, new SIPTools(state.recv, i));
                string[] result;
                Doc = SIPTools.XmlExtract(recv, i);
                if (Doc == null)
                    return;
                if (InterfaceC.IsRequest(Doc))
                {
                    sendbuffer = SocketBound.FindSip(testsocket).SIPRequest(InterfaceC.Request(Doc));
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
                    case eXosip.EventType.EXOSIP_MESSAGE_NEW:
                        osipMessage(eXosipEvent);
                        break;
                    case eXosip.EventType.EXOSIP_CALL_CLOSED:
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

            osip.From pTo = osip.Message.GetTo(eXosipEvent.request);
            osip.From pFrom = osip.Message.GetFrom(eXosipEvent.request);
            osip.URI uriTo = (osip.URI)Marshal.PtrToStructure(osip.From.GetURL(pTo.url), typeof(osip.URI));
            osip.URI uriFrom = (osip.URI)Marshal.PtrToStructure(osip.From.GetURL(pFrom.url), typeof(osip.URI));
            string name = osip.URI.ToString(pTo.url);
            string name2 = osip.URI.ToString(pFrom.url);
            string resId = name.Substring(4, name.IndexOf('@') - 4);
            string userId = name2.Substring(4, name2.IndexOf('@') - 4);
            //id = "6100002008000001";
            if ((exoSocket = SocketBound.FindSocket(resId.Substring(0, 6))) == null)//*问题*不应该执行if里面的语句。此处之所以找不到对应的套接字，主要是因为CU这边连接的套接字没有加入SocketBound对象的表中
            {
                eXosip.Call.SendAnswer(eXosipEvent.tid, 404, IntPtr.Zero);
                eXosip.Unlock();
                return;
            }

            Request = InterfaceC.StartMediaReq(resId, userId, "63", "1", "0", "", "", "1");
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
            string sessionId = osip.SdpMessage.GetSessionId(sdp);
            string sessionVersion = osip.SdpMessage.GetSessionVersion(sdp);
            IntPtr answer = eXosip.Call.BuildAnswer(eXosipEvent.tid, 200);
            if (answer != IntPtr.Zero)
            {
                string tmp = string.Format(
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
                    resId,
                    sessionId,
                    sessionVersion,
                    result[1],
                    result[2]);
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

            //eXosip.Call.SendAnswer(eXosipEvent.tid, 180, IntPtr.Zero);
            //IntPtr sdp = eXosip.GetRemoteSdp(eXosipEvent.did);
            //if (sdp == IntPtr.Zero)
            //{
            //    eXosip.Call.SendAnswer(eXosipEvent.tid, 400, IntPtr.Zero);
            //    eXosip.Unlock();
            //    return;
            //}
            osip.From pTo = osip.Message.GetTo(eXosipEvent.request);
            osip.From pFrom = osip.Message.GetFrom(eXosipEvent.request);
            osip.URI uriTo = (osip.URI)Marshal.PtrToStructure(osip.From.GetURL(pTo.url), typeof(osip.URI));
            osip.URI uriFrom = (osip.URI)Marshal.PtrToStructure(osip.From.GetURL(pFrom.url), typeof(osip.URI));
            string name = osip.URI.ToString(pTo.url);
            string name2 = osip.URI.ToString(pFrom.url);
            string resId = name.Substring(4, name.IndexOf('@') - 4);
            string userId = name2.Substring(4, name2.IndexOf('@') - 4);
            //userId = "6101012008000001 ";
            if ((exoSocket = SocketBound.FindSocket(resId.Substring(0, 6))) == null)
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
            Request = InterfaceC.Translate(TempDoc, resId, userId);//提取参数并转为C类接口格式
            SocketBound.CleanResult(exoSocket);
            temp = SocketBound.FindSipSocket(exoSocket);
            temp.Send(Request); 
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

            //eXosip.Call.SendAnswer(eXosipEvent.tid, 180, IntPtr.Zero);
            //IntPtr sdp = eXosip.GetRemoteSdp(eXosipEvent.did);
            //if (sdp == IntPtr.Zero)
            //{
            //    eXosip.Call.SendAnswer(eXosipEvent.tid, 400, IntPtr.Zero);
            //    eXosip.Unlock();
            //    return;
            //}
            osip.From pTo = osip.Message.GetTo(eXosipEvent.request);
            osip.From pFrom = osip.Message.GetFrom(eXosipEvent.request);
            osip.URI uriTo = (osip.URI)Marshal.PtrToStructure(osip.From.GetURL(pTo.url), typeof(osip.URI));
            osip.URI uriFrom = (osip.URI)Marshal.PtrToStructure(osip.From.GetURL(pFrom.url), typeof(osip.URI));
            string name = osip.URI.ToString(pTo.url);
            string name2 = osip.URI.ToString(pFrom.url);
            string resId = name.Substring(4, name.IndexOf('@') - 4);
            string userId = name2.Substring(4, name2.IndexOf('@') - 4);
            //userId = "6101012008000001 ";
            if ((exoSocket = SocketBound.FindSocket(resId.Substring(0, 6))) == null)
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
            Request = InterfaceC.Translate(TempDoc, resId, userId);//提取参数并转为C类接口格式
            //SocketBound.CleanResult(exoSocket);
            //temp = SocketBound.FindSipSocket(exoSocket);
            //temp.Send(Request); 
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
