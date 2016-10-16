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

        public string RSADecryptToBase64(string dataInBase64)
        {           
            byte[] bytes = Convert.FromBase64String(dataInBase64);
            byte[] decrypted = rsaProvider.Decrypt(bytes, true);
            return Convert.ToBase64String(decrypted);
        }

        public string RSAEncryptToBase64(string dataInBase64)
        {
            byte[] bytes = Convert.FromBase64String(dataInBase64);
            byte[] encrypted = rsaProvider.Encrypt(bytes, true);
            return Convert.ToBase64String(encrypted);
        }

    }
}
