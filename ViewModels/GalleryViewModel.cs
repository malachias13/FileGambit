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
        public static ObservableCollection<ImageItem> Files { get; set; }

        public GalleryViewModel()
        {
            string path = "C:/Users/malac/Desktop/CS_Files/";

            Files = new ObservableCollection<ImageItem>();

            FileContainer.Instance.OpenFolder(path);
            SetFiles(FileContainer.Instance.GetItems());
        }

        public static void SetFiles(List<ImageItem> items)
        {
            Files.Clear();
            foreach(ImageItem item in items)
            {
                item.OpenItemCommand = new RelayCommand(execute => OpenItem(item), canExecute => { return true; });
                Files.Add(item);
            }
        }

        // Commands
        private static async void OpenItem(object sender)
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
