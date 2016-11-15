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
    public class SocketClient : ClientConnectionBase, IMessageReceiver
    {
        #region Параметры и конструктор.
        private TcpClient client;
        private IPAddress ipAdress;      
        int port;

        //public event EventHandler<MessageModel> NewMessage;
        //public event EventHandler<Exception> ConnectionError;

        public SocketClient(string ip, int port)
        {
            clientId = Guid.NewGuid();
            ipAdress = IPAddress.Parse(ip);
            var s = ipAdress.ToString();
            client = new TcpClient();
            this.port = port;            
        }
        #endregion
        public override bool Connect()
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

        public override bool CheckConnection()
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

        public override MessageModel Send(MessageModel message, bool encrypted = true)
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

                        MessageModel request = PrepareMessage(message, encrypted);

                        writer.Write(MessageCustomXmlConverter.ToXml(request));
                        writer.Flush();
                        response = MessageCustomXmlConverter.ToModel(reader.ReadString()); //нужно сделать асинхронный вызов
                    }
                    catch(Exception e)
                    {
                        OnConnectionError(this, e);
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

        public override void Disconnect()
        {
            client.Close();
            Connected = false;
        }

        public override void AskAsymKey()
        {
            MessageModel request = new MessageModel(Types.MessageTypes.AskRSAKey);
            MessageModel response = Send(request, false);            
            string key = response.Body;
            InitEncryptionHandler(key);
        }

        private void InitEncryptionHandler(string rsaKey)
        {
            encryptionHandler = new MessageEncryptionHandler(new Data.AsymmetricEncryptionHandler(rsaKey));
        }
    }
}
