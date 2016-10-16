using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessageEncryptionService.Handlers.Connections.Messages;
using System.Messaging;

namespace MessageEncryptionService.Handlers.Connections.MQ
{
    class MessageQueueClient : IClientConnection
    {
        private MessageQueue queue;
        public MessageQueueClient(string queueName)
        {
            if (!MessageQueue.Exists(queueName))
            {
                MessageQueue.Create(queueName);
            }
            queue = new MessageQueue(queueName);
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
            throw new NotImplementedException();
        }

        public bool Connect()
        {
            throw new NotImplementedException();
        }

        public void Disconnect()
        {
            throw new NotImplementedException();
        }

        public MessageModel Send(MessageModel message, bool encrypted = true)
        {
            throw new NotImplementedException();
        }
    }
}
