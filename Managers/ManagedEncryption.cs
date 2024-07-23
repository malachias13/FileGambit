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
    public class ManagedEncryption
    {
        public static void EncryptFile(FileInfo file, string destinationFolder, string password)
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();


            // Create instance of Aes for
            // symmetric encryption of the data.
            Aes aes = GetAES(password);
            ICryptoTransform transform = aes.CreateEncryptor();
          

            // Write the following to the FileStream
            // for the encrypted file (outFs):
            // - the encrypted cipher content

            // Change the file's extension to ".enc"
            string outFile =
                Path.Combine(destinationFolder, Path.ChangeExtension(file.Name, file.Extension + ".aes"));

            using (var outFs = new FileStream(outFile, FileMode.Create))
            {

                // Now write the cipher text using
                // a CryptoStream for encrypting.
                using (var outStreamEncrypted =
                    new CryptoStream(outFs, transform, CryptoStreamMode.Write))
                {
                    // By encrypting a chunk at
                    // a time, you can save memory
                    // and accommodate large files.
                    int count = 0;
                    int offset = 0;

                    // blockSizeBytes can be any arbitrary size.
                    int blockSizeBytes = aes.BlockSize / 8;
                    byte[] data = new byte[blockSizeBytes];
                    int bytesRead = 0;

                    using (var inFs = new FileStream(file.FullName, FileMode.Open))
                    {
                        do
                        {
                            count = inFs.Read(data, 0, blockSizeBytes);
                            offset += count;
                            outStreamEncrypted.Write(data, 0, count);
                            bytesRead += blockSizeBytes;
                        } while (count > 0);
                    }
                    outStreamEncrypted.FlushFinalBlock();
                }
            }
        }


        public static void Decryptfile(FileInfo file, string destinationFolder, string password)
        {
            // Create instance of Aes for
            // symmetric decryption of the data.
            Aes aes = GetAES(password);


            // Construct the file name for the decrypted file.
            string outFile =
                Path.ChangeExtension(file.FullName.Replace("Encrypt", "Decrypt"), "");

            // Use FileStream objects to read the encrypted
            // file (inFs) and save the decrypted file (outFs).
            using (var inFs = new FileStream(file.FullName, FileMode.Open))
            {

                Directory.CreateDirectory(destinationFolder);


                // Decrypt the key.
                ICryptoTransform transform = aes.CreateDecryptor();

                // Decrypt the cipher text from
                // from the FileSteam of the encrypted
                // file (inFs) into the FileStream
                // for the decrypted file (outFs).
                using (var outFs = new FileStream(outFile, FileMode.Create))
                {
                    int count = 0;
                    int offset = 0;

                    // blockSizeBytes can be any arbitrary size.
                    int blockSizeBytes = aes.BlockSize / 8;
                    byte[] data = new byte[blockSizeBytes];

                    // By decrypting a chunk a time,
                    // you can save memory and
                    // accommodate large files.

                    // Start at the beginning
                    // of the cipher text.
                    //inFs.Seek(startC, SeekOrigin.Begin);
                    using (var outStreamDecrypted =
                        new CryptoStream(outFs, transform, CryptoStreamMode.Write))
                    {
                        do
                        {
                            count = inFs.Read(data, 0, blockSizeBytes);
                            offset += count;
                            outStreamDecrypted.Write(data, 0, count);
                        } while (count > 0);

                        outStreamDecrypted.FlushFinalBlock();
                    }
                }
            }
        }



        private static Aes GetAES(string password)
        {
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);

            //although 'Aes' implements IDisposable, it can't be disposed of in this method
            //because a reference is being created in the calling method
            //using Aes aes = Aes.Create(); 
            Aes aes = Aes.Create();

            aes.KeySize = 256;
            aes.BlockSize = 128;

            byte[] aesKey = SHA256.Create().ComputeHash(passwordBytes);
            byte[] aesIV = MD5.Create().ComputeHash(passwordBytes);
            aes.Key = aesKey;
            aes.IV = aesIV;

            aes.Padding = PaddingMode.PKCS7;
            aes.Mode = CipherMode.CBC;

            return aes;
        }

    }
}
