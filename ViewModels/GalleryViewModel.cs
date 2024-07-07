using PhotoGallery.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoGallery.ViewModels
{
    internal class GalleryViewModel : ViewModelBase
    {
        public ObservableCollection<ImageItem> Files { get; set; }


        public GalleryViewModel()
        {
            string path = "C:/Users/malac/Desktop/CS_Files/";

            Files = new ObservableCollection<ImageItem>();
            FileContainer.Instance.OpenFolder(path);
            SetFiles(FileContainer.Instance.GetItems());
        }

        private void SetFiles(List<ImageItem> items)
        {
            Files.Clear();
            for(int i = 0; i < items.Count; i++)
            {
                Files.Add(items[i]);
            }
        }

    }
}
