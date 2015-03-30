using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;

namespace TrainStationServer
{
    class SipSocket
    {
        public Socket socket;
        SIPTools sip;
        //string tcpIp, tcpPort, sessionId;
        private string[] result;

        private SipSocket()
        {

        }

        public SipSocket(Socket skt)
        {
            socket = skt;
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
