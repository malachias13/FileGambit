using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PhotoGallery.Managers
{
    public class RijndaelManagedEncryption
    {
        public static void EncryptFolder(string sourceFolder, string destinationFolder, string password)
        {
            // Generate a random salt value
            byte[] salt = new byte[8];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(salt);
            }

            // Create an AES encryption algorithm with the specified password and salt
            using (var aes = new AesManaged())
            {
                aes.KeySize = 256;
                aes.BlockSize = 128;
                var key = new Rfc2898DeriveBytes(password, salt, 1000).GetBytes(32);
                aes.Key = key;
                aes.IV = new byte[16];
                aes.Padding = PaddingMode.PKCS7;
                aes.Mode = CipherMode.CBC;

                // Create the destination folder if it doesn't exist
                if (!Directory.Exists(destinationFolder))
                {
                    Directory.CreateDirectory(destinationFolder);
                }

                // Encrypt each file in the source folder and copy it to the destination folder
                foreach (string filePath in Directory.GetFiles(sourceFolder))
                {
                    using (var fsIn = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                    {
                        using (var fsOut = new FileStream(Path.Combine(destinationFolder, Path.GetFileName(filePath)), FileMode.Create, FileAccess.Write))
                        {
                            // Write the salt value to the beginning of the output file
                            fsOut.Write(salt, 0, salt.Length);

                            // Encrypt the file using the AES algorithm
                            using (var cryptoStream = new CryptoStream(fsOut, aes.CreateEncryptor(), CryptoStreamMode.Write))
                            {
                                fsIn.CopyTo(cryptoStream);
                            }
                        }
                    }
                }

            }
        }

        public static void DecryptFolder(string sourceFolder, string destinationFolder, string password)
        {
            // Loop through each file in the source folder
            foreach (string filePath in Directory.GetFiles(sourceFolder))
            {
                // Read the salt value from the beginning of the input file
                byte[] salt = new byte[8];
                using (var fsIn = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    fsIn.Read(salt, 0, salt.Length);
                }

                // Create an AES decryption algorithm with the specified password and salt
                using (var aes = new AesManaged())
                {
                    aes.KeySize = 256;
                    aes.BlockSize = 128;
                    var key = new Rfc2898DeriveBytes(password, salt, 1000).GetBytes(32);
                    aes.Key = key;
                    aes.IV = new byte[16];
                    aes.Padding = PaddingMode.PKCS7;
                    aes.Mode = CipherMode.CBC;

                    // Decrypt the input file using the AES algorithm and copy it to the destination folder
                    using (var fsIn = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                    {
                        using (var cryptoStream = new CryptoStream(fsIn, aes.CreateDecryptor(), CryptoStreamMode.Read))
                        {
                            using (var fsOut = new FileStream(Path.Combine(destinationFolder, Path.GetFileName(filePath)), FileMode.Create, FileAccess.Write))
                            {
                                cryptoStream.CopyTo(fsOut);
                            }
                        }
                    }
                }
            }
        }

        //private static Aes GetAES(string password, byte[] salt)
        //{
        //    byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
        //    Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(passwordBytes, salt, 50000, HashAlgorithmName.SHA256);

        //    //although 'Aes' implements IDisposable, it can't be disposed of in this method
        //    //because a reference is being created in the calling method
        //    //using Aes aes = Aes.Create(); 
        //    Aes aes = Aes.Create();

        //    aes.KeySize = 256;
        //    aes.BlockSize = 128;
        //    //aes.Padding = PaddingMode.Zeros; 
        //    aes.Padding = PaddingMode.PKCS7;
        //    aes.Mode = CipherMode.CBC;
        //    aes.Key = key.GetBytes(aes.KeySize / 8);
        //    aes.IV = key.GetBytes(aes.BlockSize / 8);

        //    return aes;
        //}

        //public static void EncryptFile(string sourcePath, string destinationPath, string password)
        //{
        //    if (string.IsNullOrWhiteSpace(password)) return;

        //    // using FileStream outputStream = File.OpenWrite(destinationPath);
        //    //public FileStream(string path, FileMode mode, FileAccess access);
        //    FileStream outputStream = new FileStream(destinationPath, FileMode.Open, FileAccess.Write);

        //    byte[] salt = RandomNumberGenerator.GetBytes(32);
        //    outputStream.Write(salt, 0, salt.Length);

        //    using Aes aes = GetAES(password, salt);

        //    using CryptoStream cryptoStream = new CryptoStream(outputStream, aes.CreateEncryptor(), CryptoStreamMode.Write);
        //    //using ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
        //    //using CryptoStream cryptoStream = new CryptoStream(outputStream, encryptor, CryptoStreamMode.Write);
        //    using FileStream inputStream = new FileStream(sourcePath, FileMode.Open);

        //    long totalBytesRead = 0;
        //    int bytesRead = 0;
        //    //byte[] buffer = new byte[1024 * 1024];
        //    byte[] buffer = new byte[1024 * 4]; //4096

        //    try
        //    {
        //        long i = 0;
        //        while ((bytesRead = inputStream.Read(buffer, 0, buffer.Length)) > 0)
        //        {
        //            //add
        //            totalBytesRead += bytesRead;
        //            Debug.Write($"[{i}] bytesRead: {bytesRead} totalBytesRead: {totalBytesRead}{Environment.NewLine}");

        //            cryptoStream.Write(buffer, 0, bytesRead);

        //            //increment
        //            i++;
        //        }

        //        inputStream.Close();

        //        //flush
        //        cryptoStream.FlushFinalBlock();
        //    }
        //    catch (Exception ex)
        //    {
        //        throw;
        //    }
        //    finally
        //    {
        //        cryptoStream.Close();
        //        outputStream.Close();
        //    }
        //}

        //public static void DecryptFile(string sourcePath, string destinationPath, string password)
        //{
        //    if (string.IsNullOrWhiteSpace(password)) return;

        //    using FileStream inputStream = new FileStream(sourcePath, FileMode.Open);

        //    byte[] salt = new byte[32];
        //    inputStream.Read(salt, 0, salt.Length);

        //    //'Aes' implements IDisposable
        //    using Aes aes = GetAES(password, salt);

        //    using CryptoStream cryptoStream = new CryptoStream(inputStream, aes.CreateDecryptor(), CryptoStreamMode.Read);
        //    //using ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
        //    //using CryptoStream cryptoStream = new CryptoStream(inputStream, decryptor, CryptoStreamMode.Read);
        //    using FileStream outputStream = new FileStream(destinationPath, FileMode.Create);

        //    try
        //    {
        //        long totalBytesRead = 0;
        //        int bytesRead = 0;
        //        //byte[] buffer = new byte[1024 * 1024];
        //        byte[] buffer = new byte[1024 * 4]; //4096

        //        long i = 0;
        //        while ((bytesRead = cryptoStream.Read(buffer, 0, buffer.Length)) > 0)
        //        {
        //            //add
        //            totalBytesRead += bytesRead;
        //            Debug.Write($"[{i}] bytesRead: {bytesRead} totalBytesRead: {totalBytesRead}{Environment.NewLine}");

        //            outputStream.Write(buffer, 0, bytesRead);

        //            //increment
        //            i++;
        //        }

        //        //flush
        //        outputStream.Flush();

        //        //cryptoStream.Close(); //close in finally instead
        //    }
        //    catch (System.Security.Cryptography.CryptographicException ex)
        //    {
        //        if (ex.Message == "Padding is invalid and cannot be removed.")
        //        {
        //            //when an invalid password is entered and Padding = PaddingMode.PKCS7
        //            //one receives this exception
        //            Debug.WriteLine($"Error: Invalid password.");
        //            throw new Exception("Invalid password.", ex);
        //        }
        //        else
        //        {
        //            Debug.WriteLine($"Error: {ex.Message}");
        //            throw;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw;
        //    }
        //    finally
        //    {
        //        cryptoStream.Close();
        //        outputStream.Close();
        //        inputStream.Close();
        //    }
        //}

    }
}
