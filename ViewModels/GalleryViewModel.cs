using PhotoGallery.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace PhotoGallery.ViewModels
{
    internal class GalleryViewModel : ViewModelBase
    {
        public static GalleryViewModel Instance;

        public string BackgroundImg { get; set; }
        public float BackgroundOpacity { get; set; }
        public Stretch BackgroundStretch {  get; set; }
        public Brush BackgroundColor
        {
            get { return _backgroundColor; }
            set { _backgroundColor = value; OnPropertyChanged(); }
        }

        public Action UpdateWindowInfoDisplay;
        public Action<int,int> UpdateProgressBar;

        private ObservableCollection<ImageItem> _files;
        private Brush _backgroundColor;
        public ObservableCollection<ImageItem> Files {
            get
            {
                return _files;
            }
            set
            {
                OnPropertyChanged();
            }
        }

        public GalleryViewModel()
        {

            Instance = this;
            string path = @"C:\Users\malac\Desktop\CS_Files";

            _files = new ObservableCollection<ImageItem>();
            BackgroundColor = new SolidColorBrush(Color.FromRgb(92, 92, 92));

            BackgroundImg = @"C:\Users\malac\Pictures\1042725.png";
            BackgroundOpacity = 0.5f;
            BackgroundStretch = Stretch.UniformToFill;

            //FileContainer.Instance.OpenFolder(path);
            //SetFiles(FileContainer.Instance.GetItems());
        }

        public void SetFiles(List<ImageItem> items)
        {
            if (_files.Count > 0)
                _files.Clear();

            int Progresscount = 1;
            UpdateProgressBar.Invoke(0, items.Count);
            foreach (ImageItem item in items)
            {
                 item.OpenItemCommand = new RelayCommand(execute => OpenItem(item), canExecute => { return true; });
                _files.Add(item);
                UpdateProgressBar.Invoke(Progresscount, items.Count);
                Progresscount++;
            }
        }

        public void ClearAllFiles()
        {
            _files.Clear();
            FileContainer.Instance.GetItems().Clear();
        }

        // Commands
        private async void OpenItem(object sender)
        {
            ImageItem item = sender as ImageItem;

            if (item != null)
            {
                if (item.GetIsFile() == true)
                {
                    FileContainer.Instance.OpenFile(item.Source);
                }
                else
                {
                    await Task.Run(() => FileContainer.Instance.OpenFolder(item.Source));
                    SetFiles(FileContainer.Instance.GetItems());
                }
                UpdateWindowInfoDisplay.Invoke();
            }
        }

    }
}
