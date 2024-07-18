using PhotoGallery.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace PhotoGallery.ViewModels
{
    internal class GalleryViewModel : ViewModelBase
    {
        public static GalleryViewModel Instance;

        private ObservableCollection<ImageItem> _files;
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

            //FileContainer.Instance.OpenFolder(path);
            //SetFiles(FileContainer.Instance.GetItems());
        }

        public void SetFiles(List<ImageItem> items)
        {
            _files.Clear();
            foreach(ImageItem item in items)
            {
                 item.OpenItemCommand = new RelayCommand(execute => OpenItem(item), canExecute => { return true; });
                _files.Add(item);
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

            }
        }

    }
}
