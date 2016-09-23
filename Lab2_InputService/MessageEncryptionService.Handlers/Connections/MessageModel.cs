using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageEncryptionService.Handlers.Connections
{
    public class MessageModel
    {
        public byte[] KeyDES { get; set; }
        public byte[] EncryptedBody { get; set; }
    }
}
