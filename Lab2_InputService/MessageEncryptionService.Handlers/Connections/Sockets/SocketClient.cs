using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;
using MessageEncryptionService.Handlers.Connections.Messages;

namespace MessageEncryptionService.Handlers.Connections.Sockets
{
    public class SocketClient : IClientConnection
    {
        private TcpClient client;
        private IPAddress ipAdress;
        private Guid clientId;
        CancellationTokenSource cts;
        int port;

        Task checkingConnectionTask;

        public event EventHandler<string> ConnectionErrorRised;

        public SocketClient(string ip, int port)
        {
            clientId = Guid.NewGuid();
            ipAdress = IPAddress.Parse(ip);
            var s = ipAdress.ToString();
            client = new TcpClient();
            this.port = port;
            cts = new CancellationTokenSource();
        }

        public bool Connect()
        {
            bool connected;
            try
            {                
                client.Connect(ipAdress, port);                
                connected = true;
            }
            catch
            {
                connected = false;
            }
            return connected;
        }

        public bool CheckConnection()
        {
            bool connected = client.Connected && client.Client.Poll(1000, SelectMode.SelectRead);
            if (connected)
            {
                byte[] buff = new byte[1];
                connected = client.Client.Receive(buff, SocketFlags.Peek) != 0;
            }
            //return connected;
            return true; //пока оставлю заглушку
        }        

        public MessageModel Send(MessageModel message)
        {
            MessageModel response = null;
            if (CheckConnection())
            {
                var socketStream = client.GetStream();
                {
                    BinaryWriter writer = null;
                    BinaryReader reader = null;
                    try
                    {
                        writer = new BinaryWriter(socketStream, Encoding.UTF8, true);
                        reader = new BinaryReader(socketStream, Encoding.UTF8, true);
                        message.SenderId = clientId;
                        writer.Write(MessageCustomXmlConverter.ToXml(message));
                        writer.Flush();
                        response = MessageCustomXmlConverter.ToModel(reader.ReadString());
                    }
                    finally
                    {
                        writer.Close();
                        reader.Close();
                    }
                }
            }
            return response;
        }

        public void Disconnect()
        {
            cts.Cancel();
            checkingConnectionTask.Wait(TimeSpan.FromSeconds(10));
        }
    }
}
