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
            set 
            {
                _imgOpacity = value;
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
            set { _bgStretchSelectValue = value; OnPropertyChanged(); }
        }

        private float _imgOpacity = 30f;
        private string? _bgImgPath;
        private string _bgStretchSelectValue;

        public SettingsViewModel() 
        {
            bgStretchSettings = new ObservableCollection<string>();

            BGImgOpacity = 30f;

            bgStretchSettings.Add("None");
            bgStretchSettings.Add("Fill");
            bgStretchSettings.Add("Uniform");
            bgStretchSettings.Add("UniformToFill");
        }

        private void UpdateOpacityUI()
        {
            OnPropertyChanged(nameof(BGImgOpacity));
            OnPropertyChanged(nameof(BGImgOpacityText));
        }
    }
}
