using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessageEncryptionService.Handlers.Connections.Messages;
using System.Net.Sockets;
using MessageEncryptionService.Handlers.Data;

namespace MessageEncryptionService.Handlers.Connections
{
    public abstract class ServerConnectionBase: IMessageReceiver
    {
        public event EventHandler<MessageModel> NewMessage;
        public event EventHandler<Exception> ConnectionError;
        protected MessageEncryptionHandler encryptionHandler;

        public abstract void StartServer();
        public abstract void StopServer();
        public abstract void DisconnectClient(Guid client);

        protected IProgress<MessageModel> onNewMessageHandler;
        public Guid ServerId { get; private set; }

        public ServerConnectionBase()
        {
            ServerId = Guid.NewGuid();
            onNewMessageHandler = new Progress<MessageModel>(m => NewMessage?.Invoke(this, m));
            encryptionHandler = new MessageEncryptionHandler(new AsymmetricEncryptionHandler());
        }

        public ReplyModel MessageRouting(ref MessageModel message, Guid sender, bool notify = true)
        {            
            if (message.IsBodyEncrypted)
            {
                message = encryptionHandler.DecryptMessage(message);
            }
            if (message.MessageType == Types.MessageTypes.Reply)
            {
                //Обработка ответов. Пока не придумал.
                return null;
            }
            else
            {
                ReplyModel response;
                //Обработка запросов.
                switch (message.MessageType)
                {
                    case Types.MessageTypes.AskRSAKey:
                        response = SendRSAKey(sender);
                        break;
                    case Types.MessageTypes.CloseConnection:
                        DisconnectClient(sender);
                        response = new ReplyModel(Types.MessageTypes.CloseConnection)
                        {
                            IsBodyEncrypted = false,
                            Body = "Connection closing confirmed."
                        };
                        break;
                    case Types.MessageTypes.SendData:
                        AddData(message, sender);
                        response = new ReplyModel(Types.MessageTypes.SendData)
                        {
                            IsBodyEncrypted = false,
                            Body = "Data is adding."
                        };
                        break;
                    default:
                        return null;
                }
                if(notify)
                {
                    OnNewMessage(message);
                }     
                response.TicketId = message.TicketId;
                response.SenderId = ServerId;
                return response;
            }
        }

        public ReplyModel SendRSAKey(Guid client)
        {
            return new ReplyModel(Types.MessageTypes.AskRSAKey)
            {
                Body = encryptionHandler.GetPublicAsymKey()
            };
        }

        public async void AddData(MessageModel message, Guid client)
        {
            var data = DataTransformHandler.FromXML(message.Body);
            await DataTransformHandler.SaveToDb(data);
        }

        protected void OnNewMessage(MessageModel message)
        {
            onNewMessageHandler.Report(message);
        }
    }
}
