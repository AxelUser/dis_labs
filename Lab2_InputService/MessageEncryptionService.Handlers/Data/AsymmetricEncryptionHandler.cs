using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MessageEncryptionService.Handlers.Data
{
    public class AsymmetricEncryptionHandler
    {
        RSACryptoServiceProvider rsaProvider;

        public AsymmetricEncryptionHandler(string rsaKey = null)
        {
            rsaProvider = new RSACryptoServiceProvider();
            if (!string.IsNullOrEmpty(rsaKey))
            {
                rsaProvider.FromXmlString(rsaKey);
            }
        }

        public string RSAPublicKey
        {
            get
            {
                return rsaProvider?.ToXmlString(false);
            }
        }

        public void GenerateRSAKeys()
        {
            rsaProvider = new RSACryptoServiceProvider();
        }

        public string RSADecrypt(string data)
        {
           byte[] bytes = Encoding.Unicode.GetBytes(data);
           return Convert.ToBase64String(rsaProvider.Decrypt(bytes, true));
        }

        public string RSAEncrypt(string data)
        {
            byte[] bytes = Convert.FromBase64String(data);
            return Encoding.Unicode.GetString(rsaProvider.Encrypt(bytes, true));
        }

    }
}
