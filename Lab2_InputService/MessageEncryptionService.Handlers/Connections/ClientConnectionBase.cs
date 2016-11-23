using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessageEncryptionService.Handlers.Connections.Messages;

namespace MessageEncryptionService.Handlers.Connections
{
    public abstract class ClientConnectionBase: IMessageReceiver
    {
        protected Guid clientId;
        protected MessageEncryptionHandler encryptionHandler;

        public event EventHandler<MessageModel> NewMessage;
        public event EventHandler<Exception> ConnectionError;

        public ClientConnectionBase()
        {
            clientId = Guid.NewGuid();
        }
        protected MessageModel PrepareMessage(MessageModel message, bool encryprt)
        {
            message.SenderId = clientId;

            MessageModel newMessage = (MessageModel)message.Clone();
            if (encryprt && encryptionHandler != null)
            {
                newMessage = encryptionHandler.EncryptMessage(newMessage);
            }
            return newMessage;
        }

        protected virtual void OnNewMessage(object sender, MessageModel message)
        {
            NewMessage?.Invoke(sender, message);
        }

        protected virtual void OnConnectionError(object sender, Exception exception)
        {
            ConnectionError?.Invoke(sender, exception);
        }

        public bool Connected { get; set; }
        public abstract bool CheckConnection();
        public abstract bool Connect();
        public abstract void Disconnect();
        public abstract ReplyModel Send(MessageModel message, bool encrypted = true);
        public ReplyModel AskAsymKey()
        {
            MessageModel request = new MessageModel(Types.MessageTypes.AskRSAKey);
            ReplyModel response = Send(request, false);
            string key = response.Body;
            InitEncryptionHandler(key);
            return response;
        }

        protected void InitEncryptionHandler(string rsaKey)
        {
            encryptionHandler = new MessageEncryptionHandler(new Data.AsymmetricEncryptionHandler(rsaKey));
        }
    }
}
