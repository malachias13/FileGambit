using PhotoGallery.Models;


namespace PhotoGallery.ViewModels
{
    internal class MainWindowViewModel : ViewModelBase
    {
        public MainWindowInfo WindowInfo { get; set; }
        public MainWindowViewModel()
        {
            WindowInfo = new MainWindowInfo();
            WindowInfo.WindowsDisplayData = "Hello World";
        }

       
    }
}
