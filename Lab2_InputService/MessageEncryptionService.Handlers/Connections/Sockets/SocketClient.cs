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
        #region Параметры и конструктор.
        public event EventHandler<Exception> ConnectionErrorRised;
        private TcpClient client;
        private IPAddress ipAdress;
        private Guid clientId;        
        int port;
        public bool Connected { get; set; }
        public SocketClient(string ip, int port)
        {
            clientId = Guid.NewGuid();
            ipAdress = IPAddress.Parse(ip);
            var s = ipAdress.ToString();
            client = new TcpClient();
            this.port = port;
        }
        #endregion
        public bool Connect()
        {
            try
            {                
                client.Connect(ipAdress, port);                
                Connected = true;
            }
            catch
            {
                Connected = false;
            }
            return Connected;
        }

        public bool CheckConnection()
        {
            //bool connected = client.Connected && client.Client.Poll(1000, SelectMode.SelectRead);
            //if (connected)
            //{
            //    byte[] buff = new byte[1];
            //    connected = client.Client.Receive(buff, SocketFlags.Peek) != 0;
            //}
            //return connected;
            return Connected; //пока оставлю заглушку
        }        

        public MessageModel Send(MessageModel message, bool encrypted = true)
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
                    catch(Exception e)
                    {
                        ConnectionErrorRised?.Invoke(this, e);
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
            client.Close();
            Connected = false;
        }
    }
}
