using MessageEncryptionService.Handlers.Connections.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageEncryptionService.Handlers.Connections
{
    public class MessageModel
    {
        public MessageTypes Type { get; private set; }
        public string Body { get; set; }
        public MessageModel(MessageTypes type)
        {
            this.Type = type;
        }
    }
}
