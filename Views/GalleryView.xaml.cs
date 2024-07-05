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
using PhotoGallery.ViewModels;

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

        private FileContainer _FileContainer;

        private void ImageButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            ImageItem item = button.DataContext as ImageItem;

            if (item != null)
            {
                if(item.GetIsFile() == true)
                {
                    _FileContainer.OpenFile(item.Source);
                }
                else
                {
                    _FileContainer.OpenFolder(item.Source);
                    UpdateGallery(_FileContainer.GetItems());
                }

            }
        }

        public void UpdateGallery(List<ImageItem> data)
        {
            ImageBox.ItemsSource = null;
            ImageBox.ItemsSource = data;
        }

        public void SetFileContainer(FileContainer container)
        {
            _FileContainer = container;
        }
    }
}
