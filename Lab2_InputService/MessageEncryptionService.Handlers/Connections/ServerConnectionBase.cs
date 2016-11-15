using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessageEncryptionService.Handlers.Connections.Messages;
using System.Net.Sockets;

namespace MessageEncryptionService.Handlers.Connections
{
    public abstract class ServerConnectionBase: IMessageReceiver
    {
        public event EventHandler<MessageModel> NewMessage;
        public event EventHandler<Exception> ConnectionError;

        public abstract void StartServer();
        public abstract void StopServer();
        public abstract void DisconnectClient(Guid client);
        public abstract ReplyModel SendRSAKey(Guid client);
        public abstract ReplyModel ReplyClient(Guid client, MessageModel request);
        public abstract MessageModel ReceiveNewMessage();

        protected IProgress<MessageModel> onNewMessageHandler;
        protected Guid serverId;

        public ServerConnectionBase()
        {
            onNewMessageHandler = new Progress<MessageModel>(m => NewMessage?.Invoke(this, m));
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

        public void AddData(MessageModel message, Guid client)
        {

        }

        protected void OnNewMessage(MessageModel message)
        {
            onNewMessageHandler.Report(message);
        }
    }
}
