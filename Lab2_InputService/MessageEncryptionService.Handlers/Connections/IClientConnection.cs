using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessageEncryptionService.Handlers.Connections.Messages;

namespace MessageEncryptionService.Handlers.Connections
{
    public interface IClientConnection
    {
        event EventHandler<Exception> ConnectionErrorRised;
        bool Connected { get; set; }
        bool CheckConnection();
        bool Connect();
        void Disconnect();
        MessageModel Send(MessageModel message, bool encrypted = true);
    }
}
