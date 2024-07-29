using Microsoft.Win32;
using PhotoGallery.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;

namespace PhotoGallery.ViewModels
{
    internal class SettingsViewModel : ViewModelBase
    {
        public Action<string> SetBackgroundImage;
        public Action<float> SetBackgroundImageOpacity;
        public Action<string> SetBackgroundImmageStretch;

        public ObservableCollection<string> bgStretchSettings { get; set; }
        public ICommand ChoseImgCommand { get; set; }
        public float BGImgOpacity
        {
            get { return _imgOpacity; }
            set 
            {
                _imgOpacity = value;
                SetBackgroundImageOpacity?.Invoke(_imgOpacity);
                UpdateOpacityUI();
            }
        }
        public string BGImgOpacityText
        {
            get { return _imgOpacity.ToString(); }
            set 
            {
                if(float.TryParse(value, out float n))
                {
                    BGImgOpacity = Math.Clamp(n, 0, 100);
                }

                OnPropertyChanged(); 
            }
        }
        public string? BGImgPath
        {
            get { return _bgImgPath; }
            set { _bgImgPath = value; OnPropertyChanged(); }
        }

        public string bgStretchSelectValue
        {
            get { return _bgStretchSelectValue; }
            set 
            {   _bgStretchSelectValue = value;
                SetBackgroundImmageStretch?.Invoke(_bgStretchSelectValue);
                OnPropertyChanged(); 
            }
        }

        private float _imgOpacity = 30f;
        private string? _bgImgPath;
        private string _bgStretchSelectValue;

        public SettingsViewModel() 
        {
            ChoseImgCommand = new RelayCommand(execute => ChoseImageFromFolder(), canExecute => { return true; });
            bgStretchSettings = new ObservableCollection<string>();
            FillStretchSettings();

            BGImgOpacity = 30f;
            bgStretchSelectValue = bgStretchSettings[3];
        }

        public void SetUISettings(UISettingsModel settings)
        {
            if(FileContainer.Instance.BackgroundsFolderExist())
            {
                BGImgOpacity = settings.BackgroundImageOpacity;
                BGImgPath = settings.BackgroundImage;
                SetBackgroundImage.Invoke(BGImgPath);
                bgStretchSelectValue = settings.BackgroundImageStretch;
            }
        }

        #region Commands

        private void ChoseImageFromFolder()
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();
            string sep = string.Empty;
            string filter = string.Empty;

            Array.Reverse(codecs);

            foreach (var c in codecs)
            {
                string codecName = c.CodecName.Substring(8).Replace("Codec", "Files").Trim();
                filter = String.Format("{0}{1}{2} ({3})|{3}", filter, sep, codecName, c.FilenameExtension);
                sep = "|";
            }

            // filter = String.Format("{0}{1}{2} ({3})|{3}", filter, sep, "All Files", "*.*");

            var fileDialog = new OpenFileDialog
            {
                // Set options here
                AddExtension = true,
                Filter = filter,
                DefaultExt = ".png"
            };

            if (fileDialog.ShowDialog() == true)
            {
                // Clear all files in folder.
                FileContainer.Instance.ClearBackgroundsFolder();

                // Create and Copy file to Backgrounds folder.
                string BackgroundDir = @"..\..\..\Backgrounds";
                string destFile = Path.Combine(BackgroundDir, Path.GetFileName(fileDialog.FileName));
                if (!Directory.Exists(Path.GetFullPath(BackgroundDir)))
                {
                    Directory.CreateDirectory(Path.GetFullPath(BackgroundDir));
                }

                File.Copy(fileDialog.FileName, destFile, true );

                // Set background.
                SetBackgroundImage.Invoke(Path.GetFullPath(destFile));
                BGImgPath = Path.GetFileName(destFile);
            }
        }

        #endregion
        private void UpdateOpacityUI()
        {
            OnPropertyChanged(nameof(BGImgOpacity));
            OnPropertyChanged(nameof(BGImgOpacityText));
        }

        private void FillStretchSettings()
        {
            bgStretchSettings.Add("None");
            bgStretchSettings.Add("Fill");
            bgStretchSettings.Add("Uniform");
            bgStretchSettings.Add("UniformToFill");
        }
    }
}
