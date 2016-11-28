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

        /// <summary>
        /// Format message`s content before sending it to the server.
        /// </summary>
        /// <param name="message">Original message.</param>
        /// <param name="encryprt">Encrypt message or not.</param>
        /// <returns>Formatted message.</returns>
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

        /// <summary>
        /// Container method to firing NewMessage event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="message"></param>
        protected virtual void OnNewMessage(object sender, MessageModel message)
        {
            NewMessage?.Invoke(sender, message);
        }

        /// <summary>
        /// Container method to firing ConnectionError event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="exception"></param>
        protected virtual void OnConnectionError(object sender, Exception exception)
        {
            ConnectionError?.Invoke(sender, exception);
        }

        /// <summary>
        /// Flag, indicating whether connection to the server established or not.
        /// </summary>
        public bool Connected { get; set; }

        /// <summary>
        /// Check connection to the server.
        /// </summary>
        /// <returns></returns>
        public abstract bool CheckConnection();

        /// <summary>
        /// Connect to the server.
        /// </summary>
        /// <returns>Is connection established or not.</returns>
        public abstract bool Connect();

        /// <summary>
        /// Close connection to the server.
        /// </summary>
        public abstract void Disconnect();

        /// <summary>
        /// Send message to the server and wait for reply.
        /// </summary>
        /// <param name="message">Request to the server.</param>
        /// <param name="encrypted">Encrypt message or not.</param>
        /// <returns>Server`s response.</returns>
        public abstract Task<ReplyModel> SendAsync(MessageModel message, bool encrypted = true);

        /// <summary>
        /// Ask server for public part of asymmetric key.
        /// </summary>
        /// <returns>Message with public key.</returns>
        public async Task<ReplyModel> AskAsymKeyAsync()
        {
            MessageModel request = new MessageModel(Types.MessageTypes.AskRSAKey)
            {
                SenderId = clientId
            };
            ReplyModel response = await SendAsync(request, false);
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
