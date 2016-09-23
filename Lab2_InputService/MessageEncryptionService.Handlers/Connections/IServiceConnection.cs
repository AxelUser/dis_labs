using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageEncryptionService.Handlers.Connections
{
    public interface IServiceConnection
    {
        event EventHandler<MessageModel> NewMessage;
        void Send(MessageModel message);
        MessageModel Receive();
    }
}
