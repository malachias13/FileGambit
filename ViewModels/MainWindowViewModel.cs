﻿using Microsoft.Win32;
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
using System.Configuration;
using Haley.Abstractions;
using Squirrel;
using File_Gambit.Views;
using File_Gambit.ViewModels;




namespace PhotoGallery.ViewModels
{
    enum View
    {
        GALLERY,
        SETTINGS
    }

    internal class MainWindowViewModel : ViewModelBase
    {
        public UserControl ConentWindow 
        {
            get { return _contentWindow; } 
            set { _contentWindow = value; OnPropertyChanged(); } 
        }
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

        public string WindowDisplayVersion
        {
            get { return _windowDisplayVersion; }
            set { _windowDisplayVersion = value; OnPropertyChanged(); }
        }

        public PascodePromptView PascodePromptWindow { get; set; }
        public UpdatePromptView UpdatePromptWindow { get; set; }
        public ICommand ReloadCommand { get; set; }
        public ICommand BackCommand { get; set; }
        public ICommand EncryptAllCommand { get; set; }
        public ICommand DecryptAllCommand {  get; set; }
        public ICommand LoadFolderCommand { get; set; }
        public ICommand SettingsCommand {  get; set; }
        public bool IsPascodePromptWindowOpen
        {
            get { return _IsPascodePromptWindowOpen; } 
            set { _IsPascodePromptWindowOpen = value;  OnPropertyChanged(); } 
        }

        public bool IsUpdatePromptWindowOpen
        {
            get { return _IsUpdatePromptWindowOpen; }
            set { _IsUpdatePromptWindowOpen = value; OnPropertyChanged(); }
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
        private bool _IsUpdatePromptWindowOpen = false;
        private bool _ShowAutoUpdatePopup = true;
        private View _currentView = View.GALLERY;

        private string? _password = null;
        private string _windowsDisplayData;
        private string _windowDisplayVersion;
        private Brush _windowsDisplayForeground;

        private UserControl _contentWindow;
        private UserControl _settingsWindow;
        private UserControl _galleryWindow;

        private PascodePromptViewModel _pascodePromptVM;
        private SettingsViewModel _settingsVM;
        private UpdatePromptViewModel _updatePromptVM;

        private UISettingsModel _UISettingSection;
        private UpdateManager _manager;

        // Path variables for source, encryption, and
        // decryption folders.
        string EncrAndDecrFolder;

        private Configuration AppConfig = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

        public MainWindowViewModel()
        {

            _galleryWindow = new GalleryView();
            PascodePromptWindow = new PascodePromptView();
            UpdatePromptWindow = new UpdatePromptView();
            _pascodePromptVM = new PascodePromptViewModel();
            _updatePromptVM = new UpdatePromptViewModel();
            _settingsWindow = new SettingsView();
            _settingsVM = new SettingsViewModel();

            PascodePromptWindow.DataContext = _pascodePromptVM;
            _settingsWindow.DataContext = _settingsVM;
            UpdatePromptWindow.DataContext = _updatePromptVM;

            ConentWindow = _galleryWindow;

            DisplayFileCountInfo();

            // GalleryVM Bind Actions
            GalleryViewModel.Instance.UpdateWindowInfoDisplay = DisplayFileCountInfo;
            GalleryViewModel.Instance.UpdateProgressBar = UpdateProgressBarInArray;

            // SettingsVM Bind Actions
            _settingsVM.SetBackgroundImage = GalleryViewModel.Instance.SetBackgroundImage;
            _settingsVM.SetBackgroundImageOpacity = GalleryViewModel.Instance.SetBackgroundOpacity;
            _settingsVM.SetBackgroundImmageStretch = GalleryViewModel.Instance.SetBackgroundStretch;
            _settingsVM.SetImageItemTextColor = GalleryViewModel.Instance.SetImageItemForegroundColor;
            _settingsVM.CheckForUpdatesAction = CheckForUpdates;
            _settingsVM.SetShowAutoUpdatePopup = SetShowAutoUpdatePopup;

            // UpdateVM bind Actions
            BindAllUpdateVM_Func();


            // Main window commands.
            BackCommand = new RelayCommand(execute => Back(), canExecute => CanRunBackCommand());
            ReloadCommand = new RelayCommand(execute => Reload(), canExecute => CanRunReloadCommand());
            EncryptAllCommand = new RelayCommand(execute => OpenPascodePrompt(true), canExecute => CanRunEncryptAllCommand());
            DecryptAllCommand = new RelayCommand(execute => OpenPascodePrompt(false), canExecute => CanRunDecryptAllCommand());
            LoadFolderCommand = new RelayCommand(execute => LoadFolder(), canExecute => { return !_isEncrypting || !_isDecrypting; });
            SettingsCommand = new RelayCommand(execute => Setting(), canExecute => CanRunSettingCommand());
            // Prompt Commands.
            _pascodePromptVM.ContinueCommand = 
                new RelayCommand(execute => ContinueCommand(), canExecute => { return _pascodePromptVM.IsVaildKey(); });

            _pascodePromptVM.CloseCommand =
                new RelayCommand(execute => ClosePascodePrompt(), canExecute => { return true; });

            SetupAppConfig();
            LoadUserSettings();
        }

        #region Buttons Commands

        private void Back()
        {
            if(_currentView == View.SETTINGS)
            {
                SaveUserSettings();
                ConentWindow = _galleryWindow;
                _currentView = View.GALLERY;
                return;
            }

            if (FileContainer.Instance.MoveUpAFolder() && _currentView == View.GALLERY)
            {
                GalleryViewModel.Instance.SetFiles(FileContainer.Instance.GetItems());
                DisplayFileCountInfo();
                return;
            }
           
        }

        private void Reload()
        {
            WindowsDisplayData = "Reloading...";
            FileContainer.Instance.Reload();
            GalleryViewModel.Instance.SetFiles(FileContainer.Instance.GetItems());
            DisplayFileCountInfo();
        }

        private void Setting()
        {
            ConentWindow = _settingsWindow;
            _currentView = View.SETTINGS;
        }

        private bool CanRunSettingCommand()
        {
            return _currentView != View.SETTINGS;
        }

        private bool CanRunEncryptAllCommand()
        {
            return !_isEncrypting && FileContainer.Instance.GetItems().Count > 0;
        }

        private bool CanRunDecryptAllCommand()
        {
            return !_isDecrypting && FileContainer.Instance.GetItems().Count > 0;
        }

        private bool CanRunReloadCommand()
        {
            return FileContainer.Instance.GetItems().Count > 0;
        }

        private bool CanRunBackCommand()
        {
            return (FileContainer.Instance.GetItems().Count > 0 && FileContainer.Instance.CanMoveUpAFolder()) || _currentView == View.SETTINGS;
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
                UpdateProgressBarInArray(0, 1);
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


        #region UpdateVM_Functions

        private void BindAllUpdateVM_Func()
        {
            _updatePromptVM.SaveData = SaveUserSettings;
            _updatePromptVM.CloseAction = CloseUpdatePromptWindow;
            _updatePromptVM.UpdateAction = Update;
            _updatePromptVM.SetAutoUpdatePopup = SetShowAutoUpdatePopup;
        }
        
        private void CloseUpdatePromptWindow()
        {
            IsUpdatePromptWindowOpen = false;
        }

        #endregion

        #region Helper functions

        public async void MainwindowLoaded(object sender, RoutedEventArgs e)
        {
            // File Gambit 1.0 created by Malachias Harris
            try
            {
                _manager = await UpdateManager
                .GitHubUpdateManager(@"https://github.com/malachias13/FileGambit");

                string text = $"File Gambit {_manager.CurrentlyInstalledVersion()} created by Malachias Harris";
                WindowDisplayVersion = text;
                CheckForUpdates();
            }
            catch
            {
                string text = "File Gambit created by Malachias Harris";
                WindowDisplayVersion = text;
            }

        }

        private async void CheckForUpdates()
        {
            try
            {
                SetIsCheckingForUpdates(true);
                var UpdateInfo = await _manager.CheckForUpdate(false, UpdateProgressBar);
                if (UpdateInfo.ReleasesToApply.Count > 0)
                {
                    // New version 8.14 of File Gambit is available for download.
                    _updatePromptVM.UpdateText = $"New version {UpdateInfo.ReleasesToApply.Last().Version} of File Gambit is available for download.";
                    IsUpdatePromptWindowOpen = true;
                }
                await Task.Delay(1000).ContinueWith(tt =>
                {
                    UpdateProgressBar(0);
                    SetIsCheckingForUpdates(false);
                });
            }
            catch 
            {
                WindowsDisplayForeground = Brushes.Red;
                WindowsDisplayData = "Failed to update...";
               await Task.Delay(3000).ContinueWith(tt => SetIsCheckingForUpdates(false));
            }
        }

        private void Update()
        {
            Task.Run(() => _manager.UpdateApp(UpdateProgressBar)).GetAwaiter().OnCompleted(() =>
            {
                CloseUpdatePromptWindow();
                UpdateProgressBar(0);
                string text = $"File Gambit {_manager.CurrentlyInstalledVersion()} created by Malachias Harris";
                WindowDisplayVersion = text;
            });

        }

        private void SetupAppConfig()
        {
            if (AppConfig.Sections["UISettings"] is null)
            {
                AppConfig.Sections.Add("UISettings", new UISettingsModel());
            }
            _UISettingSection = AppConfig.GetSection("UISettings") as UISettingsModel;
            GalleryViewModel.Instance.SetUISettingsModel(_UISettingSection);
        }

        private void LoadUserSettings()
        {
            _settingsVM.SetUISettings(_UISettingSection);
        }

        private void SaveUserSettings()
        {
            AppConfig.Save();
        }

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
                WindowsDisplayForeground = Brushes.LimeGreen;
                text += " successful";
            }

            WindowsDisplayData = text;
        }

        private void DisplayFileCountInfo()
        {
            WindowsDisplayForeground = Brushes.White;
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

        private void UpdateProgressBar(int  progress)
        {
            CurrentProgress = progress;
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

        private void SetShowAutoUpdatePopup(bool showAutoUpdate)
        {
            _ShowAutoUpdatePopup = showAutoUpdate;
        }

        private void SetIsCheckingForUpdates(bool isUpdating) 
        { 
            if(isUpdating)
            {
                WindowsDisplayForeground = WindowsDisplayForeground = Brushes.LimeGreen;
                WindowsDisplayData = "Checking for updates...";
            }
            else
            {
                DisplayFileCountInfo();
            }
            _settingsVM.SetIsCheckingForUpdates(isUpdating);
        }


    }
}
