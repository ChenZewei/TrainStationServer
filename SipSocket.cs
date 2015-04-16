using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Xml;

namespace TrainStationServer
{
    class SipSocket
    {
        public Socket socket;
        public SIPTools sip;
        public string[] result;
        public string sessionIdRT, sessionIdPB, sessionIdDL;
        public string saId;
        public string[] resId;
        private static List<SipSocket> sipsocket = new List<SipSocket>();
        public List<XmlDocument> XmlList = new List<XmlDocument>();

        private SipSocket()
        {

        }

        public SipSocket(Socket skt)
        {
            socket = skt;
        }

        public SipSocket(Socket skt, SIPTools s)
        {
            socket = skt;
            sip = s;
        }

        public SipSocket(AddressFamily af, SocketType st, ProtocolType pt)
        {
            socket = new Socket(af, st, pt);
        }

        public void SipInit(byte[] buffer, int length)
        {
            sip = new SIPTools(buffer, length);
            result = new string[10];
        }

        public void SetResult(params string[] result)
        {
            int i = 0;
            foreach(string temp in result)
            {
                result[i++] = temp;
            }
        }

        public string[] GetResult()
        {
            string[] temp = result;
            result = null;
            return temp;
        }

        public int SendRequest(XmlDocument doc)
        {
            try
            {
                return socket.Send(Encoding.GetEncoding("GB2312").GetBytes(sip.SIPRequest(doc)));
            }
            catch(ObjectDisposedException e)
            {
                Delete(socket);
                socket.Dispose();
                return -1;
            }
        }

        public int SendResponse(XmlDocument doc)
        {
            try
            {
                return socket.Send(Encoding.GetEncoding("GB2312").GetBytes(sip.SIPResponse(doc)));
            }
            catch (ObjectDisposedException e)
            {
                Console.WriteLine(e.Message);
                Delete(socket);
                socket.Dispose();
                return -1;
            }
        }

        //static

        public static void Add(Socket socket, SIPTools sip)
        {
            foreach (SipSocket ss in sipsocket)
            {
                if (ss.socket.Equals(socket))
                    return;
            }
            SipSocket temp = new SipSocket(socket, sip);
            sipsocket.Add(temp);
        }

        public static void Delete(Socket socket)
        {
            if (sipsocket.Count == 0)
                return;
            foreach (SipSocket ss in sipsocket)
            {
                if (ss.socket.Equals(socket))
                {
                    sipsocket.Remove(ss);
                    return;
                }
            }
            return;
        }

        public static Socket FindSocket(string prefix)//确认sipsocket列表中是否已经存在指定id的Socket
        {
            if (sipsocket.Count == 0)
                return null;
            foreach (SipSocket temp in sipsocket)
            {
                if (temp.sip.Id.StartsWith(prefix))
                {
                    return temp.socket;
                }
            }
            return null;
        }

        public static SIPTools FindSip(Socket socket)//确认sipsocket列表中是否已经存在指定id的SIPTools
        {
            if (sipsocket.Count == 0)
                return null;
            foreach (SipSocket temp in sipsocket)
            {
                if (temp.socket.Equals(socket))
                {
                    return temp.sip;
                }
            }
            return null;
        }

        public static SipSocket FindSipSocket(Socket socket)
        {
            if (sipsocket.Count == 0)
                return null;
            foreach (SipSocket temp in sipsocket)
            {
                if (temp.socket.Equals(socket))
                {
                    return temp;
                }
            }
            return null;
        }

        public static SipSocket FindSipSocket(string resId)
        {
            if (sipsocket.Count == 0)
                return null;
            foreach (SipSocket temp in sipsocket)
            {
                for (int i = 0; i < temp.resId.Length; i++ )
                {
                    if (temp.resId[i].Equals(resId))
                    {
                        return temp;
                    }
                }  
            }
            return null;
        }

        //public static SipSocket FindSipSocket(string id)
        //{
        //    foreach (SipSocket temp in sipsocket)
        //    {
        //        if (temp.sip.Id.Equals(id))
        //        {
        //            return temp;
        //        }
        //        else
        //            return null;
        //    }
        //    return null;
        //}

        public static void SetResult(Socket socket, string[] result)
        {
            foreach (SipSocket temp in sipsocket)
            {
                if (temp.socket.Equals(socket))
                {
                    temp.result = new string[result.Length];
                    for (int i = 0; i < result.Length; i++)
                        temp.result[i] = result[i];
                }
                else
                    return;
            }
            return;
        }

        public static string[] GetResult(Socket socket)
        {
            string[] r;
            foreach (SipSocket temp in sipsocket)
            {
                if (temp.socket.Equals(socket))
                {
                    r = temp.result;
                    temp.result = null;
                    return r;
                }
                else
                    return null;
            }
            return null;
        }

        public static void CleanResult(Socket socket)
        {
            foreach (SipSocket temp in sipsocket)
            {
                if (temp.socket.Equals(socket))
                {
                    temp.result = null;
                    return;
                }
                else
                    return;
            }
            return;
        }

        public static int Send(Socket socket, XmlDocument doc)
        {
            return socket.Send(Encoding.GetEncoding("GB2312").GetBytes(FindSip(socket).SIPRequest(doc)));
        }
    }
}
