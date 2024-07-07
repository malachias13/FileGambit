using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using static System.Net.Mime.MediaTypeNames;

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

            GalleryViewModel vm = new GalleryViewModel();
            DataContext = vm;

        }
    }
}
