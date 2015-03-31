using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Xml;

namespace TrainStationServer
{
    class SocketBound
    {
        //private class SipSocket
        //{
        //    public Socket socket;
        //    public SIPTools sip;
        //    public string[] result;
        //}
        private static List<SipSocket> sipsocket = new List<SipSocket>();

        public static void Add(Socket socket,SIPTools sip)
        {
            foreach(SipSocket ss in sipsocket)
            {
                if (ss.socket.Equals(socket))
                    return;
            }
            SipSocket temp = new SipSocket(socket,sip); 
            sipsocket.Add(temp);
        }

        public static Socket FindSocket(string prefix)//确认sipsocket列表中是否已经存在指定id的Socket
        {
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
            foreach (SipSocket temp in sipsocket)
            {
                if (temp.socket.Equals(socket))
                {
                    return temp.sip;
                }
                else
                    return null;
            }
            return null;
        }

        public static SipSocket FindSipSocket(Socket socket)
        {
            foreach (SipSocket temp in sipsocket)
            {
                if (temp.socket.Equals(socket))
                {
                    return temp;
                }
                else
                    return null;
            }
            return null;
        }

        public static SipSocket FindSipSocket(string id)//寻找sipsocket列表中与id相同的SipSocket对象
        {
            foreach (SipSocket temp in sipsocket)
            {
                if (temp.sip.Id.Equals(id))
                {
                    return temp;
                }
                else
                    return null;
            }
            return null;
        }

        public static void InsertResult(Socket socket, string[] result)
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

        public static void CleanResult(Socket socket)//清除
        {
            foreach (SipSocket temp in sipsocket)
            {
                if (temp.socket.Equals(socket))
                {
                    temp.result = null;
                    return ;
                }
                else
                    return;
            }
            return;
        }

        public static int Send(Socket socket, XmlDocument doc)
        {
            return socket.Send(Encoding.GetEncoding("GB2312").GetBytes(SocketBound.FindSip(socket).SIPRequest(doc)));
        }
    }
}
