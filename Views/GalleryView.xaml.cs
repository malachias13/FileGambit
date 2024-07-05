using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using PhotoGallery.Models;

namespace PhotoGallery.Views
{
    /// <summary>
    /// Interaction logic for GalleryView.xaml
    /// </summary>
    public partial class GalleryView : UserControl
    {
        public static GalleryView Instance { get; private set; }

        public GalleryView()
        {
            InitializeComponent();

            Instance = this;
        }

        private void ImageButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            ImageItem item = button.DataContext as ImageItem;

            if (item != null)
            {
                if(item.GetIsFile() == true)
                {
                    OpenFile(item.Source);
                }
                else
                {
                    OpenFolder(item.Source);
                }

            }

        }

        public void UpdateGallery(List<ImageItem> data)
        {
            ImageBox.ItemsSource = data;
        }

        private void OpenFile(string Source)
        {
            ProcessStartInfo Process_Info = new ProcessStartInfo(Source, @"%SystemRoot%\System32\rundll32.exe % ProgramFiles %\Windows Photo Viewer\PhotoViewer.dll, ImageView_Fullscreen %1")
            {
                UseShellExecute = true,
                WorkingDirectory = System.IO.Path.GetDirectoryName(Source),
                FileName = Source,
                Verb = "OPEN"
            };
            Process.Start(Process_Info);
        }

        private void OpenFolder(string path)
        {
            List<ImageItem> imageItems = new List<ImageItem>();

            foreach (string file in Directory.EnumerateDirectories(path))
            {
                imageItems.Add(new ImageItem(file));
            }

            foreach (string file in Directory.EnumerateFiles(path))
            {
                imageItems.Add(new ImageItem(file));
            }
            UpdateGallery(imageItems);
        }

    }
}
