using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;

namespace PhotoGallery.ViewModels
{
    internal class SettingsViewModel : ViewModelBase
    {
        public ObservableCollection<string> bgStretchSettings { get; set; }
        public ICommand ChoseImgCommand { get; set; }
        public Stretch BackgroundStretch { get; set; }
        public float BGImgOpacity
        {
            get { return _imgOpacity; }
            set { _imgOpacity = value; OnPropertyChanged(); UpdateImgOpacityText(); }
        }
        public string BGImgOpacityText
        {
            get { return _bgImgOpacityText; }
            set { _bgImgOpacityText = value; OnPropertyChanged(); }
        }
        public string? BGImgPath
        {
            get { return _bgImgPath; }
            set { _bgImgPath = value; OnPropertyChanged(); }
        }

        public string bgStretchSelectValue
        {
            get { return _bgStretchSelectValue; }
            set { _bgStretchSelectValue = value; OnPropertyChanged(); }
        }

        private float _imgOpacity = 1.0f;
        private string? _bgImgPath;
        private string _bgStretchSelectValue;
        private string _bgImgOpacityText;

        public SettingsViewModel() 
        {
            bgStretchSettings = new ObservableCollection<string>();

            BGImgOpacity = 1.0f;

            bgStretchSettings.Add("None");
            bgStretchSettings.Add("Fill");
            bgStretchSettings.Add("Uniform");
            bgStretchSettings.Add("UniformToFill");
        }

        public void UpdateImgOpacityText()
        {
            // (new_max - new_min) / (old_max - old_min) * (v - old_min) + new_min
            
            string text = "" + (100 - 0) / (1 - 0) * (_imgOpacity - 0);
            BGImgOpacityText = text; 
        }
    }
}
