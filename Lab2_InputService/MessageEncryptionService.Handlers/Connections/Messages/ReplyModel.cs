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
        public MessageTypes ReplyType { get; set; }
        public ReplyModel(MessageTypes replyType) : base(MessageTypes.Reply)
        {
            ReplyType = replyType;
        }
    }
}
