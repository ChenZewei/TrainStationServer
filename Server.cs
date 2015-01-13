using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;


namespace TrainStationServer
{
    class Server
    {
        private Socket socket,client ;
        private IPEndPoint ipEnd;
        public Server()
        {
            ipEnd = new IPEndPoint(IPAddress.Any, 8888);
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Bind(ipEnd);
            socket.Listen(20);
        }

        public Socket Accept()
        {
            client = socket.Accept();
            return client;
        }
    }
}
