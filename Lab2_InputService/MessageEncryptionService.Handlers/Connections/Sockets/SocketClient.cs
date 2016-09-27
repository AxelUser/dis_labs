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
        private IPAddress ipAdress;
        int port;

        public SocketClient(string ip, int port)
        {
            ipAdress = IPAddress.Parse(ip);
            var s = ipAdress.ToString();
            client = new TcpClient();
            this.port = port;
        }
        public bool Connect()
        {
            try
            {
                client.Connect(ipAdress, port);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public MessageModel Send(MessageModel message)
        {
            MessageModel reply = null;
            if (Connect())
            {
                using(var socketStream = client.GetStream())
                {
                    BinaryWriter writer = null;
                    BinaryReader reader = null;
                    try
                    {
                        writer = new BinaryWriter(socketStream);
                        reader = new BinaryReader(socketStream);
                        string data = message.Body;
                        writer.Write(data);
                        writer.Flush();
                        reply = new MessageModel(Types.MessageTypes.Reply)
                        {
                            Body = reader.ReadString()
                        };
                    }
                    finally
                    {
                        writer.Close();
                        reader.Close();
                    }
                }
            }
            return reply;
        }        
    }
}
