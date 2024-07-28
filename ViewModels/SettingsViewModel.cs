using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace PhotoGallery.ViewModels
{
    internal class SettingsViewModel : ViewModelBase
    {
        public ObservableCollection<string> bgStretchSettings { get; set; }
        public SettingsViewModel() 
        {
            bgStretchSettings = new ObservableCollection<string>();

            bgStretchSettings.Add("None");
            bgStretchSettings.Add("Fill");
            bgStretchSettings.Add("Uniform");
            bgStretchSettings.Add("UniformToFill");
        }
    }
}
