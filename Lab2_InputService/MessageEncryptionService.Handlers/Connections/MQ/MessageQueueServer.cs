using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessageEncryptionService.Handlers.Connections.Messages;

namespace MessageEncryptionService.Handlers.Connections.MQ
{
    class MessageQueueServer : ServerConnectionBase
    {
        public override void DisconnectClient(Guid client)
        {
            throw new NotImplementedException();
        }

        public override MessageModel ReceiveNewMessage()
        {
            throw new NotImplementedException();
        }

        public override ReplyModel ReplyClient(Guid client, MessageModel request)
        {
            throw new NotImplementedException();
        }

        public override ReplyModel SendRSAKey(Guid client)
        {
            throw new NotImplementedException();
        }

        public override void StartServer()
        {
            throw new NotImplementedException();
        }

        public override void StopServer()
        {
            throw new NotImplementedException();
        }
    }
}
