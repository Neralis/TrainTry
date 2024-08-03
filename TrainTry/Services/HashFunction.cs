using System.Security.Cryptography;
using System.Text;

namespace TrainTry.Services
{
    public class HashFunction
    {
        private static readonly string Salt = "savdgyanj12hdbas";
        private static readonly string AesKeyBase64 = "AAECAwQFBgcICQoLDA0ODw==";

        public static (string EncryptedPassword, string IV) EncryptPassword(string password)
        {
            password += Salt;

            byte[] hash = SHA256.HashData(Encoding.UTF8.GetBytes(password));
            StringBuilder sb = new StringBuilder();
            foreach (byte b in hash)
            {
                sb.Append(b.ToString("X2"));
            }

            using Aes myAes = Aes.Create();
            myAes.Key = Convert.FromBase64String(AesKeyBase64);
            string encryptedPassword = Convert.ToBase64String(EncryptStringToBytes_Aes(sb.ToString(), myAes.Key, myAes.IV));

            return (encryptedPassword, Convert.ToBase64String(myAes.IV));
        }

        public static bool VerifyPassword(string inputPassword, string storedPassword, string storedIV)
        {
            inputPassword += Salt;
            byte[] hash = SHA256.HashData(Encoding.UTF8.GetBytes(inputPassword));
            StringBuilder sb = new StringBuilder();
            foreach (byte b in hash)
            {
                sb.Append(b.ToString("X2"));
            }

            using Aes myAes = Aes.Create();
            myAes.Key = Convert.FromBase64String(AesKeyBase64);
            myAes.IV = Convert.FromBase64String(storedIV);
            string encryptedInputPassword = Convert.ToBase64String(EncryptStringToBytes_Aes(sb.ToString(), myAes.Key, myAes.IV));

            return encryptedInputPassword == storedPassword;
        }

        private static byte[] EncryptStringToBytes_Aes(string plainText, byte[] Key, byte[] IV)
        {
            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException(nameof(plainText));
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException(nameof(Key));
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException(nameof(IV));

            using Aes aesAlg = Aes.Create();
            aesAlg.Key = Key;
            aesAlg.IV = IV;

            ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

            using MemoryStream msEncrypt = new MemoryStream();
            using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
            using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
            {
                swEncrypt.Write(plainText);
            }

            return msEncrypt.ToArray();
        }
    }
}
