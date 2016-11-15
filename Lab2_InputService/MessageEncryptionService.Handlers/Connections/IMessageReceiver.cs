using MessageEncryptionService.Handlers.Connections.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageEncryptionService.Handlers.Connections
{
    public interface IMessageReceiver
    {
        event EventHandler<MessageModel> NewMessage;
        event EventHandler<Exception> ConnectionError;
    }
}
