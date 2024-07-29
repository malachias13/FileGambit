using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        [ConfigurationProperty("BackgroundImageOpacity", DefaultValue = 30f)]
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


	}
}
