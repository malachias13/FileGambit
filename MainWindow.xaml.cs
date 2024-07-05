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

            GalleryView.Instance.SetFileContainer(fileContainer);
            LoadData();
        }
        FileContainer fileContainer = new FileContainer();
        void LoadData()
        {
            List<ImageItem> imageItems = new List<ImageItem>();
            string path = "C:/Users/malac/Desktop/CS_Files/";

            fileContainer.OpenFolder(path);
            GalleryView.Instance.UpdateGallery(fileContainer.GetItems());
        }

        private void backBtn_Click(object sender, RoutedEventArgs e)
        {
            
        }
    }
}