using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace PhotoGallery.Models
{
    internal class UISettingsModel : ConfigurationSection
    {
		[ConfigurationProperty("BackgroundImage")]
		public string BackgroundImage
        {
			get { return (string)this["BackgroundImage"]; }
			set { this["BackgroundImage"] = value; }
		}

        [ConfigurationProperty("BackgroundImageOpacity", DefaultValue = 100f)]
		public float BackgroundImageOpacity
		{
            get { return (float)this["BackgroundImageOpacity"]; }
            set { this["BackgroundImageOpacity"] = value; }
        }

        [ConfigurationProperty("BackgroundImageStretch", DefaultValue = "UniformToFill")]
        public string BackgroundImageStretch
        {
            get { return (string)this["BackgroundImageStretch"]; }
            set { this["BackgroundImageStretch"] = value; }
        }

        [ConfigurationProperty("ImageItemTextColor")]
        public SolidColorBrush ImageItemTextColor
        {
            get { return (SolidColorBrush)this["ImageItemTextColor"]; }
            set { this["ImageItemTextColor"] = value; }
        }

        [ConfigurationProperty("SetShowAutoUpdatePopup")]
        public bool SetShowAutoUpdatePopup
        {
            get { return (bool)this["SetShowAutoUpdatePopup"]; }
            set { this["SetShowAutoUpdatePopup"] = value; }
        }

	}
}
