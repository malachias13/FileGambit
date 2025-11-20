using Haley.Utils;
using PhotoGallery.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using static System.Net.Mime.MediaTypeNames;

namespace PhotoGallery.Models
{
    public class ImageItem
    {
        //private readonly Uri _source;
        public ImageItem(string path)
        {
            Source = path;
            Name = Path.GetFileName(path);

            //Task.Run(() =>
            //{
            //    SetImageSource(path);
            //});

            SetImageSource(path);
        }

        ~ImageItem()
        {
            Clear();
        }
        
        public string Name { get; set; }
        public string Source { get; set; }
        public BitmapImage? ImageSource { get; set; }
        public int Id { get; set; }


        public SolidColorBrush ImageItemColor { get; set; }

        // Commands
        public ICommand OpenItemCommand { get; set; }

        private bool _IsFile = true;
        private bool _IsInIFile = false;
        private bool _IsEncrypted = false;

        private string[] _fileExtensions = {".jpg", ".png", ".gif", ".webp", ".jpeg" };
        private string[] _compressedFileExtensions = { ".rar", ".zip" };
        private string[] _videoExtensions = { ".mp4", ".mov", ".avi", ".wmv", ".avchd", ".webm","flv" };
        private string[] _FoldersExtensions = { "!app" };

        private void SetImageSource(string path)
       {
            string extension = Path.GetExtension(path).ToLower();
            
            if(extension == "" || IsAFolder(extension))
            {
                _IsFile = false;
                // Folder Image.
                ImageSource = CreateImage(new Uri("pack://application:,,,/Images/icons_folder-512.png"));
                return;
            }

            // Custom Icons for extension
            switch (extension)
            {
                case ".ini":
                    _IsInIFile = true;
                    // Default Image.
                    ImageSource =
                        CreateImage(new Uri("pack://application:,,,/Images/icons_document-512.png"));
                    return;

                case ".aes":
                    _IsEncrypted = true;
                    ImageSource = CreateImage(new Uri("pack://application:,,,/Images/icons8-lock-file-64.png"));
                    return;

                case ".rar":
                    ImageSource = CreateImage(new Uri("pack://application:,,,/Images/icons8-rar-100.png"));
                    return;

                case ".zip":
                    ImageSource = CreateImage(new Uri("pack://application:,,,/Images/icons8-archive-folder-96.png"));
                    return;
            }



            if(_fileExtensions.Contains(extension))
            {
				// ImageSource = path;
                ImageSource = CreateImage(path);
			}
            else if(_videoExtensions.Contains(extension))
            {
                ImageSource = CreateImage(new Uri("pack://application:,,,/Images/icons8-video-file-100.png"));
            }
            else
            {
                // Default Image.
                ImageSource = CreateImage(new Uri("pack://application:,,,/Images/icons_document-512.png"));
            }

		}

        private bool IsAFolder(string extension)
        {
            if(extension.Length >= 4 && _FoldersExtensions.Contains(extension.Substring(extension.Length - 4))) 
            {
                return true;
            }
            else
            {
                return _FoldersExtensions.Contains(extension);
            }

        }
        
        private BitmapImage CreateImage(string path)
        {
            BitmapImage image = new BitmapImage();

            image.BeginInit();
            image.UriSource = new Uri(path);

			image.DecodePixelWidth = 200;
            image.CacheOption = BitmapCacheOption.OnLoad;
			image.EndInit();

            return image;
        }

		private BitmapImage CreateImage(Uri path)
		{
			BitmapImage image = new BitmapImage();
			image.BeginInit();
			image.UriSource = path;

			image.DecodePixelWidth = 200;
			image.CacheOption = BitmapCacheOption.OnLoad;
			image.EndInit();

			return image;
		}

        public void Clear()
        {
            Source = string.Empty;
            Name = string.Empty;
            ImageSource = null;
        }

        public bool GetIsFile() { return _IsFile; }
        public bool GetIsIniFile() { return _IsInIFile; }
        public bool GetIsEncrypted() { return _IsEncrypted; }



    }
}
