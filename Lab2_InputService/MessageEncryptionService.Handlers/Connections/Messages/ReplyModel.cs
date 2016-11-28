using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessageEncryptionService.Handlers.Connections.Types;

namespace MessageEncryptionService.Handlers.Connections.Messages
{
    public class ReplyModel: MessageModel
    {
        /// <summary>
        /// The type of a message, for which this reply was made.
        /// </summary>
        public MessageTypes ReplyType { get; set; }
        public ReplyModel(MessageTypes replyType) : base(MessageTypes.Reply)
        {
            ReplyType = replyType;
        }

        public override object Clone()
        {
            ReplyModel reply = (ReplyModel)base.Clone();
            reply.ReplyType = this.ReplyType;
            return reply;
        }
    }
}
