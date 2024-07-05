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

            LoadData();
        }

        void LoadData()
        {
            List<ImageItem> imageItems = new List<ImageItem>();
            string path = "C:/Users/malac/Desktop/CS_Files/";
            foreach(string file in Directory.EnumerateDirectories(path))
            {
                imageItems.Add(new ImageItem(file));
            }

            foreach (string file in Directory.EnumerateFiles(path))
            {
                imageItems.Add(new ImageItem(file));
            }



            GalleryView.Instance.UpdateGallery(imageItems);
        }

    }
}