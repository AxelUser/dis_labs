using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessageEncryptionService.Handlers.Data;

namespace MessageEncryptionService.Handlers.Connections.Messages
{
    public class MessageEncryptionHandler
    {
        AsymmetricEncryptionHandler asymHandler;
        SymmetricEncryptionHandler symHandler;
        public MessageEncryptionHandler(AsymmetricEncryptionHandler asym, SymmetricEncryptionHandler sym)
        {
            asymHandler = asym;
            symHandler = sym;
        }

        public MessageModel EncryptMessage(MessageModel message)        
        {
            MessageModel newMessage;
            if (message is ReplyModel)
            {
                newMessage = new ReplyModel(((ReplyModel)message).ReplyType);
            }
            else
            {
                newMessage = new MessageModel(message.MessageType)
                {
                    SenderId = message.SenderId,
                    IsBodyEncrypted = message.IsBodyEncrypted
                };
            }            
            newMessage.DESIV = asymHandler.RSAEncrypt(message.DESIV);
            newMessage.DESKey = asymHandler.RSAEncrypt(message.DESKey);
            //newMessage.Body = 
            return newMessage;
        }

        public MessageModel DecryptMessage(MessageModel message)
        {
            MessageModel newMessage;
            if (message is ReplyModel)
            {
                newMessage = new ReplyModel(((ReplyModel)message).ReplyType);
            }
            else
            {
                newMessage = new MessageModel(message.MessageType)
                {
                    SenderId = message.SenderId,
                    IsBodyEncrypted = message.IsBodyEncrypted
                };
            }
            newMessage.DESIV = asymHandler.RSADecrypt(message.DESIV);
            newMessage.DESKey = asymHandler.RSADecrypt(message.DESKey);
            return newMessage;
        }
    }
}
