using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageEncryptionService.Handlers.Connections
{
    public interface IClientConnection
    {
        event EventHandler<string> ConnectionErrorRised;
        bool CheckConnection();
        bool Connect();
        void Disconnect();
        MessageModel Send(MessageModel message);
    }
}
