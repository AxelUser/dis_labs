using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessageEncryptionService.Handlers.Connections.Messages;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace MessageEncryptionService.Handlers.Connections.MQ
{
    class MessageQueueClient : ClientConnectionBase
    {
        RabbitMQ.Client.ConnectionFactory mqConnectionFactory;
        IConnection mqConnection;
        IModel mqChannel;
        EventingBasicConsumer mqConsumer;
        string callbackQueue;
        Dictionary<Guid, MessageModel> pendingRequests;

        public MessageQueueClient()
        {
            pendingRequests = new Dictionary<Guid, MessageModel>();
        }

        public override bool CheckConnection()
        {
            return true;
        }

        public override bool Connect()
        {
            try
            {
                InitializeConnection();
                InitializeQueue(mqChannel);
                mqChannel.BasicConsume(callbackQueue, false, mqConsumer);
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
            mqChannel.BasicCancel(mqConsumer.ConsumerTag);
            mqConnection.Close();
        }

        public override MessageModel Send(MessageModel message, bool encrypted = true)
        {
            message.TicketId = Guid.NewGuid();
            MessageModel request = PrepareMessage(message, encrypted);
            byte[] requestBody = Encoding.UTF8.GetBytes(MessageCustomXmlConverter.ToXml(request));
            var requestProps = mqChannel.CreateBasicProperties();
            requestProps.CorrelationId = request.TicketId.ToString();
            mqChannel.BasicPublish(exchange: "",
                routingKey: callbackQueue,
                basicProperties: requestProps,
                body: requestBody);
            pendingRequests.Add((Guid)request.TicketId, null);
            while (true)
            {
                lock (pendingRequests)
                {
                    var response = pendingRequests[(Guid)request.TicketId];
                    if (response != null)
                    {
                        return response;
                    }
                }
            }
        }

        private void InitializeConnection()
        {
            mqConnectionFactory = new RabbitMQ.Client.ConnectionFactory()
            {
                UserName = "guest",
                Password = "guest",
                VirtualHost = "/",
                HostName = "localhost",
                Port = 5672
            };
            mqConnection = mqConnectionFactory.CreateConnection();
            mqChannel = mqConnection.CreateModel();
        }

        private void InitializeQueue(IModel channel)
        {
            callbackQueue = channel.QueueDeclare().QueueName;
            channel.BasicQos(0, 1, false);
        }

        private void InitializeConsumer(IModel channel)
        {
            mqConsumer = new EventingBasicConsumer(channel);

            mqConsumer.Received += (sender, ea) =>
            {
                string responseBody = Encoding.UTF8.GetString(ea.Body);
                MessageModel responseModel = MessageCustomXmlConverter.ToModel(responseBody);
                lock (pendingRequests)
                {
                    if (pendingRequests.ContainsKey((Guid)responseModel.TicketId))
                    {
                        pendingRequests[(Guid)responseModel.TicketId] = responseModel;
                    }
                }
                channel.BasicAck(ea.DeliveryTag, false);
            };
        }
    }
}
