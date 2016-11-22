using MessageEncryptionService.Handlers.Connections.Messages;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageEncryptionService.Handlers.Connections.MQ
{
    public class MQManager
    {
        RabbitMQ.Client.ConnectionFactory mqConnectionFactory;
        IConnection mqConnection;
        IModel inputChannel;
        IModel outputChannel;

        public event EventHandler<MessageModel> NewMessageReceived;

        public MQManager(string sendQueue, string receiveQueue)
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

        public void Send(MessageModel message)
        {

        }

        public MessageModel Receive()
        {
            return null;
        }
    }
}
