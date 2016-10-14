using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessageEncryptionService.Handlers.Connections.Messages;
using System.Net.Sockets;

namespace MessageEncryptionService.Handlers.Connections
{
    public abstract class ServerConnectionBase
    {
        public event EventHandler<MessageModel> NewMessage;
        public abstract void StartServer();
        public abstract void StopServer();
        public abstract void DisconnectClient(Guid client);
        public abstract void SendRSAKey(Guid client);
        public abstract void ReplyClient(Guid client);
        public abstract MessageModel ReceiveNewMessage();
        public void MessageRouting(MessageModel message, Guid sender)
        {
            if(message.MessageType == Types.MessageTypes.Reply)
            {
                //Обработка ответов.
            }
            else
            {
                //Обработка запросов.
                switch (message.MessageType)
                {
                    case Types.MessageTypes.AskRSAKey:
                        SendRSAKey(sender);
                        break;
                    case Types.MessageTypes.CloseConnection:
                        DisconnectClient(sender);
                        break;
                    case Types.MessageTypes.SendData:
                        AddData(message, sender);
                        break;
                }
            }
        }        

        public void AddData(MessageModel message, Guid client)
        {
            
        }

        protected void OnNewMessage(MessageModel message)
        {
            NewMessage?.Invoke(this, message);
        }
    }
}
