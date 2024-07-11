using PhotoGallery.Managers;
using PhotoGallery.Models;
using System.IO;
using System.Windows.Input;


namespace PhotoGallery.ViewModels
{
    internal class MainWindowViewModel : ViewModelBase
    {
        public MainWindowInfo WindowInfo { get; set; }
        public ICommand ReloadCommand { get; set; }
        public ICommand BackCommand { get; set; }
        public ICommand EncryptAllCommand { get; set; }
        public ICommand DecryptAllCommand {  get; set; }

        public MainWindowViewModel()
        {
            WindowInfo = new MainWindowInfo();
            WindowInfo.WindowsDisplayData = "Hello World";

            BackCommand = new RelayCommand(execute => Back(), canExecute => { return true; });
            ReloadCommand = new RelayCommand(execute => Reload(), canExecute => { return true; });
            EncryptAllCommand = new RelayCommand(execute => EncryptAllFiles(), canExecute => { return true; });
            DecryptAllCommand = new RelayCommand(execute => DecryptAllFiles(), canExecute => { return true; });

        }

        // Commands
        private void Back()
        {
            if(FileContainer.Instance.MoveUpAFolder())
            {
                GalleryViewModel.SetFiles(FileContainer.Instance.GetItems());
            }
           
        }

        private void Reload()
        {
            FileContainer.Instance.Reload();
            GalleryViewModel.SetFiles(FileContainer.Instance.GetItems());
        }

        private void EncryptAllFiles()
        {

            Reload();
        }

        private void DecryptAllFiles()
        {
            RijndaelManagedEncryption.DecryptFolder(@"C:\Users\malac\Desktop\CS_Output",
                  @"C:\Users\malac\Desktop\CS_Input", "Password");
        }


    }
}
