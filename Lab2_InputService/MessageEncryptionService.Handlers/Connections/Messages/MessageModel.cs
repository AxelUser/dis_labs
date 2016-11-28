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
        /// <summary>
        /// Id for specific task. It helps clients to recieve relevant replies in their callbacks.
        /// </summary>
        public Guid TicketId { get; set; }
        
        /// <summary>
        /// Type of message.
        /// </summary>
        public MessageTypes MessageType { get; private set; }

        /// <summary>
        /// Id of sender (client or server).
        /// </summary>
        public Guid SenderId { get; set; }

        /// <summary>
        /// Flag, indicating whether message content-body is encrypted or not.
        /// </summary>
        public bool IsBodyEncrypted { get; set; }

        /// <summary>
        /// Assymetrical key.
        /// </summary>
        public string DESKey { get; set; }

        /// <summary>
        /// Assymetrical initiatiational vector.
        /// </summary>
        public string DESIV { get; set; }

        /// <summary>
        /// Message`s content-body.
        /// </summary>
        public string Body { get; set; }

        public MessageModel(MessageTypes type)
        {
            this.MessageType = type;
            TicketId = Guid.NewGuid();
        }

        public virtual object Clone()
        {
            return new MessageModel(this.MessageType)
            {
                TicketId = this.TicketId,
                SenderId = this.SenderId,
                IsBodyEncrypted = this.IsBodyEncrypted,
                DESIV = this.DESIV,
                DESKey = this.DESKey,
                Body = this.Body
            };
        }
    }
}
