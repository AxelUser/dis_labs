using MessageEncryptionService.Handlers.Connections.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageEncryptionService.Handlers.Connections.Messages
{
    public class MessageModel: ICloneable
    {
        public MessageTypes MessageType { get; private set; }
        public Guid SenderId { get; set; }
        public bool IsBodyEncrypted { get; set; }
        public string DESKey { get; set; }
        public string DESIV { get; set; }
        public string Body { get; set; }
        public MessageModel(MessageTypes type)
        {
            this.MessageType = type;
        }

        public virtual object Clone()
        {
            return new MessageModel(this.MessageType)
            {
                SenderId = this.SenderId,
                IsBodyEncrypted = this.IsBodyEncrypted,
                DESIV = this.DESIV,
                DESKey = this.DESKey,
                Body = this.Body
            };
        }
    }
}
