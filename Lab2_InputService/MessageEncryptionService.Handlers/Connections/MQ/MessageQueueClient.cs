using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessageEncryptionService.Handlers.Connections.Messages;
using RabbitMQ.Client;

namespace MessageEncryptionService.Handlers.Connections.MQ
{
    class MessageQueueClient : IClientConnection
    {
        RabbitMQ.Client.ConnectionFactory mqConnectionFactory;
        IConnection mqConnection;
        IModel inputChannel;
        IModel outputChannel;
        public MessageQueueClient(string inputQueueName, string outputQueueName)
        {
            mqConnectionFactory = new RabbitMQ.Client.ConnectionFactory()
            {
                UserName = "guest",
                Password = "guest",
                VirtualHost = "/",
                HostName = "localhost",
                Port = 5672
            };
        }
        public bool Connected
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public event EventHandler<Exception> ConnectionErrorRised;

        public void AskAsymKey()
        {
            throw new NotImplementedException();
        }

        public bool CheckConnection()
        {
            return true;
        }

        public bool Connect()
        {
            try
            {
                mqConnection = mqConnectionFactory.CreateConnection();
                Connected = true;
            }
            catch
            {
                Connected = false;
            }
            return Connected;
        }

        public void Disconnect()
        {
            mqConnection.Close();
        }

        public MessageModel Send(MessageModel message, bool encrypted = true)
        {
            throw new NotImplementedException();
        }
    }
}
