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
                newMessage = message.Clone() as ReplyModel;
            }
            else
            {
                newMessage = message.Clone() as MessageModel;
            }
            newMessage.IsBodyEncrypted = true;
            newMessage.DESIV = asymHandler.RSAEncryptToBase64(symHandler.IVInBase64);
            newMessage.DESKey = asymHandler.RSAEncryptToBase64(symHandler.KeyInBase64);
            //newMessage.DESIV = symHandler.IVInBase64;
            //newMessage.DESKey = symHandler.KeyInBase64;
            newMessage.Body = symHandler.DESEncrypt(message.Body);
            return newMessage;
        }

        public MessageModel DecryptMessage(MessageModel message)
        {
            MessageModel newMessage;
            if (message is ReplyModel)
            {
                newMessage = message.Clone() as ReplyModel;
            }
            else
            {
                newMessage = message.Clone() as MessageModel;
            }
            newMessage.IsBodyEncrypted = false;
            newMessage.DESIV = asymHandler.RSADecryptToBase64(message.DESIV);
            newMessage.DESKey = asymHandler.RSADecryptToBase64(message.DESKey);
            symHandler = new SymmetricEncryptionHandler(newMessage.DESIV, newMessage.DESKey);
            newMessage.Body = symHandler.DESDecrypt(message.Body);
            return newMessage;
        }

        public string GetPublicAsymKey()
        {
            return asymHandler.RSAPublicKey;
        }
    }
}
