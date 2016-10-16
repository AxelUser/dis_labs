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
        public MessageEncryptionHandler(AsymmetricEncryptionHandler asym)
        {
            asymHandler = asym;
            symHandler = new SymmetricEncryptionHandler();
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
            newMessage.DESIV = asymHandler.RSAEncrypt(symHandler.IVInBase64);
            newMessage.DESKey = asymHandler.RSAEncrypt(symHandler.KeyInBase64);
            newMessage.Body = symHandler.DESEncrypt(message.Body);
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
            symHandler = new SymmetricEncryptionHandler(newMessage.DESIV, newMessage.DESKey);
            newMessage.Body = symHandler.DESDecrypt(message.Body);
            return newMessage;
        }
    }
}
