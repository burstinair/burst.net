using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Burst.Crypt
{
    public static partial class CryptUtils
    {
        public static string EncryptDES(string encryptString)
        {
            return EncryptDES(encryptString, Settings.CryptRgbKey);
        }
        public static string EncryptDES(string encryptString, string key)
        {
            try
            {
                string encryptKey = key;
                byte[] rgbKey = Encoding.UTF8.GetBytes(encryptKey.Substring(0, 8));
                byte[] rgbIV = Settings.CryptRgbIV;
                byte[] inputByteArray = Encoding.UTF8.GetBytes(encryptString);
                DESCryptoServiceProvider dCSP = new DESCryptoServiceProvider();
                MemoryStream mStream = new MemoryStream();
                CryptoStream cStream = new CryptoStream(mStream, dCSP.CreateEncryptor(rgbKey, rgbIV), CryptoStreamMode.Write);
                cStream.Write(inputByteArray, 0, inputByteArray.Length);
                cStream.FlushFinalBlock();
                return System.Convert.ToBase64String(mStream.ToArray());
            }
            catch
            {
                return string.Empty;
            }
        }
        public static string DecryptDES(string encryptString)
        {
            return DecryptDES(encryptString, Settings.CryptRgbKey);
        }
        public static string DecryptDES(string decryptString, string key)
        {
            try
            {
                if (string.IsNullOrEmpty(decryptString))
                    return string.Empty;
                string decryptKey = key;
                byte[] rgbKey = Encoding.UTF8.GetBytes(decryptKey);
                byte[] rgbIV = Settings.CryptRgbIV;
                byte[] inputByteArray = System.Convert.FromBase64String(decryptString);
                DESCryptoServiceProvider DCSP = new DESCryptoServiceProvider();
                MemoryStream mStream = new MemoryStream();
                CryptoStream cStream = new CryptoStream(mStream, DCSP.CreateDecryptor(rgbKey, rgbIV), CryptoStreamMode.Write);
                cStream.Write(inputByteArray, 0, inputByteArray.Length);
                cStream.FlushFinalBlock();
                return Encoding.UTF8.GetString(mStream.ToArray());
            }
            catch
            {
                return string.Empty;
            }
        }
        public static string Encrypt(string encryptString, CryptType format)
        {
            switch (format)
            {
                default:
                case CryptType.MD5:
                    MD5 md5Hasher = MD5.Create();
                    byte[] md5data = md5Hasher.ComputeHash(Encoding.UTF8.GetBytes(encryptString));
                    StringBuilder md5sBuilder = new StringBuilder();
                    for (int i = 0; i < md5data.Length; i++)
                        md5sBuilder.Append(md5data[i].ToString("x2"));
                    return md5sBuilder.ToString();
                case CryptType.SHA1:
                    SHA1 sha1Hasher = SHA1.Create();
                    byte[] sha1data = sha1Hasher.ComputeHash(Encoding.UTF8.GetBytes(encryptString));
                    StringBuilder sha1sBuilder = new StringBuilder();
                    for (int i = 0; i < sha1data.Length; i++)
                        sha1sBuilder.Append(sha1data[i].ToString("x2"));
                    return sha1sBuilder.ToString();
                case CryptType.DES:
                    return EncryptDES(encryptString);
            }
        }
    }

    public enum CryptType
    {
        DES, MD5, SHA1
    }
}
