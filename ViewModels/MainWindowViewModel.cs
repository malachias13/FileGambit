using Microsoft.Win32;
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
using System.Windows.Media;
using System.Xml.Linq;


namespace PhotoGallery.ViewModels
{
    internal class MainWindowViewModel : ViewModelBase
    {
        public UserControl GalleryWindow { get; set; }
        public string WindowsDisplayData 
        { 
            get { return _windowsDisplayData; }
            set {  _windowsDisplayData = value; OnPropertyChanged(); }
        }
        public Brush WindowsDisplayForeground 
        {
            get { return _windowsDisplayForeground; }
            set { _windowsDisplayForeground = value; OnPropertyChanged(); } 
        }

        public PascodePromptView PascodePromptWindow { get; set; }
        public ICommand ReloadCommand { get; set; }
        public ICommand BackCommand { get; set; }
        public ICommand EncryptAllCommand { get; set; }
        public ICommand DecryptAllCommand {  get; set; }
        public ICommand LoadFolderCommand { get; set; }
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
        private bool _HasClickedEncryptBtn = false;
        private float _currentProgress;
        private bool _IsPascodePromptWindowOpen = false;

        private string? _password = null;
        private string _windowsDisplayData;
        private Brush _windowsDisplayForeground;

        private PascodePromptViewModel _pascodePromptVM;

        // Path variables for source, encryption, and
        // decryption folders. Must end with a backslash.
        string EncrAndDecrFolder;

        public MainWindowViewModel()
        {

            GalleryWindow = new GalleryView();
            PascodePromptWindow = new PascodePromptView();
            _pascodePromptVM = new PascodePromptViewModel();

            PascodePromptWindow.DataContext = _pascodePromptVM;

            DisplayFileCountInfo();

            // GalleryVM Bind Actions
            GalleryViewModel.Instance.UpdateWindowInfoDisplay = DisplayFileCountInfo;

            // Main window commands.
            BackCommand = new RelayCommand(execute => Back(), canExecute => { return true; });
            ReloadCommand = new RelayCommand(execute => Reload(), canExecute => { return true; });
            EncryptAllCommand = new RelayCommand(execute => OpenPascodePrompt(true), canExecute => { return !_isEncrypting; });
            DecryptAllCommand = new RelayCommand(execute => OpenPascodePrompt(false), canExecute => { return !_isDecrypting; });
            LoadFolderCommand = new RelayCommand(execute => LoadFolder(), canExecute => { return !_isEncrypting || !_isDecrypting; });
            // Prompt Commands.
            _pascodePromptVM.ContinueCommand = 
                new RelayCommand(execute => ContinueCommand(), canExecute => { return _pascodePromptVM.IsVaildKey(); });

            _pascodePromptVM.CloseCommand =
                new RelayCommand(execute => ClosePascodePrompt(), canExecute => { return true; });
        }


        #region Buttons Commands

        private void Back()
        {
            if (FileContainer.Instance.MoveUpAFolder())
            {
                GalleryViewModel.Instance.SetFiles(FileContainer.Instance.GetItems());
                DisplayFileCountInfo();
            }
           
        }

        private void Reload()
        {
            FileContainer.Instance.Reload();
            GalleryViewModel.Instance.SetFiles(FileContainer.Instance.GetItems());
            DisplayFileCountInfo();
        }


        private void OpenPascodePrompt(bool IsEncrypting)
        {
            IsPascodePromptWindowOpen = true;
            _HasClickedEncryptBtn = IsEncrypting;
            if(IsEncrypting)
            {
                _pascodePromptVM.TitleTxt = "Encryption";
            }
            else
            {
                _pascodePromptVM.TitleTxt = "Decryption";
            }
        }
        
        private void ClosePascodePrompt()
        {
            IsPascodePromptWindowOpen = false;
            ClearPascodeData();
        }

        private void ContinueCommand()
        {
            _password = _pascodePromptVM.keycode;
            _pascodePromptVM.ClearKeyCode();
            if (_HasClickedEncryptBtn)
            {
                EncryptAllFiles();
            }
            else
            {
                DecryptAllFiles();
            }
            IsPascodePromptWindowOpen = false;
        }

        private void LoadFolder()
        {
            var folderDialog = new OpenFolderDialog
            {
                // Set options here
            };

            if (folderDialog.ShowDialog() == true)
            {
                FileContainer.Instance.ClearDirectory();
                FileContainer.Instance.OpenFolder(folderDialog.FolderName);
                GalleryViewModel.Instance.SetFiles(FileContainer.Instance.GetItems());
            }
        }


        private void EncryptAllFiles()
        {
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
                    ClearPascodeData();
                    Task.Delay(3000).ContinueWith(tt => { DisplayFileCountInfo(); });
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
                    ClearPascodeData();
                    Task.Delay(3000).ContinueWith(tt => { DisplayFileCountInfo(); });
                    CommandManager.InvalidateRequerySuggested();
                });
            } 
        }

        #endregion

        #region Helper functions

        private void DisplayEncryptInfo(FileInfo file, bool HasError)
        {
            string text;
            if(_HasClickedEncryptBtn)
            {
                text = "Encrypted:";
            }
            else
            {
                text = "Decrypted:";
            }

            text += " " + file.Name;
            if(HasError)
            {
                WindowsDisplayForeground = Brushes.Red;
                text += " failed";
            }
            else
            {
                WindowsDisplayForeground = Brushes.DarkGreen;
                text += " successful";
            }

            WindowsDisplayData = text;
        }

        private void DisplayFileCountInfo()
        {
            WindowsDisplayForeground = Brushes.Black;
            string text = ""+FileContainer.Instance.GetItems().Count + " items";
            WindowsDisplayData = text;
        }

        private void ClearPascodeData()
        {
            _password = null;
            _pascodePromptVM.ClearKeyCode();
        }

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
                FileInfo fileInfo = new FileInfo(files[i]);
                try
                {
                    ManagedEncryption.Decryptfile(fileInfo, EncrAndDecrFolder, _password);
                }
                catch {
                    //MessageBox.Show("Wrong password");
                    UpdateProgressBarInArray(i, files.Count);
                    string Tempfile = Path.ChangeExtension(files[i].Replace("Encrypt", "Decrypt"), "");
                    DeleteFile(Tempfile);
                    DisplayEncryptInfo(fileInfo, true);
                    continue;
                }
                DisplayEncryptInfo(fileInfo, false);


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
                FileInfo fileInfo = new FileInfo(files[i]);
                ManagedEncryption.EncryptFile(fileInfo, EncrAndDecrFolder, _password);

                if (DeleteFile(files[i]))
                {
                    UpdateProgressBarInArray(i, files.Count);
                }
                DisplayEncryptInfo(fileInfo, false);


            }
        }
        #endregion


    }
}
