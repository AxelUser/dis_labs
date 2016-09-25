using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;

namespace MessageEncryptionService.Handlers.Connections.Sockets
{
    public class SocketClient : IClientConnection
    {
        private TcpClient client;
        string domain;
        int port;

        public SocketClient(string domain, int port)
        {
            IPAddress ipAdress = Dns.GetHostAddresses(domain).First();
            client = new TcpClient();
            this.domain = domain;
            this.port = port;
        }
        public bool Connect()
        {
            try
            {
                IPAddress ipAdress = Dns.GetHostAddresses(domain).First();
                client.Connect(domain, port);
                return true;
            }
            catch
            {
                return false;
            }
        }
        public byte[] Receive()
        {
            string data = "";
            using(var socketStream = client.GetStream())
            {
                using(BinaryReader reader = new BinaryReader(socketStream))
                {
                    data = reader.ReadString();
                }
            }
            return Encoding.Unicode.GetBytes(data);
        }
        public void Send(byte[] message)
        {
            using (var socketStream = client.GetStream())
            {
                using (BinaryWriter writer = new BinaryWriter(socketStream))
                {
                    string data = Encoding.Unicode.GetString(message);
                    writer.Write(data);
                }
            }
        }
    }
}
