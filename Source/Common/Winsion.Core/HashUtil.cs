using System;
using System.Text;
using System.Security.Cryptography;
using System.IO;



namespace Winsion.Core
{
    public static class HashUtil
    {
        static HashUtil()
        {

        }


        #region Md5Hash
        /// <summary>
        /// get Md5Hash Bytes for your string 
        /// </summary>
        /// <param name="valueToHash"></param>        
        /// <returns></returns>
        public static byte[] GetMd5HashBytesFor(string valueToHash)
        {
            MD5CryptoServiceProvider md5Hasher = new MD5CryptoServiceProvider();
            Byte[] hashedBytes;
            UTF8Encoding encoder = new UTF8Encoding();
            hashedBytes = md5Hasher.ComputeHash(encoder.GetBytes(valueToHash));
            return hashedBytes;
        }
        
        /// <summary>
        /// get Md5Hash for your string
        /// </summary>
        /// <param name="valueToHash"></param>        
        /// <returns></returns>
        public static string GetMd5HashFor(string valueToHash)
        {
            Byte[] hashedBytes = GetMd5HashBytesFor(valueToHash);

            StringBuilder sBuilder = new StringBuilder();
            for (int i = 0; i < hashedBytes.Length; i++)
            {
                sBuilder.Append(hashedBytes[i].ToString("x2"));
            }

            return sBuilder.ToString();
        }

        public static string GetMd5HashFor(string valueToHash, string salt)
        {
            return GetMd5HashFor(valueToHash + salt);
        }

        /// <summary>
        /// get Md5Hash for your string and salt
        /// </summary>
        /// <param name="valueToHash"></param>
        /// <param name="salt"></param>
        /// <returns></returns>
        public static string GetMd5HashFor(string valueToHash, Guid salt)
        {
            return GetMd5HashFor(valueToHash, salt.ToString());
        }

        /// <summary>
        /// get Base64 Md5Hash for your string
        /// </summary>
        /// <param name="valueToHash"></param>        
        /// <returns></returns>
        public static string GetBase64Md5HashFor(string valueToHash)
        {
            return ToBase64String(GetMd5HashFor(valueToHash));
        }

        public static string GetBase64Md5HashFor(string valueToHash, string salt)
        {
            return GetBase64Md5HashFor(valueToHash, salt);
        }

        /// <summary>
        /// get Base64 Md5Hash for your string and salt
        /// </summary>
        /// <param name="valueToHash"></param>
        /// <param name="salt"></param>
        /// <returns></returns>
        public static string GetBase64Md5HashFor(string valueToHash, Guid salt)
        {
            return GetBase64Md5HashFor(valueToHash, salt.ToString());
        }
        #endregion

        public static bool VerifyMd5Hash(string input, string md5Hash)
        {
            if (input == null || string.IsNullOrEmpty(md5Hash))
                return false;

            // Hash the input.
            string hashOfInput = GetMd5HashFor(input);

            // Create a StringComparer an compare the hashes.
            StringComparer comparer = StringComparer.OrdinalIgnoreCase;

            if (0 == comparer.Compare(hashOfInput, md5Hash))
                return true;
            else
                return false;
        }

        public static bool VerifyMd5Hash(string input, string salt, string md5Hash)
        {
            return VerifyMd5Hash(input + salt, md5Hash);
        }

        public static bool VerifyMd5Hash(string input, Guid salt, string md5Hash)
        {
            return VerifyMd5Hash(input, salt.ToString(), md5Hash);
        }



        public static bool VerifyBase64Md5Hash(string input, Guid salt, string base64Md5Hash)
        {
            var hash = FromBase64String(base64Md5Hash);
            return VerifyMd5Hash(input, salt, hash);
        }


        #region Encrypt
        private static byte[] IV = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };
        private static string tempKey = "12345678";
        public static String Encrypt(String Key, String ClearText)
        {
            try
            {
                var myKey = Key + tempKey;
                byte[] rgbKey = Encoding.UTF8.GetBytes(myKey.Substring(0, 8));
                byte[] rgbIV = IV;
                byte[] clearTextArray = Encoding.UTF8.GetBytes(ClearText);

                DESCryptoServiceProvider dCSP = new DESCryptoServiceProvider();
                using (MemoryStream mStream = new MemoryStream())
                {
                    CryptoStream cStream = new CryptoStream(mStream, dCSP.CreateEncryptor(rgbKey, rgbIV), CryptoStreamMode.Write);
                    cStream.Write(clearTextArray, 0, clearTextArray.Length);
                    cStream.FlushFinalBlock();

                    cStream.Close();

                    return Convert.ToBase64String(mStream.ToArray());
                }
            }
            catch
            {
                return string.Empty;
            }
        }


        public static String Decrypt(String Key, String DecryptText)
        {
            if (string.IsNullOrEmpty(DecryptText))
                return string.Empty;
            try
            {
                var myKey = Key + tempKey;
                byte[] rgbKey = Encoding.UTF8.GetBytes(myKey.Substring(0, 8));
                byte[] rgbIV = IV;
                byte[] decryptArray = Convert.FromBase64String(DecryptText);
                DESCryptoServiceProvider DCSP = new DESCryptoServiceProvider();
                using (MemoryStream mStream = new MemoryStream())
                {
                    CryptoStream cStream = new CryptoStream(mStream, DCSP.CreateDecryptor(rgbKey, rgbIV), CryptoStreamMode.Write);
                    cStream.Write(decryptArray, 0, decryptArray.Length);
                    cStream.FlushFinalBlock();
                    cStream.Close();
                    return Encoding.UTF8.GetString(mStream.ToArray());
                }
            }
            catch
            {
                return string.Empty;
            }

        }
        #endregion

        public static string ToBase64String(string str)
        {
            if (string.IsNullOrEmpty(str))
                return string.Empty;
            var s = Convert.ToBase64String(Encoding.UTF8.GetBytes(str));
            return s;

        }


        public static string FromBase64String(string base64Str)
        {
            if (string.IsNullOrEmpty(base64Str))
                return string.Empty;
            var str = base64Str;
            try
            {
                var bytes = Convert.FromBase64String(str);
                var str2 = Encoding.UTF8.GetString(bytes);
                return str2;
            }
            catch
            {
                return string.Empty;
            }


        }




    }
}
