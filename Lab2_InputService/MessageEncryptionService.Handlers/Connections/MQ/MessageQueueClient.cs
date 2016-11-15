using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessageEncryptionService.Handlers.Connections.Messages;
using RabbitMQ.Client;

namespace MessageEncryptionService.Handlers.Connections.MQ
{
    class MessageQueueClient : ClientConnectionBase
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

        public event EventHandler<Exception> ConnectionErrorRised;

        public override void AskAsymKey()
        {
            throw new NotImplementedException();
        }

        public override bool CheckConnection()
        {
            return true;
        }

        public override bool Connect()
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

        public override void Disconnect()
        {
            mqConnection.Close();
        }

        public override MessageModel Send(MessageModel message, bool encrypted = true)
        {
            throw new NotImplementedException();
        }
    }
}
