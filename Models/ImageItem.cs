﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace PhotoGallery.Models
{
    public class ImageItem
    {
        private readonly Uri _source;
        public ImageItem(string path)
        {
            Source = path;
            _source = new Uri(path);
            Name = Path.GetFileName(path);
            SetImageSource(path);
        }
        public string Name { get; set; }
        public string Source { get; }
        public string ImageSource { get; set; }
        public int Id { get; set; }

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

        public bool GetIsFile() { return _IsFile; }

    }
}
