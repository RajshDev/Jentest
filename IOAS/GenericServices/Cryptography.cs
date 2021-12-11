using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Text;
using System.Security.Cryptography;
using System.Configuration;

namespace IOAS.GenericServices
{
    public class Cryptography
    {
        public static string Encrypt(string clearText, string Password)
        {
            byte[] clearBytes = System.Text.Encoding.Unicode.GetBytes(clearText);
            System.Security.Cryptography.PasswordDeriveBytes pdb =
                new System.Security.Cryptography.PasswordDeriveBytes
                    (Password, new byte[] {0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d,
            0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76});

            // PasswordDeriveBytes is for getting Key and IV.
            // Using PasswordDeriveBytes object we are first getting 32 bytes for the Key (the default
            //Rijndael key length is 256bit = 32bytes) and then 16 bytes for the IV.
            // IV should always be the block size, which is by default 16 bytes (128 bit) for Rijndael.

            byte[] encryptedData = Encrypt(clearBytes, pdb.GetBytes(32), pdb.GetBytes(16));
            return Convert.ToBase64String(encryptedData);
        }


        // Encrypt a byte array into a byte array using a key and an IV

        public static byte[] Encrypt(byte[] clearData, byte[] Key, byte[] IV)
        {
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            System.Security.Cryptography.Rijndael alg = System.Security.Cryptography.Rijndael.Create();

            // Algorithm. Rijndael is available on all platforms.

            alg.Key = Key;
            alg.IV = IV;
            System.Security.Cryptography.CryptoStream cs =
                new System.Security.Cryptography.CryptoStream(ms, alg.CreateEncryptor(),
                    System.Security.Cryptography.CryptoStreamMode.Write);

            //CryptoStream is for pumping our data.

            cs.Write(clearData, 0, clearData.Length);
            cs.Close();
            byte[] encryptedData = ms.ToArray();
            return encryptedData;
        }


        public static byte[] Decrypt(byte[] cipherData, byte[] Key, byte[] IV)
        {
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            System.Security.Cryptography.Rijndael alg = System.Security.Cryptography.Rijndael.Create();
            alg.Key = Key;
            alg.IV = IV;
            System.Security.Cryptography.CryptoStream cs =
                new System.Security.Cryptography.CryptoStream(ms, alg.CreateDecryptor(),
                    System.Security.Cryptography.CryptoStreamMode.Write);
            cs.Write(cipherData, 0, cipherData.Length);
            cs.Close();
            byte[] decryptedData = ms.ToArray();
            return decryptedData;
        }


        //Decrypt a string into a string using a password
        //Uses Decrypt(byte[], byte[], byte[])


        public static string Decrypt(string cipherText, string Password)
        {
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            System.Security.Cryptography.PasswordDeriveBytes pdb =
                new System.Security.Cryptography.PasswordDeriveBytes(
                    Password, new byte[] {0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65,
            0x64, 0x76, 0x65, 0x64, 0x65, 0x76});
            byte[] decryptedData = Decrypt(cipherBytes, pdb.GetBytes(32), pdb.GetBytes(16));
            return System.Text.Encoding.Unicode.GetString(decryptedData);
        }

    }
}