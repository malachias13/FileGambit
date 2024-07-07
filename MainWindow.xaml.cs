using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using PhotoGallery.Views;
using PhotoGallery.Models;
using PhotoGallery.ViewModels;
using System.IO;

namespace PhotoGallery
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            MainWindowViewModel vm = new MainWindowViewModel();
            DataContext = vm;

            GalleryView.Instance.SetFileContainer(_fileContainer);
            LoadData();
        }
        private FileContainer _fileContainer = new FileContainer();

        public string NumberOfItems { get; set; }
        void LoadData()
        {
            List<ImageItem> imageItems = new List<ImageItem>();
            string path = "C:/Users/malac/Desktop/CS_Files/";

            _fileContainer.OpenFolder(path);
            GalleryView.Instance.UpdateGallery(_fileContainer.GetItems());
            NumberOfItems = _fileContainer.GetItems().Count + " Items";
        }

        private void backBtn_Click(object sender, RoutedEventArgs e)
        {
            _fileContainer.MoveUpAFolder();
            GalleryView.Instance.UpdateGallery(_fileContainer.GetItems());
        }

        private void ReloadBtn_Click(object sender, RoutedEventArgs e)
        {
            _fileContainer.Reload();
            GalleryView.Instance.UpdateGallery(_fileContainer.GetItems());
        }
    }
}