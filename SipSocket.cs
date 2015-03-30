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

        public int Send(XmlDocument doc)
        {
            return socket.Send(Encoding.GetEncoding("GB2312").GetBytes(sip.SIPRequest(doc)));
        }

        public IAsyncResult BeginAccept(AsyncCallback callback, object obj)
        {
            return socket.BeginAccept(callback, obj);
        }

        public SipSocket EndAccept(IAsyncResult ar)
        {
            SipSocket temp = new SipSocket();
            temp.socket = socket.EndAccept(ar);
            return temp;
        }

        public IAsyncResult BeginReceive(byte[] buffer,int offset,int size,SocketFlags socketflags,AsyncCallback callback, object obj)
        {
            return socket.BeginReceive(buffer, offset, size, socketflags, callback, obj);
        }
    }
}
