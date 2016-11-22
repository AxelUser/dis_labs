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
        protected Guid serverId;

        public ServerConnectionBase()
        {
            onNewMessageHandler = new Progress<MessageModel>(m => NewMessage?.Invoke(this, m));
            encryptionHandler = new MessageEncryptionHandler(new AsymmetricEncryptionHandler());
        }

        public ReplyModel MessageRouting(MessageModel message, Guid sender)
        {
            if(message.MessageType == Types.MessageTypes.Reply)
            {
                //Обработка ответов. Пока не придумал.
                return null;
            }
            else
            {
                //Обработка запросов.
                switch (message.MessageType)
                {
                    case Types.MessageTypes.AskRSAKey:
                        return SendRSAKey(sender);
                    case Types.MessageTypes.CloseConnection:
                        DisconnectClient(sender);
                        return new ReplyModel(Types.MessageTypes.CloseConnection)
                        {
                            SenderId = sender,
                            IsBodyEncrypted = false,
                            Body = "Завершение соединения подтверждено."
                        };
                    case Types.MessageTypes.SendData:
                        AddData(message, sender);
                        return new ReplyModel(Types.MessageTypes.SendData)
                        {
                            SenderId = sender,
                            IsBodyEncrypted = false,
                            Body = "Данные добавлены."
                        };
                    default: return null;
                }
            }
        }

        public ReplyModel SendRSAKey(Guid client)
        {
            return new ReplyModel(Types.MessageTypes.AskRSAKey)
            {
                Body = encryptionHandler.GetPublicAsymKey()
            };
        }

        public void AddData(MessageModel message, Guid client)
        {

        }

        protected void OnNewMessage(MessageModel message)
        {
            onNewMessageHandler.Report(message);
        }
    }
}
