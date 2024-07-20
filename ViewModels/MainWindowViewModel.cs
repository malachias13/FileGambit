using PhotoGallery.Managers;
using PhotoGallery.Models;
using PhotoGallery.Views;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml.Linq;


namespace PhotoGallery.ViewModels
{
    internal class MainWindowViewModel : ViewModelBase
    {
        public UserControl GalleryWindow { get; set; }
        public MainWindowInfo WindowInfo { get; set; }
        public ICommand ReloadCommand { get; set; }
        public ICommand BackCommand { get; set; }
        public ICommand EncryptAllCommand { get; set; }
        public ICommand DecryptAllCommand {  get; set; }
        


        // Declare CspParameters and RsaCryptoServiceProvider
        // objects with global scope of your Form class.
        readonly CspParameters _cspp = new CspParameters();
        RSACryptoServiceProvider _rsa;

        // Path variables for source, encryption, and
        // decryption folders. Must end with a backslash.
        const string EncrFolder = @"C:\Users\malac\Desktop\CS_Files";
        const string DecrFolder = @"C:\Users\malac\Desktop\CS_Files";
        const string SrcFolder = @"c:\docs\";

        // Public key file
        const string PubKeyFile = @"c:\encrypt\rsaPublicKey.txt";

        const string path = @"C:\Users\malac\Desktop\CS_Files";

        // Key container name for
        // private/public key value pair.
        const string KeyName = "Key01";

        public MainWindowViewModel()
        {

            GalleryWindow = new GalleryView();

            FileContainer.Instance.OpenFolder(path);
            GalleryViewModel.Instance.SetFiles(FileContainer.Instance.GetItems());

            WindowInfo = new MainWindowInfo();
            WindowInfo.WindowsDisplayData = "Hello World";

            BackCommand = new RelayCommand(execute => Back(), canExecute => { return true; });
            ReloadCommand = new RelayCommand(execute => Reload(), canExecute => { return true; });
            EncryptAllCommand = new RelayCommand(execute => EncryptAllFiles(), canExecute => { return true; });
            DecryptAllCommand = new RelayCommand(execute => DecryptAllFiles(), canExecute => { return true; });


            // Stores a key pair in the key container.
            _cspp.KeyContainerName = KeyName;
            _rsa = new RSACryptoServiceProvider(_cspp)
            {
                PersistKeyInCsp = true
            };

        }


        #region Buttons Commands

        private void Back()
        {
            if(FileContainer.Instance.MoveUpAFolder())
            {
                GalleryViewModel.Instance.SetFiles(FileContainer.Instance.GetItems());
            }
           
        }

        private void Reload()
        {
            FileContainer.Instance.Reload();
            GalleryViewModel.Instance.SetFiles(FileContainer.Instance.GetItems());
        }

        private void EncryptAllFiles()
        {
            List<string> fileNames = new List<string>();
           foreach (ImageItem item in FileContainer.Instance.GetItems())
            {
                if(item.GetIsFile() && !item.GetIsIniFile() && !item.GetIsEncrypted())
                {
                    fileNames.Add(item.Source);
                }
                
            }
            GalleryViewModel.Instance.ClearAllFiles();

            if (_rsa is null)
            {
                MessageBox.Show("Key not set.");
            }
            else
            {
                Task task = Task.Delay(2000).ContinueWith(t => EncryptAllRapper(fileNames));
                task.GetAwaiter().OnCompleted(() =>
                {
                    Reload();
                });

            }

        }

        private void DecryptAllFiles()
        {
            List<string> fileNames = new List<string>();
            foreach (ImageItem item in FileContainer.Instance.GetItems())
            {
                if (item.GetIsFile() && !item.GetIsIniFile() && item.GetIsEncrypted())
                {
                    fileNames.Add(item.Source);
                }

            }
            GalleryViewModel.Instance.ClearAllFiles();


            if (_rsa is null)
            {
                MessageBox.Show("Key not set.");
            }
            else
            {
                Task task = Task.Delay(2000).ContinueWith(t => DecryptAllRapper(fileNames));
                task.GetAwaiter().OnCompleted(() =>
                {
                    Reload();
                });
            } 
        }

        #endregion

        #region Helper functions
        private void DecryptAllRapper(List<string> files)
        {
            for (int i = 0; i < files.Count; i++)
            {
                DecryptFile(new FileInfo(files[i]));
            }
        }

        private void EncryptAllRapper(List<string> files)
        {
            for (int i = 0; i < files.Count; i++)
            {
                EncryptFile(new FileInfo(files[i]));
            }
        }
        #endregion


        private void EncryptFile(FileInfo file)
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();


            // Create instance of Aes for
            // symmetric encryption of the data.
            Aes aes = Aes.Create();
            ICryptoTransform transform = aes.CreateEncryptor();

            // Use RSACryptoServiceProvider to
            // encrypt the AES key.
            // rsa is previously instantiated:
            //    rsa = new RSACryptoServiceProvider(cspp);
            byte[] keyEncrypted = _rsa.Encrypt(aes.Key, false);

            // Create byte arrays to contain
            // the length values of the key and IV.
            int lKey = keyEncrypted.Length;
            byte[] LenK = BitConverter.GetBytes(lKey);
            int lIV = aes.IV.Length;
            byte[] LenIV = BitConverter.GetBytes(lIV);

            // Write the following to the FileStream
            // for the encrypted file (outFs):
            // - length of the key
            // - length of the IV
            // - encrypted key
            // - the IV
            // - the encrypted cipher content

            // Change the file's extension to ".enc"
            string outFile =
                Path.Combine(EncrFolder, Path.ChangeExtension(file.Name, file.Extension + ".aes"));

            using (var outFs = new FileStream(outFile, FileMode.Create))
            {
                outFs.Write(LenK, 0, 4);
                outFs.Write(LenIV, 0, 4);
                outFs.Write(keyEncrypted, 0, lKey);
                outFs.Write(aes.IV, 0, lIV);

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


        private void DecryptFile(FileInfo file)
        {
            // Create instance of Aes for
            // symmetric decryption of the data.
            Aes aes = Aes.Create();

            // Create byte arrays to get the length of
            // the encrypted key and IV.
            // These values were stored as 4 bytes each
            // at the beginning of the encrypted package.
            byte[] LenK = new byte[4];
            byte[] LenIV = new byte[4];

            // Construct the file name for the decrypted file.
            string outFile =
                Path.ChangeExtension(file.FullName.Replace("Encrypt", "Decrypt"), "");

            // Use FileStream objects to read the encrypted
            // file (inFs) and save the decrypted file (outFs).
            using (var inFs = new FileStream(file.FullName, FileMode.Open))
            {
                inFs.Seek(0, SeekOrigin.Begin);
                inFs.Read(LenK, 0, 3);
                inFs.Seek(4, SeekOrigin.Begin);
                inFs.Read(LenIV, 0, 3);

                // Convert the lengths to integer values.
                int lenK = BitConverter.ToInt32(LenK, 0);
                int lenIV = BitConverter.ToInt32(LenIV, 0);

                // Determine the start position of
                // the cipher text (startC)
                // and its length(lenC).
                int startC = lenK + lenIV + 8;
                int lenC = (int)inFs.Length - startC;

                // Create the byte arrays for
                // the encrypted Aes key,
                // the IV, and the cipher text.
                byte[] KeyEncrypted = new byte[lenK];
                byte[] IV = new byte[lenIV];

                // Extract the key and IV
                // starting from index 8
                // after the length values.
                inFs.Seek(8, SeekOrigin.Begin);
                inFs.Read(KeyEncrypted, 0, lenK);
                inFs.Seek(8 + lenK, SeekOrigin.Begin);
                inFs.Read(IV, 0, lenIV);

                Directory.CreateDirectory(DecrFolder);
                // Use RSACryptoServiceProvider
                // to decrypt the AES key.
                byte[] KeyDecrypted = _rsa.Decrypt(KeyEncrypted, false);

                // Decrypt the key.
                ICryptoTransform transform = aes.CreateDecryptor(KeyDecrypted, IV);

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
                    inFs.Seek(startC, SeekOrigin.Begin);
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


    }
}
