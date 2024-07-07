using PhotoGallery.Models;
using System.Configuration;
using System.Data;
using System.Windows;

namespace PhotoGallery
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private FileContainer _fileContainer;
        public App()
        {
            _fileContainer = new FileContainer();
        }
    }

}
