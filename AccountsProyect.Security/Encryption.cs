using AccountsProyect.BE;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AccountsProyect.Security
{
    public class Encryption
    {

        private const string TripleDESKey = "0123456789QwErTaSdFgZxCv";
        private byte[] Llave { get; set; }

        private byte[] Vector { get; set; }

        public Encryption()
        {
            string llave = "NMwgO9deAcEt9kDSCOBknUUilfZUtDYZdpoe3rYp9orernDb2xNmCw == ";
            string vector = "1OYeDrVnzlLpwf4jf / VUVTZVjZp849 + R";
            llave = TripleDESDecode(llave);
            vector = TripleDESDecode(vector);
            Llave = ASCIIEncoding.ASCII.GetBytes(llave);
            Vector = ASCIIEncoding.ASCII.GetBytes(vector);
        }


        public HashSalt EncryptPassword(string password)
        {
            byte[] salt = new byte[256 / 8]; // Generate a 256-bit salt using a secure PRNG
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }
            string encryptedPassw = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: 10000,
                numBytesRequested: 256 / 8
            ));
            return new HashSalt { Hash = encryptedPassw, Salt = salt };
        }

        public bool VerifyPassword(string enteredPassword, byte[] salt, string storedPassword)
        {
            string encryptedPassw = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: enteredPassword,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: 10000,
                numBytesRequested: 256 / 8
            ));
            return encryptedPassw == storedPassword;
        }



        public Encryption(string llave, string vector)
        {
            Llave = ASCIIEncoding.ASCII.GetBytes(llave);
            Vector = ASCIIEncoding.ASCII.GetBytes(vector);
        }

        public string AESEncrypt(string Mensaje)
        {
            byte[] inputBytes = Encoding.ASCII.GetBytes(Mensaje);
            byte[] encripted;
            RijndaelManaged cripto = new RijndaelManaged();
            using (MemoryStream ms =
            new MemoryStream(inputBytes.Length))
            {
                using (CryptoStream objCryptoStream =
                new CryptoStream(ms,
                cripto.CreateEncryptor(Llave, Vector),
                CryptoStreamMode.Write))
                {
                    objCryptoStream.Write(inputBytes, 0, inputBytes.Length);
                    objCryptoStream.FlushFinalBlock();
                    objCryptoStream.Close();
                }
                encripted = ms.ToArray();
            }

            return Convert.ToBase64String(encripted);
        }

        public string AESDecrypt(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return string.Empty;
            }
            string textoLimpio = text;
            try
            {
                byte[] inputBytes = Convert.FromBase64String(text);
                byte[] resultBytes = new byte[inputBytes.Length];
                RijndaelManaged cripto = new RijndaelManaged();
                using (MemoryStream ms = new MemoryStream(inputBytes))
                {
                    using (CryptoStream objCryptoStream =
                    new CryptoStream(ms, cripto.CreateDecryptor(Llave, Vector),
                    CryptoStreamMode.Read))
                    {
                        using (StreamReader sr =
                        new StreamReader(objCryptoStream, true))
                        {
                            textoLimpio = sr.ReadToEnd();
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                return exception.Message;
            }

            return textoLimpio;
        }

        public string TripleDESEncode(string value)
        {

            TripleDESCryptoServiceProvider des = new TripleDESCryptoServiceProvider();
            des.IV = new byte[8];
            PasswordDeriveBytes pdb = new PasswordDeriveBytes(TripleDESKey, new byte[-1 + 1]);
            des.Key = pdb.CryptDeriveKey("RC2", "MD5", 128, new byte[8]);
            System.IO.MemoryStream ms = new System.IO.MemoryStream((value.Length * 2) - 1);
            CryptoStream encStream = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write);
            byte[] plainBytes = System.Text.Encoding.UTF8.GetBytes(value);
            encStream.Write(plainBytes, 0, plainBytes.Length);
            encStream.FlushFinalBlock();
            byte[] encryptedBytes = new byte[(int)ms.Length - 1 + 1];
            ms.Position = 0;
            ms.Read(encryptedBytes, 0, (int)ms.Length);
            encStream.Close();
            return Convert.ToBase64String(encryptedBytes);
        }

        public string TripleDESDecode(string value)
        {
            TripleDESCryptoServiceProvider des = new TripleDESCryptoServiceProvider();
            des.IV = new byte[8];
            PasswordDeriveBytes pdb = new PasswordDeriveBytes(TripleDESKey, new byte[-1 + 1]);
            des.Key = pdb.CryptDeriveKey("RC2", "MD5", 128, new byte[8]);
            byte[] encryptedBytes = Convert.FromBase64String(value);
            System.IO.MemoryStream ms = new System.IO.MemoryStream(value.Length);
            CryptoStream decStream = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write);
            decStream.Write(encryptedBytes, 0, encryptedBytes.Length);
            decStream.FlushFinalBlock();
            byte[] plainBytes = new byte[(int)ms.Length - 1 + 1];
            ms.Position = 0;
            ms.Read(plainBytes, 0, (int)ms.Length);
            decStream.Close();
            return System.Text.Encoding.UTF8.GetString(plainBytes);
        }
    }
}
