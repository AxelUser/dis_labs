using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MessageEncryptionService.Handlers.Connections
{
    public class SocketConnection
    {
        private IPEndPoint ipEndPoint;
        private Socket socket;
        public SocketConnection(string host, int port, int connectionsCount)
        {
            IPHostEntry ipHost = Dns.GetHostEntry(host);
            IPAddress ipAddr = ipHost.AddressList.First(); ;
            ipEndPoint = new IPEndPoint(ipAddr, port);
            socket = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        }


        public byte[] Receive()
        {
            throw new NotImplementedException();
        }

        public void Send(byte[] message)
        {
            throw new NotImplementedException();
        }
    }
}
