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
        public SymmetricEncryptionHandler(byte[] iv, byte[] key)
        {
            desProvider = new TripleDESCryptoServiceProvider();
            if(iv != null && key != null)
            {
                desProvider.IV = iv;
                desProvider.Key = key;
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
