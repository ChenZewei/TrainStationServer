using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace TrainStationServer
{
    class SocketBound
    {
        private class SipSocket
        {
            public Socket socket;
            public SIPTools sip;
            public string[] result;
        }
        private static List<SipSocket> sipsocket = new List<SipSocket>();

        public static void Add(Socket socket,SIPTools sip)
        {
            foreach(SipSocket ss in sipsocket)
            {
                if (ss.socket.Equals(socket))
                    return;
            }
            SipSocket temp = new SipSocket(); ;
            temp.socket = socket;
            temp.sip = sip;
            sipsocket.Add(temp);
        }

        public static Socket FindSocket(string id)
        {
            foreach (SipSocket temp in sipsocket)
            {
                if (temp.sip.Id.Equals(id))
                {
                    return temp.socket;
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

        public static void CleanResult(Socket socket)
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
    }
}
