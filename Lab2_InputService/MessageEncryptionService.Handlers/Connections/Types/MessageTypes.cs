using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageEncryptionService.Handlers.Connections.Types
{
    public enum MessageTypes
    {
        AskRSAKey,
        SendData,
        Reply,
        CloseConnection
    }
}
