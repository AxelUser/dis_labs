using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageEncryptionService.Handlers.Connections
{
    public interface IServerConnection
    {
        event EventHandler<MessageModel> NewMessage;
        MessageModel ReceiveNewMessage();
        void SendRSAKey();
        void StartServer();
        void StopServer();
    }
}
