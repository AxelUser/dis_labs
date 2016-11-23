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
    class MessageQueueServer : ServerConnectionBase
    {
        RabbitMQ.Client.ConnectionFactory mqConnectionFactory;
        IConnection mqConnection;
        IModel channel;
        EventingBasicConsumer mqConsumer;
        string rpcQueueName;
        string consumerTag;

        public MessageQueueServer(): base()
        {
            
        }

        public override void DisconnectClient(Guid client)
        {
            throw new NotImplementedException();
        }

        public override void StartServer()
        {
            InitializeConnection();
            InitializeQueue(channel);
            InitializeConsumer(channel);
            consumerTag = channel.BasicConsume(rpcQueueName, false, mqConsumer);
        }

        public override void StopServer()
        {
            channel.BasicCancel(consumerTag);
            mqConnection.Close();
        }

        private void InitializeConnection()
        {
            rpcQueueName = "rpc_queue";
            mqConnectionFactory = new RabbitMQ.Client.ConnectionFactory()
            {
                UserName = "guest",
                Password = "guest",
                VirtualHost = "/",
                HostName = "localhost",
                Port = 5672
            };
            mqConnection = mqConnectionFactory.CreateConnection();
            channel = mqConnection.CreateModel();
        }

        private void InitializeQueue(IModel channel)
        {
            channel.QueueDeclare(
                queue: rpcQueueName,
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);
            channel.BasicQos(0, 1, false);
        }

        private void InitializeConsumer(IModel channel)
        {
            mqConsumer = new EventingBasicConsumer(channel);

            mqConsumer.Received += (sender, ea) =>
            {
                string requestBody = Encoding.UTF8.GetString(ea.Body);
                var requestProps = ea.BasicProperties;
                MessageModel requestModel = MessageCustomXmlConverter.ToModel(requestBody);

                ReplyModel responseModel = MessageRouting(ref requestModel, requestModel.SenderId);
                var responseProps = channel.CreateBasicProperties();
                responseProps.CorrelationId = requestProps.CorrelationId;
                byte[] responseBody = Encoding.UTF8.GetBytes(MessageCustomXmlConverter.ToXml(responseModel));

                channel.BasicPublish(exchange:"",
                    routingKey: requestProps.ReplyTo,
                    basicProperties: responseProps,
                    body: responseBody);
                channel.BasicAck(ea.DeliveryTag, false);
            };

        }
    }
}
