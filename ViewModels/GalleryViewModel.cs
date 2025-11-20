using PhotoGallery.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace PhotoGallery.ViewModels
{
    internal class GalleryViewModel : ViewModelBase
    {
        public static GalleryViewModel Instance;

        public string BackgroundImg 
        {   get { return _backgroundImage; }
            set { _backgroundImage = value; OnPropertyChanged(); }
        }
        public float BackgroundOpacity { get; set; }
        public Stretch BackgroundStretch {  get; set; }
        public SolidColorBrush BackgroundColor
        {
            get { return _backgroundColor; }
            set { _backgroundColor = value; OnPropertyChanged(); }
        }

        public Action UpdateWindowInfoDisplay;
        public Action<int,int> UpdateProgressBar;

        private UISettingsModel _settingsConfig;
        private ObservableCollection<ImageItem> _files;
        private SolidColorBrush _backgroundColor;
        private SolidColorBrush _ImageItemforegroundColor;
        private string _backgroundImage;
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

            _files = new ObservableCollection<ImageItem>();
            BackgroundColor = new SolidColorBrush(Color.FromRgb(92, 92, 92));

        }

        public void SetUISettingsModel(UISettingsModel uISettingsModel)
        {
            _settingsConfig = uISettingsModel;
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
                item.ImageItemColor = _ImageItemforegroundColor;
                _files.Add(item);
                UpdateProgressBar.Invoke(Progresscount, items.Count);
                Progresscount++;
            }
            UpdateProgressBar.Invoke(0, 1);
        }

        public void ClearAllFiles()
        {
            _files.Clear();
            FileContainer.Instance.GetItems().Clear();
        }

        public void SetBackgroundImage(string file)
        {
            BackgroundImg = file;
            _settingsConfig.BackgroundImage = BackgroundImg;
        }

        public void SetBackgroundOpacity(float opacity)
        {
            BackgroundOpacity = opacity/100;
            OnPropertyChanged(nameof(BackgroundOpacity));
            _settingsConfig.BackgroundImageOpacity = opacity;
        }

        public void SetBackgroundStretch(string StretchStr)
        {
            switch (StretchStr)
            {
                case "None":
                    BackgroundStretch = Stretch.None;
                    break;
                case "Fill":
                    BackgroundStretch = Stretch.Fill;
                    break;
                case "Uniform":
                    BackgroundStretch = Stretch.Uniform;
                    break;
                case "UniformToFill":
                    BackgroundStretch = Stretch.UniformToFill;
                    break;
            }
            OnPropertyChanged(nameof(BackgroundStretch));
            _settingsConfig.BackgroundImageStretch = StretchStr;
        }

        public void SetImageItemForegroundColor(SolidColorBrush color)
        {
            _ImageItemforegroundColor = color;
            _settingsConfig.ImageItemTextColor = color;
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
                  await Application.Current.Dispatcher.InvokeAsync(() =>
                    {
                        FileContainer.Instance.OpenFolder(item.Source);

					});

                    SetFiles(FileContainer.Instance.GetItems());
                }
                UpdateWindowInfoDisplay.Invoke();
            }
        }

    }
}
