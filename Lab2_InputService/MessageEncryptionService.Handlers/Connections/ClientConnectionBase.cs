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

        protected MessageModel PrepareMessage(MessageModel message, bool encryprt)
        {
            message.SenderId = clientId;

            MessageModel newMessage = (MessageModel)message.Clone();
            if (encryprt && encryptionHandler != null)
            {
                newMessage = encryptionHandler.DecryptMessage(newMessage);
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
        public abstract MessageModel Send(MessageModel message, bool encrypted = true);
        public void AskAsymKey()
        {
            MessageModel request = new MessageModel(Types.MessageTypes.AskRSAKey);
            MessageModel response = Send(request, false);
            string key = response.Body;
            InitEncryptionHandler(key);
        }

        protected void InitEncryptionHandler(string rsaKey)
        {
            encryptionHandler = new MessageEncryptionHandler(new Data.AsymmetricEncryptionHandler(rsaKey));
        }
    }
}
