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
    }
}
