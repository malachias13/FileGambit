using PhotoGallery.Managers;
using PhotoGallery.Models;
using PhotoGallery.Views;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
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
        public PascodePromptView PascodePromptWindow { get; set; }
        public ICommand ReloadCommand { get; set; }
        public ICommand BackCommand { get; set; }
        public ICommand EncryptAllCommand { get; set; }
        public ICommand DecryptAllCommand {  get; set; }
        public bool IsPascodePromptWindowOpen 
        {
            get { return _IsPascodePromptWindowOpen; } 
            set { _IsPascodePromptWindowOpen = value;  OnPropertyChanged(); } 
        }

        public float CurrentProgress
        {
            get { return _currentProgress; }
            private set
            {
                if (_currentProgress != value)
                {
                    _currentProgress = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _isEncrypting = false;
        private bool _isDecrypting = false;
        private float _currentProgress;
        private bool _IsPascodePromptWindowOpen = false;

        private string? _password = null;

        private PascodePromptViewModel _pascodePromptVM;

        // Path variables for source, encryption, and
        // decryption folders. Must end with a backslash.
        string EncrAndDecrFolder = @"C:\Users\malac\Desktop\CS_Files";

        const string path = @"C:\Users\malac\Desktop\CS_Files";



        public MainWindowViewModel()
        {

            GalleryWindow = new GalleryView();
            PascodePromptWindow = new PascodePromptView();
            _pascodePromptVM = new PascodePromptViewModel();

            PascodePromptWindow.DataContext = _pascodePromptVM;

            FileContainer.Instance.OpenFolder(path);
            GalleryViewModel.Instance.SetFiles(FileContainer.Instance.GetItems());

            WindowInfo = new MainWindowInfo();
            WindowInfo.WindowsDisplayData = "Hello World";

            BackCommand = new RelayCommand(execute => Back(), canExecute => { return true; });
            ReloadCommand = new RelayCommand(execute => Reload(), canExecute => { return true; });
            EncryptAllCommand = new RelayCommand(execute => OpenPascodePrompt(), canExecute => { return !_isEncrypting; });
            DecryptAllCommand = new RelayCommand(execute => DecryptAllFiles(), canExecute => { return !_isDecrypting; });

            // Prompt Commands.
            _pascodePromptVM.ContinueCommand = 
                new RelayCommand(execute => EncryptAllFiles(), canExecute => { return _pascodePromptVM.IsVaildKey(); });

            _pascodePromptVM.CloseCommand =
                new RelayCommand(execute => ClosePascodePrompt(), canExecute => { return true; });
        }


        #region Buttons Commands

        private void Back()
        {
            if (FileContainer.Instance.MoveUpAFolder())
            {
                GalleryViewModel.Instance.SetFiles(FileContainer.Instance.GetItems());
            }
           
        }

        private void Reload()
        {
            FileContainer.Instance.Reload();
            GalleryViewModel.Instance.SetFiles(FileContainer.Instance.GetItems());
        }


        private void OpenPascodePrompt()
        {
            IsPascodePromptWindowOpen = true;
        }
        
        private void ClosePascodePrompt()
        {
            IsPascodePromptWindowOpen = false;
            _password = null;
            _pascodePromptVM.ClearKeyCode();
        }


        private void EncryptAllFiles()
        {
            IsPascodePromptWindowOpen = false;

            EncrAndDecrFolder = FileContainer.Instance.GetCurrentPath();

            List<string> fileNames = new List<string>();
           foreach (ImageItem item in FileContainer.Instance.GetItems())
            {
                if(item.GetIsFile() && !item.GetIsIniFile() && !item.GetIsEncrypted())
                {
                    fileNames.Add(item.Source);
                }
                
            }
            if (fileNames.Count <= 0) { return; }

            GalleryViewModel.Instance.ClearAllFiles();

            if (_password is null)
            {
                MessageBox.Show("Key not set.");
            }
            else
            {
                _isEncrypting = true;
                Task task = Task.Delay(2000).ContinueWith(t => EncryptAllRapper(fileNames));
                task.GetAwaiter().OnCompleted(() =>
                {
                    Reload();
                    _isEncrypting = false;
                    CurrentProgress = 0;
                    CommandManager.InvalidateRequerySuggested();
                });

            }

        }

        private void DecryptAllFiles()
        {
            EncrAndDecrFolder = FileContainer.Instance.GetCurrentPath();

            List<string> fileNames = new List<string>();
            foreach (ImageItem item in FileContainer.Instance.GetItems())
            {
                if (item.GetIsFile() && !item.GetIsIniFile() && item.GetIsEncrypted())
                {
                    fileNames.Add(item.Source);
                }

            }
            if(fileNames.Count <= 0) { return; }

            GalleryViewModel.Instance.ClearAllFiles();


            if (_password is null)
            {
                MessageBox.Show("Key not set.");
            }
            else
            {
                _isDecrypting = true;
                Task task = Task.Delay(2000).ContinueWith(t => DecryptAllRapper(fileNames));
                task.GetAwaiter().OnCompleted(() =>
                {
                    Reload();
                    _isDecrypting = false;
                    CurrentProgress = 0;
                    CommandManager.InvalidateRequerySuggested();
                });
            } 
        }

        #endregion

        #region Helper functions

        private bool DeleteFile(string filePath)
        {
            try
            {
                File.Delete(filePath);
            }
            catch (IOException e)
            {
                MessageBox.Show(e.Message);
                return false;
            }
            return true;
        }

        private void UpdateProgressBarInArray(int index, int arrayCount)
        {
            CurrentProgress = (index * 100) / arrayCount;
        }

        private void DecryptAllRapper(List<string> files)
        {

            for (int i = 0; i < files.Count; i++)
            {

                try
                {
                    ManagedEncryption.Decryptfile(new FileInfo(files[i]), EncrAndDecrFolder, _password);
                }
                catch {
                    MessageBox.Show("Wrong password");
                    UpdateProgressBarInArray(i, files.Count);
                    string Tempfile = Path.ChangeExtension(files[i].Replace("Encrypt", "Decrypt"), "");
                    DeleteFile(Tempfile);
                    continue;
                }


                if (DeleteFile(files[i]))
                {
                    UpdateProgressBarInArray(i, files.Count);
                }

            }
        }

        private void EncryptAllRapper(List<string> files)
        {
            for (int i = 0; i < files.Count; i++)
            {

                ManagedEncryption.EncryptFile(new FileInfo(files[i]), EncrAndDecrFolder, _password);

                if (DeleteFile(files[i]))
                {
                    UpdateProgressBarInArray(i, files.Count);
                }

            }
        }
        #endregion


    }
}
