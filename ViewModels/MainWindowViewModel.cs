using PhotoGallery.Models;
using System.Windows.Input;


namespace PhotoGallery.ViewModels
{
    internal class MainWindowViewModel : ViewModelBase
    {
        public MainWindowInfo WindowInfo { get; set; }
        public ICommand ReloadCommand { get; set; }
        public ICommand BackCommand { get; set; }

        public MainWindowViewModel()
        {
            WindowInfo = new MainWindowInfo();
            WindowInfo.WindowsDisplayData = "Hello World";

            BackCommand = new RelayCommand(execute => Back(), canExecute => { return true; });
            ReloadCommand = new RelayCommand(execute => Reload(), canExecute => { return true; });

        }

        // Command
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
       
    }
}
