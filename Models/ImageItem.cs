using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
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
        private bool _IsInIFile = false;
        private bool _IsEncrypted = false;

        private string[] _fileExtensions = {".jpg", ".png", ".gif", ".webp", ".jpeg" };
        private string[] _videoExtensions = { ".mp4", ".mov", ".avi", ".wmv", ".avchd", ".webm","flv" };

       private void SetImageSource(string path)
       {
            string extension = Path.GetExtension(path).ToLower();

            
            if(extension == "")
            {
                _IsFile = false;
                // Folder Image.
                ImageSource = @"..\..\..\Images\icons_folder-512.png";
                return;
            }
            else if(extension == ".ini")
            {
                _IsInIFile = true;
            }
            else if(extension == ".aes")
            {
                _IsEncrypted = true;
                ImageSource = @"..\..\..\Images\icons8-lock-file-64.png";
                return;
            }

            if(_fileExtensions.Contains(extension))
            {
                ImageSource = path;
            }
            else if(_videoExtensions.Contains(extension))
            {
                ImageSource = @"..\..\..\Images\icons8-video-file-100.png";
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
        public bool GetIsIniFile() { return _IsInIFile; }
        public bool GetIsEncrypted() { return _IsEncrypted; }



    }
}
