using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace PhotoGallery.Models
{
    public class ImageItem
    {
        //private readonly Uri _source;
        public ImageItem(string path)
        {
            Source = path;
            Name = Path.GetFileName(path);
            SetImageSource(path);
        }

        ~ImageItem()
        {
            Clear();
        }
        
        public string Name { get; set; }
        public string Source { get; set; }
        public string ImageSource { get; set; }
        public int Id { get; set; }

        // Commands
        public ICommand OpenItemCommand { get; set; }

        private bool _IsFile = true;

        private string[] _fileExtensions = {".jpg", ".png", ".gif", ".webp" };

       private void SetImageSource(string path)
       {
            string extension = Path.GetExtension(path);

            
            if(extension == "")
            {
                _IsFile = false;
                // Folder Image.
                ImageSource = @"..\..\..\Images\icons_folder-512.png";
                return;
            }

            if(_fileExtensions.Contains(extension))
            {
                ImageSource = path;
            }
            else
            {
                // Default Image.
                ImageSource = @"..\..\..\Images\icons_document-512.png";
            }
        }

        public void Clear()
        {
            Source = string.Empty;
            Name = string.Empty;
            ImageSource = string.Empty;
        }

        public bool GetIsFile() { return _IsFile; }

    }
}
