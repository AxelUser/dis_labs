using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MessageEncryptionService.Handlers.Data
{
    public class SymmetricEncryptionHandler
    {
        TripleDESCryptoServiceProvider desProvider;
        public SymmetricEncryptionHandler()
        {
            desProvider = new TripleDESCryptoServiceProvider();
        }

        public SymmetricEncryptionHandler(string ivBase64 = null, string keyBase64 = null)
        {
            desProvider = new TripleDESCryptoServiceProvider();
            if (ivBase64 != null && keyBase64 != null)
            {
                desProvider.IV = Convert.FromBase64String(ivBase64);
                desProvider.Key = Convert.FromBase64String(keyBase64);
            }
        }

        public string IVInBase64
        {
            get
            {
                return Convert.ToBase64String(desProvider.IV);
            }
        }

        public string KeyInBase64
        {
            get
            {
                return Convert.ToBase64String(desProvider.Key);
            }
        }

        public string DESEncrypt(string data)
        {
            byte[] bytes = Encoding.Unicode.GetBytes(data);
            return Convert.ToBase64String(desProvider.CreateEncryptor()
                .TransformFinalBlock(bytes, 0, bytes.Length));
        }

        public string DESDecrypt(string data)
        {
            byte[] bytes = Encoding.Unicode.GetBytes(data);
            return Convert.ToBase64String(desProvider.CreateDecryptor()
                .TransformFinalBlock(bytes, 0, bytes.Length));
        }


    }
}
