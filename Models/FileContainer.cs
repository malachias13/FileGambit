using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoGallery.Models
{
    public class FileContainer
    {
        public static FileContainer Instance { get; private set; }
        public FileContainer() 
        {
            Instance = this;
        }
      
        private Stack<string> _Directory = new Stack<string>();
        private List<ImageItem> _Images = new List<ImageItem>();

        public void OpenFolder(string path)
        {
            _Images.Clear();
            if (!_Directory.Contains(path))
            {
                _Directory.Push(path);
            }
            foreach (string file in Directory.EnumerateDirectories(path))
            {
                _Images.Add(new ImageItem(file));
            }

            foreach (string file in Directory.EnumerateFiles(path))
            {
                _Images.Add(new ImageItem(file));
            }
        }

        public bool MoveUpAFolder()
        {
            if(_Directory.Count <= 1) { return false; }

            _Directory.Pop();
            OpenFolder(GetCurrentPath());
            return true;
        }

        public void OpenFile(string Source)
        {
            ProcessStartInfo Process_Info = new ProcessStartInfo(Source, @"%SystemRoot%\System32\rundll32.exe % ProgramFiles %\Windows Photo Viewer\PhotoViewer.dll, ImageView_Fullscreen %1")
            {
                UseShellExecute = true,
                WorkingDirectory = System.IO.Path.GetDirectoryName(Source),
                FileName = Source,
                Verb = "OPEN"
            };
            Process.Start(Process_Info);
        }

        public void Reload()
        {
            OpenFolder(GetCurrentPath());
        }

        public void ClearDirectory()
        {
            _Directory.Clear();
        }
        // Getters
        public string GetCurrentPath() { return _Directory.Peek(); }
        public List<ImageItem> GetItems() { return _Images; }

    }
}
