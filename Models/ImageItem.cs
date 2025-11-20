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
        public string ImageSource { get; set; }
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
                ImageSource = @"..\..\..\Images\icons_folder-512.png";
                return;
            }

            // Custom Icons for extension
            switch (extension)
            {
                case ".ini":
                    _IsInIFile = true;
                    // Default Image.
                    ImageSource = @"..\..\..\Images\icons_document-512.png";
                    return;

                case ".aes":
                    _IsEncrypted = true;
                    ImageSource = @"..\..\..\Images\icons8-lock-file-64.png";
                    return;

                case ".rar":
                    ImageSource = @"..\..\..\Images\icons8-rar-100.png";
                    return;

                case ".zip":
                    ImageSource = @"..\..\..\Images\icons8-archive-folder-96.png";
                    return;
            }



            if(_fileExtensions.Contains(extension))
            {
				ImageSource = path;

                // BitmapImage image = CreateImage(path);
                //image.DecodePixelHeight = 5;
                //image.DecodePixelWidth = 5;

                

				//ImageSource = image.UriSource.ToString();
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
            image.EndInit();

            return image;
        }

        private ImageCodecInfo? GetEncoder(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();
            foreach(ImageCodecInfo codec in codecs)
            {
                if(codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }

        private byte[] CompressBitmapImage(BitmapImage image, int quality = 75)
        {
            // Dead code ---- Try code out before removing!!!
            //image.DecodePixelWidth = 200;
            //image.DecodePixelHeight = 200;

			var encoder = new JpegBitmapEncoder();
			encoder.QualityLevel = quality;
			encoder.Frames.Add(BitmapFrame.Create(image));

			using (var stream = new MemoryStream())
			{
				encoder.Save(stream);
				return stream.ToArray();
			}
		}

		private string CompressBitmapImageToString(BitmapImage image, int quality = 75)
		{
			// Dead code ---- Try code out before removing!!!
			//image.DecodePixelWidth = 200;
			//image.DecodePixelHeight = 200;

			var encoder = new JpegBitmapEncoder();
			encoder.QualityLevel = quality;
			encoder.Frames.Add(BitmapFrame.Create(image));

			using (var stream = new MemoryStream())
			{
				encoder.Save(stream);
				return Convert.ToBase64String(stream.ToArray());
			}
		}


		private void CompressImageForView(string InImagePath, int Quality)
        {

            using (Bitmap bitmap = new Bitmap(InImagePath))
            {
                ImageCodecInfo jpgEncoder = GetEncoder(ImageFormat.Jpeg);

                System.Drawing.Imaging.Encoder encoder =
                    System.Drawing.Imaging.Encoder.Quality;

                EncoderParameters encoderParameters = new EncoderParameters();
                EncoderParameter encoderParameter = new EncoderParameter(encoder, Quality);
                encoderParameters.Param[0] = encoderParameter;

                bitmap.Save(ImageSource, jpgEncoder, encoderParameters);
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
