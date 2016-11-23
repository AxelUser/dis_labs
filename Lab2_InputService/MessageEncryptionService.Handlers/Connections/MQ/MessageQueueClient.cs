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
        string rpcQueueName = "rpc_queue";
        Dictionary<Guid, ReplyModel> pendingRequests;

        public MessageQueueClient()
        {
            pendingRequests = new Dictionary<Guid, ReplyModel>();
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
                InitializeConsumer(mqChannel);
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

        public override async Task<ReplyModel> Send(MessageModel message, bool encrypted = true)
        {
            MessageModel request = PrepareMessage(message, encrypted);
            byte[] requestBody = Encoding.UTF8.GetBytes(MessageCustomXmlConverter.ToXml(request));
            var requestProps = mqChannel.CreateBasicProperties();
            requestProps.CorrelationId = request.TicketId.ToString();
            requestProps.ReplyTo = callbackQueue;
            await Task.Run(() =>
            {
                mqChannel.BasicPublish(exchange: "",
                    routingKey: rpcQueueName,
                    basicProperties: requestProps,
                    body: requestBody);
            });
            pendingRequests.Add(request.TicketId, null);
            return await Task.Run(() =>
            {
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
            });
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

            mqConsumer.Received += (ch, ea) =>
            {
                string responseBody = Encoding.UTF8.GetString(ea.Body);
                ReplyModel responseModel = (ReplyModel)MessageCustomXmlConverter.ToModel(responseBody);
                lock (pendingRequests)
                {
                    if (pendingRequests.ContainsKey(responseModel.TicketId))
                    {
                        pendingRequests[responseModel.TicketId] = responseModel;
                    }
                }
                ((EventingBasicConsumer)ch).Model.BasicAck(ea.DeliveryTag, false);
            };
        }
    }
}
