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
            public int BufferSize = 8000;
            public byte[] recv;
            public byte[] send;
            public int recvLen;
        }
        private Socket socket, client, client2, testsocket, socket2;
        private IPEndPoint ipEnd, ipEnd2;
        private eXosip exosip;
        private Thread snoopThread;
        private DataBase Database;
        private InterfaceC C;
        private System.Timers.Timer timer;
        private bool timeout = false;
        private byte[] recv = new byte[8000], send = new byte[8000];
        private byte[] recv2 = new byte[8000], send2 = new byte[8000];
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Start_Click_1(object sender, RoutedEventArgs e)//绑定套接字等初始化
        {
            ipEnd = new IPEndPoint(IPAddress.Any, 15000);
            ipEnd2 = new IPEndPoint(IPAddress.Any, 16000);
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket2 = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Bind(ipEnd);
            socket2.Bind(ipEnd2);
            socket.Listen(20);
            socket2.Listen(5);
            Database = new DataBase();
            C = new InterfaceC(Database);
            stateobject mainObject = new stateobject();
            mainObject.socket = socket;
            stateobject mainObject2 = new stateobject();
            mainObject2.socket = socket2;
            socket.BeginAccept(new AsyncCallback(AsyncAccept), mainObject);
            socket2.BeginAccept(new AsyncCallback(AsyncAccept2), mainObject2);
            Result.AppendText("Start listening...\r\n");
            exosip = new eXosip();
            snoopThread = new Thread(Snoop);
            snoopThread.IsBackground = true;
            //snoopThread.Start();
            timer = new System.Timers.Timer(60000);
            timer.Elapsed += new System.Timers.ElapsedEventHandler(ClearTextBox);
            timer.Enabled = true;
            Start.IsEnabled = false;
            //Combo.IsEnabled = true;
            //Test.IsEnabled = true;
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
            this.Dispatcher.BeginInvoke(new Action(() => Combo.IsEnabled = true));
            this.Dispatcher.BeginInvoke(new Action(() => Test.IsEnabled = true));
            //if (Test.IsEnabled == false)
            //{
            //    Combo.IsEnabled = true;
            //    Test.IsEnabled = true;
            //}
        }

        private void AsyncAccept2(IAsyncResult ar)//异步Accept
        {
            stateobject mainObject = (stateobject)ar.AsyncState;
            stateobject clientObject = new stateobject();
            client2 = mainObject.socket.EndAccept(ar);
            mainObject.socket.BeginAccept(new AsyncCallback(AsyncAccept2), mainObject);
            this.Dispatcher.BeginInvoke(new Action(() => Result.AppendText("Accepted...\r\n")));
            clientObject.socket = client2;
            clientObject.recv = recv2;
            clientObject.send = send2;
            client2.BeginReceive(clientObject.recv, 0, clientObject.BufferSize, 0, new AsyncCallback(recvProc2), clientObject);
        }

        void recvProc(IAsyncResult ar)//异步Receive
        {
            stateobject state = (stateobject)ar.AsyncState;
            XmlDocument Doc = new XmlDocument();
            SipSocket temp;
            bool Added = false;
            int cseq;
            int recvlen;
            string[] result;
            int targetIndex = 0;
            byte[] recv = new byte[10000];
            byte[] temprecv = new byte[10000];
            string tt;
            string sip, xml;
            temp = SipSocket.FindSipSocket(state.socket);
            if(temp != null)
            {
                if(temp.lastRecv != null)
                {
                    targetIndex = temp.lastRecv.Length;
                    temp.lastRecv.CopyTo(recv, 0);
                    temp.lastRecv = null;
                }
            }
            else
            {
                Added = SipSocket.Add(state.socket, new SIPTools(""));
                temp = SipSocket.FindSipSocket(state.socket);
            }
            try
            {
                int i = state.socket.EndReceive(ar);
                if (i <= 0)
                {
                    state.socket.Shutdown(SocketShutdown.Both);
                    state.socket.Close();
                    return;
                }
                recvlen = i + targetIndex;
                this.Dispatcher.BeginInvoke(new Action(() => Result.AppendText(Encoding.GetEncoding("GB2312").GetString(state.recv, 0, i))));
                Console.WriteLine(Encoding.GetEncoding("GB2312").GetString(state.recv, 0, i));
                Copy(state.recv, 0, recv, targetIndex, state.recv.Length);
                for (int j = 0; j < 8000; j++)
                {
                    state.recv[j] = 0;
                }
                do
                {
                    Doc = new XmlDocument();
                    if(SIPTools.Extraction(ref temp, ref recv, ref recvlen, out sip, out xml))
                    {
                        temp.sip.Refresh(sip);
                        //temp = SipSocket.FindSipSocket(state.socket);
                        //if (temp == null)
                        //    return;
                        cseq = SIPTools.getCSeq(Encoding.ASCII.GetBytes(sip));
                        if (cseq == -1)
                        {
                            temp.lastRecv = new byte[recv.Length];
                            Copy(recv, 0, temp.lastRecv, 0, recv.Length);
                            state.socket.BeginReceive(state.recv, 0, state.BufferSize, 0, new AsyncCallback(recvProc), state);
                            return;
                        }
                        Doc.LoadXml(xml);
                        if (Doc == null)
                        {
                            temp.lastRecv = new byte[recv.Length];
                            Copy(recv, 0, temp.lastRecv, 0, recv.Length);
                            Console.WriteLine("Xml extraction failed.");
                            state.socket.BeginReceive(state.recv, 0, state.BufferSize, 0, new AsyncCallback(recvProc), state);
                            return;
                        }
                        if (InterfaceC.IsRequest(Doc))
                        {
                            //temp.sip.Refresh(sip);
                            temp.SendResponse(InterfaceC.Request(Doc, temp));
                        }
                        else
                        {
                            result = InterfaceC.Response(Doc, temp);
                            if (result != null)
                            {
                                if (result[0] != "sendback")
                                {
                                    SipSocket.SetResult(state.socket, result);
                                    for (int k = 0; k < result.Length; k++)
                                        Console.WriteLine(result[k]);
                                }
                                else
                                {
                                    ResponseList RL = new ResponseList(cseq, Doc);
                                    temp.XmlList.Add(RL);
                                    XmlDocument response = new XmlDocument();
                                    while (true)
                                    {
                                        response = temp.Redy2Return();
                                        if (response != null)
                                        {
                                            int j = client2.Send(Encoding.GetEncoding("GB2312").GetBytes(response.OuterXml));
                                            if (j <= 0)
                                            {
                                                client2.Close();
                                            }
                                            Console.WriteLine("<---------------------------------------->");
                                            Console.WriteLine("SendToServer: " + j.ToString());
                                            if (temp.XmlList.Count == 0)
                                            {
                                                client2.Shutdown(SocketShutdown.Both);
                                                client2.Close();
                                                break;
                                            }
                                        }
                                        else
                                        {
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        break;
                    }
                    //temprecv = SIPTools.PckExtract(ref recv, recvlen);
                    //if (temprecv != null)
                    //{
                    //    recvlen = recv.Length;
                    //    //tt = Encoding.GetEncoding("GB2312").GetString(temprecv, 0, temprecv.Length);
                    //    //Console.WriteLine(tt);
                    //}
                    //else //if(temprecv == null)
                    //{
                    //    temp.lastRecv = new byte[recv.Length];
                    //    Copy(recv, 0, temp.lastRecv, 0, recv.Length);
                    //    state.socket.BeginReceive(state.recv, 0, state.BufferSize, 0, new AsyncCallback(recvProc), state);
                    //    return;
                    //}

                    //Added = SipSocket.Add(state.socket, new SIPTools(temprecv, temprecv.Length));
                    //temp = SipSocket.FindSipSocket(state.socket);
                    //if (temp == null)
                    //    return;
                    ////Console.WriteLine("Received from: " + temp.sip.Id);
                    ////Console.WriteLine(Encoding.GetEncoding("GB2312").GetString(recv, 0, i));
                    //cseq = SIPTools.getCSeq(temprecv);
                    //if (cseq == -1)
                    //{
                    //    temp.lastRecv = new byte[temprecv.Length];
                    //    Copy(temprecv, 0, temp.lastRecv, 0, temprecv.Length);
                    //    state.socket.BeginReceive(state.recv, 0, state.BufferSize, 0, new AsyncCallback(recvProc), state);
                    //    return;
                    //}
                    //Doc = SIPTools.XmlExtract(temprecv, temprecv.Length);
                    //if (Doc == null)
                    //{
                    //    temp.lastRecv = new byte[temprecv.Length];
                    //    Copy(temprecv, 0, temp.lastRecv, 0, temprecv.Length);
                    //    Console.WriteLine("Xml extraction failed.");
                    //    state.socket.BeginReceive(state.recv, 0, state.BufferSize, 0, new AsyncCallback(recvProc), state);
                    //    return;
                    //}

                    //if (InterfaceC.IsRequest(Doc))
                    //{
                    //    temp.sip.Refresh(temprecv, temprecv.Length);
                    //    temp.SendResponse(InterfaceC.Request(Doc, temp));
                    //}
                    //else
                    //{
                    //    result = InterfaceC.Response(Doc, temp);
                    //    if (result != null)
                    //    {
                    //        if (result[0] != "sendback")
                    //        {
                    //            SipSocket.SetResult(state.socket, result);
                    //            for (int k = 0; k < result.Length; k++)
                    //                Console.WriteLine(result[k]);
                    //        }
                    //        else
                    //        {
                    //            ResponseList RL = new ResponseList(cseq, Doc);
                    //            temp.XmlList.Add(RL);
                    //            XmlDocument response = new XmlDocument();
                    //            while (true)
                    //            {
                    //                response = temp.Redy2Return();
                    //                if (response != null)
                    //                {
                    //                    int j = client2.Send(Encoding.GetEncoding("GB2312").GetBytes(response.OuterXml));
                    //                    if (j <= 0)
                    //                    {
                    //                        client2.Close();
                    //                    }
                    //                    Console.WriteLine("<---------------------------------------->");
                    //                    Console.WriteLine("SendToServer: " + j.ToString());
                    //                    if (temp.XmlList.Count == 0)
                    //                    {
                    //                        client2.Shutdown(SocketShutdown.Both);
                    //                        client2.Close();
                    //                        break;
                    //                    }
                    //                }
                    //                else
                    //                {
                    //                    break;
                    //                }
                    //            }
                    //        }
                    //    }
                    //}

                } while (recvlen > 0);
                
                    
                
                //if(Added)
                //{
                //    if(!InterfaceC.IsSaRegister(Doc, temp))
                //    {
                //        temp.SendResponse(InterfaceC.Error("SA未注册"));
                //        temp.socket.Close();
                //        SipSocket.Delete(temp.socket);
                //    }
                //}
                state.socket.BeginReceive(state.recv, 0, state.BufferSize, 0, new AsyncCallback(recvProc), state);
            }
            catch(SocketException e)
            {
                Console.WriteLine("recvProc: " + e.Message);
                try
                {
                    state.socket.BeginReceive(state.recv, 0, state.BufferSize, 0, new AsyncCallback(recvProc), state);
                }
                catch (SocketException w)
                {
                    Console.WriteLine("recvProc: " + w.Message);
                }
                return;
            }
            catch(XmlException e)
            {
                Console.WriteLine("recvProc: " + e.Message);
                try
                {
                    state.socket.BeginReceive(state.recv, 0, state.BufferSize, 0, new AsyncCallback(recvProc), state);
                }
                catch (SocketException w)
                {
                    Console.WriteLine("recvProc: " + w.Message);
                }
                return;
            }
            catch(ObjectDisposedException e)
            {
                Console.WriteLine("recvProc: " + e.Message);
                try
                {
                    state.socket.BeginReceive(state.recv, 0, state.BufferSize, 0, new AsyncCallback(recvProc), state);
                }
                catch (SocketException w)
                {
                    Console.WriteLine("recvProc: " + w.Message);
                }
                return;
            }
        }

        void recvProc2(IAsyncResult ar)//接收服务器请求
        {
            stateobject state = (stateobject)ar.AsyncState;
            XmlDocument Doc = new XmlDocument();
            SipSocket temp;
            if (!state.socket.Connected)
                return;
            try
            {
                int i = state.socket.EndReceive(ar);
                if (i <= 0)
                {
                    state.socket.Shutdown(SocketShutdown.Both);
                    state.socket.Close();
                    return;
                }   
                state.socket.BeginReceive(state.recv, 0, state.BufferSize, 0, new AsyncCallback(recvProc2), state);
                string recvbuffer = Encoding.GetEncoding("GB2312").GetString(state.recv, 0, i);
                Doc.LoadXml(recvbuffer);
                XmlTools XmlOp = new XmlTools();
                string resId = XmlOp.GetInnerText(Doc, "resId");
                temp = SipSocket.FindSipSocket(resId);
                if (temp == null)
                {
                    Console.WriteLine("SA server not found.");
                    return;
                }
                int j = temp.SendRequest(Doc);
                Console.WriteLine("<========================================>");
                Console.WriteLine("SendToClient: " + j.ToString());
                Console.WriteLine(Encoding.GetEncoding("GB2312").GetString(state.recv, 0, i));
                this.Dispatcher.BeginInvoke(new Action(() => Result.AppendText(Encoding.GetEncoding("GB2312").GetString(state.recv, 0, i))));
            }
            catch (SocketException e)
            {
                Console.WriteLine(e.Message);
                return;
            }
            catch (XmlException e)
            {
                Console.WriteLine("recvProc2: " + e.Message);
                return;
            }
            catch(ObjectDisposedException e)
            {
                Console.WriteLine("recvProc2: " + e.Message);
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
                        //osipCallMessage(eXosipEvent);
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
            IntPtr ptr; 
            byte[] recv = new byte[2048];
            XmlDocument Request = new XmlDocument(); ;
            SipSocket temp;
            ptr = osip.Message.GetContentType(eXosipEvent.request);
            if (ptr == IntPtr.Zero) return;
            osip.ContentType content = (osip.ContentType)Marshal.PtrToStructure(ptr, typeof(osip.ContentType));
            System.Timers.Timer timer = new System.Timers.Timer(2000);
            timer.Elapsed += new System.Timers.ElapsedEventHandler(Tick);
            string[] result = new string[10];
            try
            {
                eXosip.Call.SendAnswer(eXosipEvent.tid, 180, IntPtr.Zero);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            IntPtr sdp = eXosip.GetRemoteSdp(eXosipEvent.did);
            try
            {
                if (sdp == IntPtr.Zero)
                {
                    eXosip.Call.SendAnswer(eXosipEvent.tid, 400, IntPtr.Zero);
                    eXosip.Unlock();
                    return;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("osipCallInvite: " + e.Message);
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
            exoSocket = SipSocket.FindSocket(resId.Substring(0, 6));
            try
            {
                if (exoSocket == null)
                {
                    eXosip.Call.SendAnswer(eXosipEvent.tid, 404, IntPtr.Zero);
                    eXosip.Unlock();
                    return;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("osipCallInvite: " + e.Message);
            }

            string sessionname = osip.SdpMessage.GetSessionName(sdp);

            if (sessionname == "RealTime")
            {
                Request = InterfaceC.StartMediaReq(resId, userId, "63", "1", "0", "", "", "1");
            }
            else if (sessionname == "PlayBack")
            {
                ptr = osip.Message.GetBody(eXosipEvent.request);
                osip.Body body = (osip.Body)Marshal.PtrToStructure(ptr, typeof(osip.Body));
                if (Marshal.PtrToStringAnsi(content.type) != "RVSS" ||
                    Marshal.PtrToStringAnsi(content.subtype) != "xml")
                    return;
                string xml = Marshal.PtrToStringAnsi(body.body);
                Console.Write("PlayBack:\r\n" + xml);
                try
                {
                    Request.LoadXml(xml);
                }
                catch (XmlException e)
                {
                    Console.WriteLine("osipCallInvite: " + e.Message);
                    return;
                }
            }
            else if (sessionname == "DownLoad")
            {
                ptr = osip.Message.GetBody(eXosipEvent.request);
                osip.Body body = (osip.Body)Marshal.PtrToStructure(ptr, typeof(osip.Body));
                if (Marshal.PtrToStringAnsi(content.type) != "RVSS" ||
                    Marshal.PtrToStringAnsi(content.subtype) != "xml")
                    return;
                string xml = Marshal.PtrToStringAnsi(body.body);
                Console.Write("DownLoad:\r\n" + xml);
                try
                {
                    Request.LoadXml(xml);
                }
                catch (XmlException e)
                {
                    Console.WriteLine("osipCallInvite: " + e.Message);
                    return;
                }
            }
            else
            {
                eXosip.Call.SendAnswer(eXosipEvent.tid, 404, IntPtr.Zero);
                eXosip.Unlock();
                return;
            }

            temp = SipSocket.FindSipSocket(exoSocket);
            SipSocket.CleanResult(exoSocket);
            temp.SendRequest(Request);
            result = WaitForResult(temp.socket, timer, 5000);

            if (result != null)
            {
                switch (sessionname)
                {
                    case "RealTime":
                        temp.sessionIdRT = result[0];
                        break;
                    case "PlayBack":
                        temp.sessionIdPB = result[0];
                        break;
                    case "DownLoad":
                        temp.sessionIdDL = result[0];
                        break;
                }
                for (int k = 0; k < result.Length; k++)
                    Console.WriteLine(result[k]);
            }
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

        //void osipCallMessage(eXosip.Event eXosipEvent)
        //{
        //    IntPtr ptr;
        //    XmlDocument TempDoc = new XmlDocument();
        //    //XmlDocument Request;
        //    Socket exoSocket;
        //    //SipSocket temp;
        //    string[] result = new string[10];
        //    System.Timers.Timer timer = new System.Timers.Timer(2000);
        //    ptr = osip.Message.GetContentType(eXosipEvent.request);
        //    if (ptr == IntPtr.Zero) return;
        //    osip.ContentType content = (osip.ContentType)Marshal.PtrToStructure(ptr, typeof(osip.ContentType));
        //    ptr = osip.Message.GetBody(eXosipEvent.request);
        //    if (ptr == IntPtr.Zero) return;

        //    osip.From pTo = osip.Message.GetTo(eXosipEvent.request);
        //    osip.From pFrom = osip.Message.GetFrom(eXosipEvent.request);
        //    osip.URI uriTo = (osip.URI)Marshal.PtrToStructure(osip.From.GetURL(pTo.url), typeof(osip.URI));
        //    osip.URI uriFrom = (osip.URI)Marshal.PtrToStructure(osip.From.GetURL(pFrom.url), typeof(osip.URI));
        //    string name = osip.URI.ToString(pTo.url);
        //    string name2 = osip.URI.ToString(pFrom.url);
        //    string resId = name.Substring(4, name.IndexOf('@') - 4);
        //    string userCode = name2.Substring(4, name2.IndexOf('@') - 4);
        //    string userId = resId.Substring(0, 10) + userCode;
        //    try
        //    {
        //        if ((exoSocket = SipSocket.FindSocket(resId.Substring(0, 6))) == null)
        //        {
        //            eXosip.Call.SendAnswer(eXosipEvent.tid, 404, IntPtr.Zero);
        //            eXosip.Unlock();
        //            return;
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine(e.Message);
        //    }
        //    IntPtr sdp = eXosip.GetRemoteSdp(eXosipEvent.did);
        //    string sessionname = osip.SdpMessage.GetSessionName(sdp);

        //    osip.Body data = (osip.Body)Marshal.PtrToStructure(ptr, typeof(osip.Body));
        //    if (Marshal.PtrToStringAnsi(content.type) != "application" ||
        //        Marshal.PtrToStringAnsi(content.subtype) != "xml")
        //        return;
        //    string xml = Marshal.PtrToStringAnsi(data.body);
        //    Console.Write(xml);
        //    /*----------------------------分割线-----------------------------*/
        //    //TempDoc.LoadXml(xml);
        //    //temp = SipSocket.FindSipSocket(exoSocket);
        //    //Request = InterfaceC.CallMessageTranslate(TempDoc, resId, userId);//提取参数并转为C类接口格式
        //    //SipSocket.CleanResult(exoSocket);
        //    //temp.SendRequest(Request);
        //    //result = WaitForResult(testsocket, timer, 2000);

        //    //if (result != null)
        //    //    for (int k = 0; k < result.Length; k++)
        //    //        Console.WriteLine(result[k]);
        //    //temp = SipSocket.FindSipSocket(exoSocket);
        //    //temp.SendRequest(Request); 
        //}

        private void Test_Click_1(object sender, RoutedEventArgs e)//测试用
        {
            byte[] recv = new byte[2048];
            string[] result = new string[10];
            int sendLen = 0;
            bool test = false;
            SipSocket temp = SipSocket.FindSipSocket(testsocket);
            if (temp == null)
                return;
            System.Timers.Timer timer1 = new System.Timers.Timer(2000);
            SipSocket.CleanResult(testsocket);
            string[] resId = { "6101010000000001", "6101010000000002" }, name = { "01", "02" };
            string[] camId = { "6100001201000301" }, id = { "6100001201000301" };
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
                    if(temp.sessionIdPB != null)
                        sendLen = temp.SendRequest(InterfaceC.ControlFileBack(temp.sessionIdPB, "6100001201000101", "RATE", 0));
                    break;
                case "StartHisLoad":
                    test = true;
                    sendLen = temp.SendRequest(InterfaceC.StartHisLoad("0000000000000000", "6101010000000001", "0", "2015-03-22 12:22:33", "2015-03-22 12:42:33", 0, "192.168.1.1", "15000", 1, 1));
                    break;
                case "ReqCamResState":
                    sendLen = temp.SendRequest(InterfaceC.ReqCamResState("0000000000000000", str, 1));
                    break;
                case "GetUserCurState":
                    sendLen = temp.SendRequest(InterfaceC.GetUserCurState("0000000000000000", "6101010000000001"));
                    break;
                case "SetUserCamManage":
                    sendLen = temp.SendRequest(InterfaceC.SetUserCamManage("6100003020990001", "0", 0, "2015-04-17 16:05:45", "2015-04-17 17:05:45", "2015-04-17 16:05:58", camId, 1, null, 0));
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

            result = WaitForResult(testsocket, timer1, 2000);
            Console.WriteLine("\r\n<===================>\r\n");
            Console.WriteLine("Send Length: " + sendLen.ToString());
            if (result != null)
            {
                for (int k = 0; k < result.Length; k++)
                    Console.WriteLine(result[k]);
                if(test)
                {
                    switch (Combo.SelectionBoxItem.ToString())
                    {
                        case "StartMediaReq":
                            temp.sessionIdRT = result[0];
                            break;
                        case "StartPlayBack":
                            temp.sessionIdPB = result[0];
                            break;
                        case "StartHisLoad":
                            temp.sessionIdDL = result[0];
                            break;
                    }
                }
            }
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

        private void ClearTextBox(object source, System.Timers.ElapsedEventArgs e)
        {
            this.Dispatcher.BeginInvoke(new Action(() => Result.Clear()));
        }

        ~MainWindow()
        {
            try
            {
                if (socket != null)
                {
                    socket.Shutdown(SocketShutdown.Both);
                    socket.Close();
                    socket.Dispose();
                }
                if (client != null)
                {
                    client.Shutdown(SocketShutdown.Both);
                    client.Close();
                    client.Dispose();
                }
                if (client2 != null)
                {
                    client2.Shutdown(SocketShutdown.Both);
                    client2.Close();
                    client2.Dispose();
                }
                if (testsocket != null)
                {
                    testsocket.Shutdown(SocketShutdown.Both);
                    testsocket.Close();
                    testsocket.Dispose();
                }
                if (socket2 != null)
                {
                    socket2.Shutdown(SocketShutdown.Both);
                    socket2.Close();
                    socket2.Dispose();
                }
            }
            catch(SocketException e)
            {
                Console.WriteLine(e.Message);
            }
            

            SipSocket.CloseAllSocket();

        }

        void Copy(byte[] source, int sourceOffset, byte[] target, int targetOffset, int count)
        {
            byte[] result = new byte[source.Length + count];
            // If either array is not instantiated, you cannot complete the copy. 
            if ((source == null) || (target == null))
            {
                throw new System.ArgumentException();
            }
            byte[] test = new byte[count];
            // If either offset, or the number of bytes to copy, is negative, you 
            // cannot complete the copy. 
            if ((sourceOffset < 0) || (targetOffset < 0) || (count < 0))
            {
                throw new System.ArgumentException();
            }

            // If the number of bytes from the offset to the end of the array is  
            // less than the number of bytes you want to copy, you cannot complete 
            // the copy.  
            if ((source.Length - sourceOffset < count) ||
                (target.Length - targetOffset < count))
            {
                throw new System.ArgumentException();
            }

            for (int i = 0; i < count; i++)
            {
                target[targetOffset + i] = source[sourceOffset + i];
            }
        }

    }
}
